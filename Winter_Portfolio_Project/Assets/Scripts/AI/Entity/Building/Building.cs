using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI.STAT;
using WPP.AI.ATTACK;
using WPP.AI.CAPTURE;
using WPP.AI.BTUtility;
using WPP.AI.GRID;
using WPP.AI.FSM;
using WPP.AI.ACTION.STATE;
using System;

namespace WPP.AI.BUILDING
{
    // 기본 기능은 InitializeBT 여기에서 구현하고
    // 추가로 구현할 기능이 있으면 하위 클레스를 따로 추가 제작해서 적용시키기

    abstract public class AttackBuilding : Building
    {
        // 기본 능력치
        protected float _damage; // 데미지
        protected float _hitSpeed; // 공격 속도
        protected float _range; // 범위
        //

        protected float _offsetDistance = 0.2f; // 추적 거리 오프셋

        [SerializeField] Transform _arm;
        protected CaptureComponent _captureComponent;
        protected ViewComponent _viewComponent;
        protected AttackComponent _attackComponent;

        protected override void InitializeComponent()
        {
            _captureComponent = GetComponentInChildren<CaptureComponent>();
            _viewComponent = GetComponentInChildren<ViewComponent>();
            _attackComponent = GetComponent<AttackComponent>();
            _attackComponent.Initialize(_damage);

            base.InitializeComponent();
        }

        public override void Initialize(int id, int level, string name, float hp, OffsetFromCenter fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange)
        {
            _id = id;
            _level = level;
            _name = name;

            _maxHp = hp; // 최대 체력 지정
            HP = hp;
            _damage = damage;
            _hitSpeed = hitSpeed;
            _range = range;

            _fillOffset = fillOffset;

            InitializeComponent();

            _captureComponent.Initialize(targetTag, PlayerId, captureRange); // 이런 식으로 세부 변수를 할당해준다.
        }

        protected override void InitializeBT()
        {
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
                                new Sequence
                                (
                                    new List<Node>
                                    {
                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, _range, _offsetDistance, _attackComponent), // DelayForAttack도 넣어주기
                                        new LookAtTarget(_captureComponent, _viewComponent, _arm),

                                        new Attack(_attackComponent, _captureComponent)
                                        // 공격 진행
                                        // 만약 공격이 진행 중인 경우 거리가 멀어져도 계속 진행
                                    }
                                )
                            }
                        ),
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }
    }

    public class LiveOutAttackBuilding : AttackBuilding
    {
        LiveOutComponent _liveOutComponent;
        protected float _delayDuration = 0; // 스폰 시 일정 시간 딜레이

        // Ready --> Active State로 전환됨

        public override void ResetDelayAfterSpawn(float delayDuration) { _delayDuration = delayDuration; }

        public override void Initialize(int id, int level, string name, float hp, OffsetFromCenter fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange, float lifeTime)
        {
            base.Initialize(id, level, name, hp, fillOffset, targetTag, damage, hitSpeed, range, captureRange);
            _lifeTime = lifeTime; // 라이프 타임 초기화
            _liveOutComponent.Initialize(_maxHp, _lifeTime, GetDamage);

            Dictionary<ActionState, BaseState> attackStates = new Dictionary<ActionState, BaseState>()
            {
                {ActionState.Ready, new ReadyState(this, _delayDuration)},
                {ActionState.Active, new ActiveState(this)}
            };

            InitializeFSM(attackStates, ActionState.Ready);
        }

        protected override void InitializeComponent()
        {
            _liveOutComponent = GetComponent<LiveOutComponent>();
            base.InitializeComponent();
        }
    }

    abstract public class LiveOutSpawnBuilding : Building
    {
        [SerializeField] Transform _spawnPoint;
        LiveOutComponent _liveOutComponent;

        protected float _delayDuration = 0; // 스폰 시 일정 시간 딜레이

        // Ready --> Active State로 전환됨

        public override void ResetDelayAfterSpawn(float delayDuration) { _delayDuration = delayDuration; }

        // 기본 능력치
        protected int _spawnUnitId; // 생성할 유닛 아이디
        protected float _spawnDelay; // 다음 스폰까지 딜레이
        protected Vector3[] _spawnOffsets; // 스폰 오프셋

        public override void Initialize(int id, int level, string name, float hp, OffsetFromCenter fillOffset, float lifeTime, int spawnUnitId, float spawnDelay, SerializableVector3[] spawnOffsets)
        {
            _id = id;
            _level = level;
            _name = name;

            _maxHp = hp; // 최대 체력 지정
            HP = hp;

            _fillOffset = fillOffset;

            _lifeTime = lifeTime;
            _spawnUnitId = spawnUnitId;

            _spawnDelay = spawnDelay;

            _spawnOffsets = new Vector3[spawnOffsets.Length];
            for (int i = 0; i < spawnOffsets.Length; i++) _spawnOffsets[i] = new Vector3(spawnOffsets[i]._x, spawnOffsets[i]._y, spawnOffsets[i]._z);

            InitializeComponent();
            _liveOutComponent.Initialize(_maxHp, _lifeTime, GetDamage);

            Dictionary<ActionState, BaseState> attackStates = new Dictionary<ActionState, BaseState>()
            {
                {ActionState.Ready, new ReadyState(this, _delayDuration)},
                {ActionState.Active, new ActiveState(this)}
            };

            InitializeFSM(attackStates, ActionState.Ready);
        }

        protected override void InitializeComponent()
        {
            _liveOutComponent = GetComponent<LiveOutComponent>();
            base.InitializeComponent();
        }

        protected override void InitializeBT()
        {
            // 딜레이, 스폰 --> 이 두 가지 기능 계속 반복시키기
            List<Node> _childNodes = new List<Node>()
            {
                new Sequence
                (
                    new List<Node>()
                    {
                        new Delay(_spawnDelay),
                        new Spawn(_spawnUnitId, PlayerId, _spawnPoint, _spawnOffsets, transform.rotation)
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }
    }

    abstract public class Building : EntityAI
    {
        protected BoxCollider _boxCollider;
        protected OffsetFromCenter _fillOffset = new OffsetFromCenter(1, 1, 1, 1);

        // 기본 능력치
        protected float _lifeTime; // 생존 시간
        //
        Action<Vector3, OffsetFromCenter> OnPlantRequested;
        Action<Vector3, OffsetFromCenter> OnReleaseRequested;

        void InitializeFillerAction()
        {
            GameObject grid = GameObject.FindWithTag("Grid");
            if (grid == null) return;

            FillComponent filler = grid.GetComponent<FillComponent>();
            if (filler == null) return;

            OnPlantRequested = filler.OnBuildingPlanted;
            OnReleaseRequested = filler.OnBuildingReleased;
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            _boxCollider = GetComponent<BoxCollider>();

            // 여기에서 GridFiller 참조해서 현재 스폰된 위치 기반으로 그리드 인덱스 찾아서 막아주기
            // 만약 타워가 터진다면 그리드 참조해서 다시 풀어주기
            InitializeFillerAction();
            OnPlantRequested(transform.position, _fillOffset);

            InitializeBT(); // 여기서 초기화 진행
        }

        private void OnDestroy()
        {
            OnReleaseRequested(transform.position, _fillOffset);
        }

        public override float ReturnColliderSize()
        {
            return (_boxCollider.size.x / 2) * (transform.localScale.x + transform.localScale.z) / 2;
        }
    }
}
