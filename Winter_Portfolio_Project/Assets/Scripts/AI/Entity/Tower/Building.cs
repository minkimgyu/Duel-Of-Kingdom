using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI;
using WPP.AI.ATTACK;
using WPP.AI.CAPTURE;
using WPP.AI.BTUtility;

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
        protected ProjectileAttackComponent _projectileAttackComponent;

        protected override void InitializeComponent()
        {
            _captureComponent = GetComponentInChildren<CaptureComponent>();
            _viewComponent = GetComponentInChildren<ViewComponent>();
            _projectileAttackComponent = GetComponent<ProjectileAttackComponent>();
            base.InitializeComponent();
        }

        public override void Initialize(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range)
        {
            _id = id;
            _level = level;
            _name = name;

            _maxHp = hp; // 최대 체력 지정
            HP = hp;
            _damage = damage;
            _hitSpeed = hitSpeed;
            _range = range;

            InitializeComponent();

            _captureComponent.Initialize(targetTag); // 이런 식으로 세부 변수를 할당해준다.
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
                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, _range, _offsetDistance, _projectileAttackComponent), // DelayForAttack도 넣어주기
                                        new LookAtTarget(_captureComponent, _viewComponent, _arm),

                                        new Attack(_projectileAttackComponent, _captureComponent)
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

    abstract public class SpawnBuilding : Building
    {
        // 기본 능력치
        protected float _spawnUnitId; // 생성할 유닛 아이디
        protected float _spawnUnitCount; // 생성할 유닛 개수
        //

        public override void Initialize(int id, int level, string name, float hp, float lifeTime, int spawnUnitId, int spawnUnitCount)
        {
            _id = id;
            _level = level;
            _name = name;

            _maxHp = hp; // 최대 체력 지정
            HP = hp;
            _lifeTime = lifeTime;
            _spawnUnitId = spawnUnitId;
            _spawnUnitCount = spawnUnitCount;
        }

        protected override void InitializeBT()
        {

        }
    }

    abstract public class Building : EntityAI
    {
        protected BoxCollider _boxCollider;

        // 기본 능력치
        protected float _lifeTime; // 생존 시간
        //

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            _boxCollider = GetComponent<BoxCollider>();

            InitializeBT(); // 여기서 초기화 진행
        }

        public override float ReturnColliderSize()
        {
            return (_boxCollider.size.x / 2) * (transform.localScale.x + transform.localScale.z) / 2;
        }
    }
}
