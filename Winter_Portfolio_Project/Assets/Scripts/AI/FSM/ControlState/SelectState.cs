using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;

namespace WPP.AI.GRID.STATE
{
    public class SelectState : State
    {
        GridController _gridController;

        public SelectState(GridController gridController)
        {
            _gridController = gridController;
        }

        public override void OnStateUpdate()
        {
            _gridController.SelectComponent.SelectGrid(); // 작동시켜주기
        }

        // 여기서 스폰 이후 딜레이, 스폰시킬 오브젝트 id, 스폰 오프셋 등등 스폰에 필요한 변수를 받는다
        // 
        public override void OnPlantRequested()
        {
            _gridController.FSM.SetState(GridController.ControlState.Plant);
        }
    }
}
