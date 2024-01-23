using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WPP.AI.TARGET;
using Tree = BehaviorTree.Tree;

namespace WPP.AI
{
    abstract public class Entity : MonoBehaviour, IDamagable, ITarget
    {
        [SerializeField] float _maxHp;

        public float HP { get; set; }
        public bool IsDie { get; set; }

        HpContainer _hpContainer;

        Action<float> OnHpChange;

        protected virtual void Start()
        {
            HP = _maxHp;

            _hpContainer = GetComponentInChildren<HpContainer>();
            OnHpChange = _hpContainer.OnHpChangeRequested;
        }

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

        public abstract float ReturnColliderLength(); // �̰Ŵ� ���� Ŭ�������� ����

        public Vector3 ReturnPosition()
        {
            return transform.position;
        }

        public IDamagable ReturnDamagable()
        {
            return this;
        }
    }


    // AI ������ ���� BT�� ������ ����
    abstract public class EntityAI : Entity
    {
        protected Tree _bt;

        protected override void Start()
        {
            base.Start();
            _bt = new Tree();
            InitializeBT(); // ���⼭ �ʱ�ȭ ����
        }

        // Update is called once per frame
        void Update()
        {
            _bt.OnUpdate();
        }

        protected abstract void InitializeBT();
    }
}