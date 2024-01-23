using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.TIMER;

namespace WPP.AI.ATTACK.STATE
{
    public class DelayState : State
    {
        Timer _delayTimer;
        AttackComponent _attackComponent;
        float _preDelayDuration;

        AttackState _nextState;

        public DelayState(AttackComponent attackComponent, float preDelayDuration)
        {
            _attackComponent = attackComponent;
            _delayTimer = new Timer();

            _preDelayDuration = preDelayDuration;
            _nextState = AttackState.Ready;
        }

        public DelayState(AttackComponent attackComponent, float preDelayDuration, AttackState nextState)
        {
            _attackComponent = attackComponent;
            _delayTimer = new Timer();

            _preDelayDuration = preDelayDuration;
            _nextState = nextState;
        }

        void GoToOtherState(AttackState nextState)
        {
            _delayTimer.Reset();
            _attackComponent.FSM.SetState(nextState);
        }

        public override void OnCancelAttackRequested()
        {
            //_attackComponent.CancelAttackAnimation();
            GoToOtherState(AttackState.Ready);
        }

        public override void CheckStateChange()
        {
            if (_delayTimer.IsFinish == false) return;

            GoToOtherState(_nextState);
        }

        public override void OnStateEnter()
        {
            Debug.Log("DelayState");
            _delayTimer.Start(_preDelayDuration);
        }

        public override void OnStateUpdate() => _delayTimer.Update();
    }
}
