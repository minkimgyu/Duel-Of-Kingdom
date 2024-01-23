using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.TIMER;

namespace WPP.AI.ATTACK.STATE
{
    public class HitState : State
    {
        Timer _delayTimer;
        AttackComponent _attackComponent;
        float _applyDamageDelayDuration;
        float _fullDelayDuration;
        bool _nowApplyDamage;

        public HitState(AttackComponent attackComponent, float applyDamageDelayDuration, float fullDelayDuration)
        {
            _attackComponent = attackComponent;
            _delayTimer = new Timer();

            _applyDamageDelayDuration = applyDamageDelayDuration;
            _fullDelayDuration = fullDelayDuration;
        }

        void GoToOtherState(AttackState nextState)
        {
            _attackComponent.Fix = false; // 만약에 고정이 되었다면

            _nowApplyDamage = false;
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
            if (_delayTimer.PassedTime >= _applyDamageDelayDuration && _nowApplyDamage == false)
            {
                Debug.Log("Attack");
                _attackComponent.DoAttackTask();

                _nowApplyDamage = true;
                _attackComponent.Fix = true;
            }

            if (_delayTimer.IsFinish == false) return;
            GoToOtherState(AttackState.AfterDelay);
        }
    }
}
