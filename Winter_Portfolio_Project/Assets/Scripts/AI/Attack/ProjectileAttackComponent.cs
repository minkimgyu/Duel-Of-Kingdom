using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WPP.AI.TARGET;
using WPP.AI.FSM;
using WPP.AI.ATTACK.STATE;

namespace WPP.AI.ATTACK
{
    public class ProjectileAttackComponent : AttackComponent
    {
        [SerializeField] Projectile _projectile;
        [SerializeField] Transform _spawnPoint;

        public override void DoAttackTask()
        {
            Projectile projectile = Instantiate(_projectile, _spawnPoint.position, Quaternion.identity);
            projectile.Initialize(Target, Damage, ApplyDamage);
        }

        protected override void InitializeFSM()
        {
            Dictionary<AttackState, BaseState> attackStates = new Dictionary<AttackState, BaseState>();

            BaseState ready = new ReadyState(this);
            BaseState preDelay = new DelayState(this, 2, AttackState.Fire);
            BaseState fire = new FireState(this, 0.05f, 0.2f);
            BaseState afterDelay = new DelayState(this, 2);

            attackStates.Add(AttackState.Ready, ready);
            attackStates.Add(AttackState.PreDelay, preDelay);
            attackStates.Add(AttackState.Fire, fire);
            attackStates.Add(AttackState.AfterDelay, afterDelay);

            _fsm.Initialize(attackStates);
            _fsm.SetState(AttackState.Ready);
        }
    }
}