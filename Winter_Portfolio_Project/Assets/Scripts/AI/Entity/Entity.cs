using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WPP.AI.TARGET;
using WPP.AI.CAPTURE;
using WPP.AI.GRID;
using WPP.AI.STAT;
using WPP.AI.UI;
using WPP.AI.FSM;
using Tree = BehaviorTree.Tree;

namespace WPP.AI
{
    abstract public class Entity : MonoBehaviour
    {
        //[SerializeField] protected int _id;
        //public int Id { get { return _id; } }

        protected int _level;
        [SerializeField] protected string _name;
        public string Name { get { return _name; } }

        /// <summary>
        /// ��ü�� �����ϰ� �ִ� �÷��̾��� Id
        /// </summary>
        [SerializeField] protected int _ownershipId;
        public int OwnershipId { get { return _ownershipId; } }

        /// <summary>
        /// ���� Ŭ���̾�Ʈ�� �����ϴ� �÷��̾��� Id
        /// </summary>
        protected int _clientId;

        /// <summary>
        /// ���� ������ Entity�� ����ȭ��Ű�� ���ؼ� �ʿ��� Id
        /// </summary>
        protected string _networdId;
        public string NetwordId { get { return _networdId; } } // NetwordId ��ȯ


        protected bool IsMyEntity { get { return _ownershipId == _clientId; } } // �� ������ Entity�� ���

        /// <summary>
        /// ������ ����
        /// </summary>
        public void ResetId(int ownershipId, int clientId) { _ownershipId = ownershipId; _clientId = clientId; }

        /// <summary>
        /// �̰� ���
        /// </summary>
        public void ResetId(int ownershipId, int clientId, string networdId) { _ownershipId = ownershipId; _clientId = clientId; _networdId = networdId; }

        public virtual void ResetMagicStartPosition(Vector3 pos) { }

        public virtual void InitializeListRemover(Action<string> removeAction) { }
        abstract public bool CanAttachHpBar();
        public virtual void AttachHpBar(HpContainerUI hpContainer) { }
        protected virtual void InitializeComponent() { }
        public virtual void ResetDelayAfterSpawn(float delayDuration) { }
        public virtual void IsLeft(bool isLeft) { }

        public virtual void Initialize(int level, string name, float range, float durationBeforeDestroy, float damage, float speed) { }
        public virtual void Initialize(int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange) { }
        public virtual void Initialize(int level, string name, float hp, OffsetRect fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange) { }
        public virtual void Initialize(int level, string name, float hp, OffsetRect fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange, float lifeTime) { }
        public virtual void Initialize(int level, string name, float hp, OffsetRect fillOffset, float lifeTime, int spawnUnitId, float spawnDelay, SerializableVector3[] spawnOffsets) { }

        // ���⼭ Initialize �Լ��� virtual�� ���� �� ���� ���� Ŭ�������� �̸� Ȱ���� �� �ְԲ� �����ϱ�
        // ��ü�� ������Ų ���������� Id, ���� Ŭ���̾�Ʈ�� �����ϴ� �÷��̾��� Id
    }

    abstract public class Life : Entity, IDamagable, ITarget
    {
        protected float _maxHp;

        public float HP { get; set; }
        public bool IsDie { get; set; }

        Action<int, float, Transform> OnHpInitializeRequested;
        Action<float, float> OnHpChangeRequested;
        public Action<bool> OnVisibleChangeRequested;
        protected Action<bool> OnTxtVisibleRequested;
        Action OnHpDestroyRequested;

        Action<string> RemoveFromListInSpawnerRequested;
        public override void InitializeListRemover(Action<string> removeAction) { RemoveFromListInSpawnerRequested = removeAction; }

        public override bool CanAttachHpBar() { return true; }

        public override void AttachHpBar(HpContainerUI hpContainer)
        {
            OnHpInitializeRequested = hpContainer.Initialize;
            OnHpChangeRequested = hpContainer.OnHpChangeRequested;
            OnVisibleChangeRequested = hpContainer.OnVisibleChangeRequested;
            OnTxtVisibleRequested = hpContainer.OnTxtVisibleRequested;

            OnHpDestroyRequested = hpContainer.OnDestroyRequested;
        }

        protected override void InitializeComponent()
        {
            OnHpInitializeRequested(_level, HP, transform);
        }

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
            RemoveFromListInSpawnerRequested?.Invoke(NetwordId);
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

        public int ReturnOwnershipId()
        {
            return _ownershipId;
        }
    }

    // AI ������ ���� BT�� ������ ����
    abstract public class LifeAI : Life
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
            _fsm.OnActive();
        }

        // Update is called once per frame
        protected virtual void Update() => _fsm.OnUpdate(); // ������Ʈ ����

        protected abstract void InitializeBT();
    }
}