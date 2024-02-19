using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;

namespace WPP.AI.ACTION.STATE
{
    public class ActiveState : State
    {
        LifeAI _entityAI;

        public ActiveState(LifeAI entityAI)
        {
            _entityAI = entityAI;
        }

        public override void OnStateUpdate()
        {
            _entityAI.BT.OnUpdate(); // 여기서 BT를 작동시켜준다.
        }
    }
}
