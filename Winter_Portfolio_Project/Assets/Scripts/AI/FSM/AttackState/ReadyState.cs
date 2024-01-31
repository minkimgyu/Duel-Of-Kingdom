using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.TIMER;
using WPP.AI.ATTACK;

namespace WPP.AI.ATTACK.STATE
{
    public class ReadyState : State
    {
        AttackComponent _attackComponent;

        public ReadyState(AttackComponent attackComponent)
        {
            _attackComponent = attackComponent;
        }

        public override void OnStateEnter()
        {
            //Debug.Log("ReadyState");
        }

        public override void OnAttackRequested()
        {
            _attackComponent.FSM.SetState(AttackState.PreDelay);
        }
    }
}