using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WPP.AI.TARGET;
using WPP.AI.CAPTURE;
using Tree = BehaviorTree.Tree;

namespace WPP.AI
{
    abstract public class Entity : MonoBehaviour, IDamagable, ITarget
    {
        protected float _maxHp;

        protected int _id;
        protected int _level;
        protected string _name;

        public float HP { get; set; }
        public bool IsDie { get; set; }

        HpContainer _hpContainer;

        Action<float> OnHpChange;

        protected virtual void InitializeComponent()
        {
            _hpContainer = GetComponentInChildren<HpContainer>();
            OnHpChange = _hpContainer.OnHpChangeRequested;
        }

        // ���⼭ Initialize �Լ��� virtual�� ���� �� ���� ���� Ŭ�������� �̸� Ȱ���� �� �ְԲ� �����ϱ�

        public virtual void Initialize(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range) { }
        public virtual void Initialize(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float lifeTime) { }
        public virtual void Initialize(int id, int level, string name, float hp, float lifeTime, int spawnUnitId, int spawnUnitCount) { }

        public virtual void GetDamage(float damage)
        {
            HP -= damage;
            OnHpChange?.Invoke(HP / _maxHp);

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
    }


    // AI ������ ���� BT�� ������ ����
    abstract public class EntityAI : Entity
    {
        protected Tree _bt;

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            _bt = new Tree();
            InitializeBT(); // ���⼭ �ʱ�ȭ ����
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            _bt.OnUpdate();
        }

        protected abstract void InitializeBT();
    }
}