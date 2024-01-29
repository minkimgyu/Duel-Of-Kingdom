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

        // 여기서 Initialize 함수를 virtual로 여러 개 만들어서 하위 클래스에서 이를 활용할 수 있게끔 제작하기

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
            if (Application.isPlaying == false) return; // 플레이만 적용시키기

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ReturnColliderSize());
        }

        public abstract float ReturnColliderSize(); // 이거는 하위 클레스에서 구현

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


    // AI 적용을 위한 BT를 가지고 있음
    abstract public class EntityAI : Entity
    {
        protected Tree _bt;

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            _bt = new Tree();
            InitializeBT(); // 여기서 초기화 진행
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            _bt.OnUpdate();
        }

        protected abstract void InitializeBT();
    }
}