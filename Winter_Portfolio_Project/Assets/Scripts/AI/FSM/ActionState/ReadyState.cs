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

        EntityAI _entityAI;

        public ReadyState(EntityAI entityAI, float duration)
        {
            _entityAI = entityAI;
            _duration = duration;
            _timer = new Timer();
        }

        void GoToNextState()
        {
            _entityAI.FSM.SetState(EntityAI.ActionState.Active);
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
