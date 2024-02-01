using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;

namespace WPP.AI.ACTION.STATE
{
    public class NeutralState : State
    {
        EntityAI _entityAI;

        public NeutralState(EntityAI entityAI)
        {
            _entityAI = entityAI;
        }

        public override void OnStateEnter()
        {
            // 체력 바 숨겨주기
            _entityAI.OnVisibleChangeRequested?.Invoke(false);
            // 체력 표시는 보여주게끔 하기
        }

        public override void OnActiveRequested()
        {
            // 체력 바 보여주기
            _entityAI.OnVisibleChangeRequested?.Invoke(true);
            _entityAI.FSM.SetState(EntityAI.ActionState.Active);
        }
    }
}
