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

            _maxHp = hp; // �ִ� ü�� ����
            HP = hp;
            _damage = damage;
            _hitSpeed = hitSpeed;
            _range = range;

            _fillOffset = fillOffset;

            InitializeComponent();

            _captureComponent.Initialize(targetTag, PlayerId, captureRange); // �̷� ������ ���� ������ �Ҵ����ش�.
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
                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, _range, _offsetDistance, _attackComponent), // DelayForAttack�� �־��ֱ�
                                        new LookAtTarget(_captureComponent, _viewComponent, _arm),

                                        new Attack(_attackComponent, _captureComponent)
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

    public class LiveOutAttackBuilding : AttackBuilding
    {
        LiveOutComponent _liveOutComponent;
        protected float _delayDuration = 0; // ���� �� ���� �ð� ������

        // Ready --> Active State�� ��ȯ��

        public override void ResetDelayAfterSpawn(float delayDuration) { _delayDuration = delayDuration; }

        public override void Initialize(int id, int level, string name, float hp, OffsetFromCenter fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange, float lifeTime)
        {
            base.Initialize(id, level, name, hp, fillOffset, targetTag, damage, hitSpeed, range, captureRange);
            _lifeTime = lifeTime; // ������ Ÿ�� �ʱ�ȭ
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

        protected float _delayDuration = 0; // ���� �� ���� �ð� ������

        // Ready --> Active State�� ��ȯ��

        public override void ResetDelayAfterSpawn(float delayDuration) { _delayDuration = delayDuration; }

        // �⺻ �ɷ�ġ
        protected int _spawnUnitId; // ������ ���� ���̵�
        protected float _spawnDelay; // ���� �������� ������
        protected Vector3[] _spawnOffsets; // ���� ������

        public override void Initialize(int id, int level, string name, float hp, OffsetFromCenter fillOffset, float lifeTime, int spawnUnitId, float spawnDelay, SerializableVector3[] spawnOffsets)
        {
            _id = id;
            _level = level;
            _name = name;

            _maxHp = hp; // �ִ� ü�� ����
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
            // ������, ���� --> �� �� ���� ��� ��� �ݺ���Ű��
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

        // �⺻ �ɷ�ġ
        protected float _lifeTime; // ���� �ð�
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

            // ���⿡�� GridFiller �����ؼ� ���� ������ ��ġ ������� �׸��� �ε��� ã�Ƽ� �����ֱ�
            // ���� Ÿ���� �����ٸ� �׸��� �����ؼ� �ٽ� Ǯ���ֱ�
            InitializeFillerAction();
            OnPlantRequested(transform.position, _fillOffset);

            InitializeBT(); // ���⼭ �ʱ�ȭ ����
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
