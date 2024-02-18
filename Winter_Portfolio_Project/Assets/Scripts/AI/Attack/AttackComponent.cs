using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WPP.AI.TARGET;
using WPP.AI.FSM;
using WPP.SOUND;

namespace WPP.AI.ATTACK
{
    public enum AttackState
    {
        Ready,
        PreDelay,
        AfterDelay,

        Hit,
        Fire,
    }

    abstract public class AttackComponent : MonoBehaviour
    {
        protected StateMachine<AttackState> _fsm;
        public StateMachine<AttackState> FSM { get { return _fsm; } }

        protected Animator _animator;

        protected ITarget _target;
        public ITarget Target { get { return _target; } }

        protected int _ownershipId;

        protected float _attackDelay;

        protected float _damage;
        public float Damage { get { return _damage; } }

        protected bool _fix; // 공격이 나간 이후 대상을 고정시킨다.
        public bool Fix { get { return _fix; } set { _fix = value; } }

        [SerializeField] protected float _delayBeforeApplyingTask = 0.2f; // 이거는 나중에 데이터베이스에 추가하던가 해야할 듯?
        [SerializeField] string _attackSoundName;

        public void CancelAttackAnimation() => _animator.SetTrigger("CancelAttack");
        public void PlayAttackAnimation() => _animator.SetTrigger("NowAttack");

        public void Initialize(float damage, float attackDelay, int ownershipId)
        {
            _damage = damage;
            _attackDelay = attackDelay;
            _ownershipId = ownershipId;

            _animator = GetComponent<Animator>();
            _fsm = new StateMachine<AttackState>();
            InitializeFSM();
        }

        protected abstract void InitializeFSM();

        // 내부에서 getcomponent 사용해서 공격 적용
        public virtual void Attack(ITarget target) 
        {
            _target = target;
            //_damage = damage;
            _fsm.OnAttack(); // 공격 적용
        }

        private void Update()
        {
            _fsm.OnUpdate(); // 업데이트 적용
        }

        public void CancelAttack()
        {
            _fsm.OnCancelAttack();
        }

        protected void ApplyDamage(IDamagable damagable, float damage) => damagable.GetDamage(damage);

        public virtual void DoAttackTask() 
        {
            SoundManager.PlaySFX(_attackSoundName);
        }
    }
}