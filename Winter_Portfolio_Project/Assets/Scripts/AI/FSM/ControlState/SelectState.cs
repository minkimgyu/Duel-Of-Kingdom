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
            _gridController.SelectComponent.SelectGrid(); // �۵������ֱ�
        }

        // ���⼭ ���� ���� ������, ������ų ������Ʈ id, ���� ������ ��� ������ �ʿ��� ������ �޴´�
        // 
        public override void OnPlantRequested()
        {
            _gridController.FSM.SetState(GridController.ControlState.Plant);
        }
    }
}
