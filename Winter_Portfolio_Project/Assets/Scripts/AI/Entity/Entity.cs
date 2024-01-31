using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WPP.AI.TARGET;
using WPP.AI.CAPTURE;
using WPP.GRID;
using WPP.AI.STAT;
using WPP.AI.UI;
using WPP.AI.FSM;
using Tree = BehaviorTree.Tree;

namespace WPP.AI
{
    abstract public class Entity : MonoBehaviour, IDamagable, ITarget
    {
        protected float _maxHp;

        [SerializeField] protected int _id;
        public int Id { get { return _id; } }

        protected int _level;
        protected string _name;


        public float HP { get; set; }
        public bool IsDie { get; set; }
        public int PlayerId { get; set; }

        Action<int, float, Transform> OnHpInitializeRequested;
        Action<float, float> OnHpChangeRequested;
        public Action<bool> OnVisibleChangeRequested;
        protected Action<bool> OnTxtVisibleRequested;
        Action OnHpDestroyRequested;

        public void AttachHpBar(HpContainerUI hpContainer)
        {
            OnHpInitializeRequested = hpContainer.Initialize;
            OnHpChangeRequested = hpContainer.OnHpChangeRequested;
            OnVisibleChangeRequested = hpContainer.OnVisibleChangeRequested;
            OnTxtVisibleRequested = hpContainer.OnTxtVisibleRequested;

            OnHpDestroyRequested = hpContainer.OnDestroyRequested;
        }

        protected virtual void InitializeComponent()
        {
            OnHpInitializeRequested(_level, HP, transform);
        }

        // ���⼭ Initialize �Լ��� virtual�� ���� �� ���� ���� Ŭ�������� �̸� Ȱ���� �� �ְԲ� �����ϱ�

        public virtual void ResetPlayerId(int playerId) { PlayerId = playerId; }
        public virtual void ResetDelayAfterSpawn(float delayDuration) { }

        public virtual void Initialize(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange) { }
        public virtual void Initialize(int id, int level, string name, float hp, OffsetFromCenter fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange) { }
        public virtual void Initialize(int id, int level, string name, float hp, OffsetFromCenter fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange, float lifeTime) { }
        public virtual void Initialize(int id, int level, string name, float hp, OffsetFromCenter fillOffset, float lifeTime, int spawnUnitId, float spawnDelay, SerializableVector3[] spawnOffsets) { }

        public virtual void GetDamage(float damage)
        {
            HP -= damage;
            OnHpChangeRequested?.Invoke(HP, _maxHp);

            if (HP <= 0)
            {
                HP = 0;
                Die();
            }
        }

        public virtual void Die()
        {
            if (IsDie == true) return;
            Destroy(gameObject);
            OnHpDestroyRequested?.Invoke();
        }

        private void OnDrawGizmos() => DrawGizmo();

        protected virtual void DrawGizmo()
        {
            if (Application.isPlaying == false) return; // �÷��̸� �����Ű��

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ReturnColliderSize());
        }

        public abstract float ReturnColliderSize(); // �̰Ŵ� ���� Ŭ�������� ����

        public Vector3 ReturnPosition()
        {
            return transform.position;
        }

        public IDamagable ReturnDamagable()
        {
            return this;
        }

        public string ReturnTag()
        {
            return gameObject.tag;
        }

        public string ReturnName()
        {
            return _name;
        }
    }


    // AI ������ ���� BT�� ������ ����
    abstract public class EntityAI : Entity
    {
        public enum ActionState
        {
            Ready,
            Neutral,
            Active
        }

        protected StateMachine<ActionState> _fsm;
        public StateMachine<ActionState> FSM { get { return _fsm; } }

        // ���⿡ FSM �־ Ready, Active ���� Ȯ�����ֱ�
        protected Tree _bt;
        public Tree BT { get { return _bt; } }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            _fsm = new StateMachine<ActionState>();

            _bt = new Tree();
            InitializeBT(); // ���⼭ �ʱ�ȭ ����
        }

        // �̰� ���� �������� �����������
        protected void InitializeFSM(Dictionary<ActionState, BaseState> states, ActionState startState) 
        { 
            _fsm.Initialize(states); 
            _fsm.SetState(startState); 
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);
            _fsm.OnDamage();
        }

        // Update is called once per frame
        protected virtual void Update() => _fsm.OnUpdate(); // ������Ʈ ����

        protected abstract void InitializeBT();
    }
}