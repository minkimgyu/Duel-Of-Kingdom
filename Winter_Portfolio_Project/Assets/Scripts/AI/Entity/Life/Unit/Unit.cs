using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI.TARGET;
using Tree = BehaviorTree.Tree;
using WPP.AI.ATTACK;
using WPP.AI.BTUtility;
using WPP.AI.CAPTURE;
using WPP.AI.GRID;
using WPP.AI.FSM;
using WPP.AI.ACTION.STATE;
using WPP.POOL;

namespace WPP.AI.UNIT
{
    abstract public class Unit : LifeAI
    {
        public enum Type
        {
            AirUnit,
            GroundUnit,
        }

        // 기본적인 겹치는 기능은 여기서 구현
        // 이동성을 가짐

        // Ready --> Active State로 전환됨

        /// <summary>
        /// 벽을 무시할건지 확인.
        /// 공중 유닛의 경우 성립
        /// </summary>
        protected bool IgnoreWall { get { return CompareTag(Type.AirUnit.ToString()); } }

        protected MoveComponent _moveComponent;
        protected ViewComponent _viewComponent;
        protected CaptureComponent _captureComponent; // 포착 기능은 포탑이랑 유닛 클레스만 가지고 있음

        protected CapsuleCollider _capsuleCollider;

        protected List<Vector3> _posListForDrawingGizmo = new List<Vector3>(); // 이거는 BT에서 지정해주기

        public override float ReturnHpContainerScale() { return 0.5f; }

        protected override void InitializeComponent()
        {
            _moveComponent = GetComponent<MoveComponent>();
            _viewComponent = GetComponent<ViewComponent>();

            _captureComponent = GetComponentInChildren<CaptureComponent>();

            _capsuleCollider = GetComponent<CapsuleCollider>();

            base.InitializeComponent();
        }

        protected void ResetPosListForDrawingGizmo(List<Vector3> posList) => _posListForDrawingGizmo = posList;

        protected bool IsPosListEmpty() { return _posListForDrawingGizmo.Count == 0; }

        protected override void DrawGizmo() // 이것도 이벤트로 넘겨주자
        {
            base.DrawGizmo();

            // 가장 최근 리턴한 길을 보여줌
            if (_posListForDrawingGizmo.Count == 0) return;

            Gizmos.color = Color.blue;

            for (int i = 0; i < _posListForDrawingGizmo.Count - 1; i++)
                Gizmos.DrawLine(
                    new Vector3(_posListForDrawingGizmo[i].x, 1.5f, _posListForDrawingGizmo[i].z),
                    new Vector3(_posListForDrawingGizmo[i + 1].x, 1.5f, _posListForDrawingGizmo[i + 1].z));

        }

        public override float ReturnColliderSize()
        {
            return _capsuleCollider.radius * (transform.localScale.x + transform.localScale.z) / 2; // 이거는 캡슐콜라이더를 사용함 + scale의 평균 값만 적용시켜야함
        }

        public override void Die()
        {
            ObjectPooler.SpawnFromPool("DieEffect", transform.position);
            base.Die();
        }
    }

    // 기본적인 공격 기능이 갖춰진 유닛
    // Ground 또는 Air 유닛인지는 하위 클레스에서 직접 할당
    abstract public class AttackUnit : Unit
    {
        // 기본 능력치
        protected float _damage; // 데미지
        protected float _hitSpeed; // 공격 속도
        protected float _range; // 범위

        protected float _offsetDistance = 0.45f; // 추적 거리 오프셋
        protected float _directFollowOffset = 2f; // 직접 추적 기능 활성화 거리
        //

        protected float _delayDuration = 0; // 스폰 시 일정 시간 딜레이

        AttackComponent _attackComponent;

        public override void ResetDelayAfterSpawn(float delayDuration) { _delayDuration = delayDuration; }

        protected override void InitializeComponent()
        {
            _attackComponent = GetComponent<AttackComponent>();
            _attackComponent.Initialize(_damage);
            base.InitializeComponent();
        }

        public override void Initialize(int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange)
        {
            // BT에 사용될 변수는 여기서 초기화해야함

            //_id = id;
            _level = level;
            _name = name;

            _maxHp = hp; // 최대 체력 지정
            HP = hp;
            _damage = damage;
            _hitSpeed = hitSpeed;
            _range = range;

            InitializeComponent(); 
            // 여기서 컴포넌트를 가져와서 초기화해준다.
            // 추가로 BT도 초기화해준다.
           
            _captureComponent.Initialize(targetTag, OwnershipId, captureRange); // 이런 식으로 세부 변수를 할당해준다.

            Dictionary<ActionState, BaseState> attackStates = new Dictionary<ActionState, BaseState>()
            {
                {ActionState.Ready, new ReadyState(this, _delayDuration)},
                {ActionState.Active, new ActiveState(this)}
            };

            InitializeFSM(attackStates, ActionState.Ready);
        }

        protected override void InitializeBT()
        {
            GameObject go = GameObject.FindWithTag("Grid");
            if (go == null) return;

            PathFinder pathFinder = go.GetComponent<PathFinder>();
            if (pathFinder == null) return;

            List<Node> _childNodes = new List<Node>()
            {
                new Selector
                (
                    new List<Node>()
                    {
                        new Sequence
                        (
                            new List<Node>
                            {
                                new CanFindTarget(_captureComponent), // 만약 타겟이 없다면 타워를 타겟으로 지정해준다.
                                new Selector
                                (
                                    new List<Node>
                                    {
                                        new Selector
                                        (
                                            new List<Node>
                                            {
                                                new Sequence
                                                (
                                                    new List<Node>
                                                    {
                                                        // 타겟이 바뀌는 경우, 일정 시간 딜레이 넣어주기
                                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, _range, _offsetDistance, _attackComponent, true, ReturnColliderSize()), // DelayForAttack도 넣어주기
                                                        new LookAtTarget(_captureComponent, _viewComponent),
                                                        new Stop(_moveComponent),

                                                        new Attack(_attackComponent, _captureComponent)
                                                        // 공격 진행
                                                        // 만약 공격이 진행 중인 경우 거리가 멀어져도 계속 진행
                                                    }
                                                ),
                                                new Sequence
                                                (
                                                    new List<Node>
                                                    {
                                                        new CheckIsNear(_captureComponent, _range + _directFollowOffset),
                                                        new GoDirectToPoint(_captureComponent, _moveComponent, _viewComponent, ResetPosListForDrawingGizmo, IsPosListEmpty)
                                                    }
                                                )
                                            }
                                        ),
                                        new FollowPath(_moveComponent, _viewComponent, _captureComponent, pathFinder, ResetPosListForDrawingGizmo, IsMyEntity, IgnoreWall)
                                    }
                                )
                            }
                        ),
                        new Stop(_moveComponent)
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }
    }
}