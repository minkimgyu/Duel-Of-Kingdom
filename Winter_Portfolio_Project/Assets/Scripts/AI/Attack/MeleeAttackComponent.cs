using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;
using WPP.AI.FSM;
using WPP.AI.ATTACK.STATE;

namespace WPP.AI.ATTACK
{
    public class MeleeAttackComponent : AttackComponent
    {
        public override void DoAttackTask() 
        {
            ApplyDamage(Target.ReturnDamagable(), Damage);
        }

        protected override void InitializeFSM()
        {
            Dictionary<AttackState, BaseState> attackStates = new Dictionary<AttackState, BaseState>();

            BaseState ready = new ReadyState(this);
            BaseState preDelay = new DelayState(this, 2, AttackState.Hit);
            BaseState hit = new HitState(this, 0.4f, 1f);
            BaseState afterDelay = new DelayState(this, 2);

            attackStates.Add(AttackState.Ready, ready);
            attackStates.Add(AttackState.PreDelay, preDelay);
            attackStates.Add(AttackState.Hit, hit);
            attackStates.Add(AttackState.AfterDelay, afterDelay);

            _fsm.Initialize(attackStates);
            _fsm.SetState(AttackState.Ready);
        }
    }
}