using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;

namespace WPP.AI.ACTION.STATE
{
    public class NeutralState : State
    {
        LifeAI _entityAI;

        public NeutralState(LifeAI entityAI)
        {
            _entityAI = entityAI;
        }

        public override void OnStateEnter()
        {
            // ü�� �� �����ֱ�
            _entityAI.OnVisibleChangeRequested?.Invoke(false);
            // ü�� ǥ�ô� �����ְԲ� �ϱ�
        }

        public override void OnActiveRequested()
        {
            // ü�� �� �����ֱ�
            _entityAI.OnVisibleChangeRequested?.Invoke(true);
            _entityAI.FSM.SetState(LifeAI.ActionState.Active);
        }
    }
}
