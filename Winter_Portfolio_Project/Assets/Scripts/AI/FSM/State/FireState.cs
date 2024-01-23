using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.TIMER;

namespace WPP.AI.ATTACK.STATE
{
    public class FireState : State
    {
        Timer _delayTimer;
        AttackComponent _attackComponent;
        float _applyDamageDelayDuration;
        float _fullDelayDuration;
        bool _nowFire;

        public FireState(AttackComponent attackComponent, float applyDamageDelayDuration, float fullDelayDuration)
        {
            _attackComponent = attackComponent;
            _delayTimer = new Timer();

            _applyDamageDelayDuration = applyDamageDelayDuration;
            _fullDelayDuration = fullDelayDuration;
        }

        void GoToOtherState(AttackState nextState)
        {
            _attackComponent.Fix = false; // 만약에 고정이 되었다면

            _nowFire = false;
            _delayTimer.Reset();
            _attackComponent.FSM.SetState(nextState);
        }

        public override void OnCancelAttackRequested()
        {
            _attackComponent.CancelAttackAnimation();
            GoToOtherState(AttackState.Ready);
        }

        public override void OnStateEnter()
        {
            _attackComponent.PlayAttackAnimation();
            _delayTimer.Start(_fullDelayDuration);
        }

        public override void OnStateUpdate() => _delayTimer.Update();

        public override void CheckStateChange()
        {
            if (_delayTimer.PassedTime >= _applyDamageDelayDuration && _nowFire == false)
            {
                Debug.Log("Attack");
                _attackComponent.DoAttackTask();

                _nowFire = true;
                _attackComponent.Fix = true;
            }


            if (_delayTimer.IsFinish == false) return;
            GoToOtherState(AttackState.AfterDelay);
        }
    }
}
