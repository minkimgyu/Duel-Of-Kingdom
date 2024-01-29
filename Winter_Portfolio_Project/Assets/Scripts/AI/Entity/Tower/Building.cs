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
    // �⺻ ����� InitializeBT ���⿡�� �����ϰ�
    // �߰��� ������ ����� ������ ���� Ŭ������ ���� �߰� �����ؼ� �����Ű��

    abstract public class AttackBuilding : Building
    {
        // �⺻ �ɷ�ġ
        protected float _damage; // ������
        protected float _hitSpeed; // ���� �ӵ�
        protected float _range; // ����
        //

        protected float _offsetDistance = 0.2f; // ���� �Ÿ� ������

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

            _maxHp = hp; // �ִ� ü�� ����
            HP = hp;
            _damage = damage;
            _hitSpeed = hitSpeed;
            _range = range;

            InitializeComponent();

            _captureComponent.Initialize(targetTag); // �̷� ������ ���� ������ �Ҵ����ش�.
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
                                new CanFindTarget(_captureComponent), // ���� Ÿ���� ���ٸ� Ÿ���� Ÿ������ �������ش�.
                                new Sequence
                                (
                                    new List<Node>
                                    {
                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, _range, _offsetDistance, _projectileAttackComponent), // DelayForAttack�� �־��ֱ�
                                        new LookAtTarget(_captureComponent, _viewComponent, _arm),

                                        new Attack(_projectileAttackComponent, _captureComponent)
                                        // ���� ����
                                        // ���� ������ ���� ���� ��� �Ÿ��� �־����� ��� ����
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
        // �⺻ �ɷ�ġ
        protected float _spawnUnitId; // ������ ���� ���̵�
        protected float _spawnUnitCount; // ������ ���� ����
        //

        public override void Initialize(int id, int level, string name, float hp, float lifeTime, int spawnUnitId, int spawnUnitCount)
        {
            _id = id;
            _level = level;
            _name = name;

            _maxHp = hp; // �ִ� ü�� ����
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

        // �⺻ �ɷ�ġ
        protected float _lifeTime; // ���� �ð�
        //

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            _boxCollider = GetComponent<BoxCollider>();

            InitializeBT(); // ���⼭ �ʱ�ȭ ����
        }

        public override float ReturnColliderSize()
        {
            return (_boxCollider.size.x / 2) * (transform.localScale.x + transform.localScale.z) / 2;
        }
    }
}
