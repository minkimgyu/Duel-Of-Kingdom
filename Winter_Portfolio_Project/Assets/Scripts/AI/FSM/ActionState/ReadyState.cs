using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.TIMER;

namespace WPP.AI.ACTION.STATE
{
    public class ReadyState : State
    {
        Timer _timer;
        float _duration;

        LifeAI _entityAI;

        public ReadyState(LifeAI entityAI, float duration)
        {
            _entityAI = entityAI;
            _duration = duration;
            _timer = new Timer();
        }

        void GoToNextState()
        {
            _entityAI.FSM.SetState(LifeAI.ActionState.Active);
        }

        public override void OnStateEnter()
        {
            if (_duration == 0)
            {
                GoToNextState();
                return;
            }

            _timer.Start(_duration);
        }

        public override void OnStateUpdate()
        {
            _timer.Update();

            if (_timer.IsFinish == true)
            {
                GoToNextState();
                _timer.Reset();
            }
        }
    }
}
