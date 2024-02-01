using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;

namespace WPP.AI.GRID.STATE
{
    public class ReadyState : State
    {
        GridController _gridController;

        public ReadyState(GridController gridController)
        {
            _gridController = gridController;
        }

        public override void OnSelectRequested(OffsetRect offsetFromCenter)
        {
            _gridController.FSM.SetState(GridController.ControlState.Select, "SendRectOffset", offsetFromCenter);
        }
    }
}
