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

        // ���⼭ Ÿ���� �ı��� ��� �̺�Ʈ�� �޾Ƽ� äũ���ֱ�

        public override void OnStateUpdate()
        {
            _gridController.SelectComponent.SelectGrid(); // �۵������ֱ�
        }

        public override void OnTowerConditionChangeRequested()
        {
            _gridController.FillComponent.EraseSpawnImpossibleRect();
            _gridController.FillComponent.DrawSpawnImpossibleRect();
            // ������ �����ٰ� �ٽ� �׷������
        }

        public override void OnMessageRequested(string info, OffsetRect offsetFromCenter)
        {
            Debug.Log(info);
            _gridController.SelectComponent.ResetRect(offsetFromCenter); // ���⼭ �׷���
            _gridController.FillComponent.DrawSpawnImpossibleRect();
        }

        public override void OnStateExit()
        {
            _gridController.SelectComponent.EraseArea();
            _gridController.FillComponent.EraseSpawnImpossibleRect();
        }

        public override void OnCancelSelectRequested() => _gridController.FSM.SetState(GridController.ControlState.Ready); 

        // ���⼭ ���� ���� ������, ������ų ������Ʈ id, ���� ������ ��� ������ �ʿ��� ������ �޴´�
        public override void OnPlantRequested(int entityId, int ownershipId, int clientId, float duration)
        {
            Vector3 pos = _gridController.SelectComponent.ReturnSpawnPoint();
            _gridController.FSM.SetState(GridController.ControlState.Plant, "GoToPlant", entityId, ownershipId, clientId, pos, duration);
        }

        public override void OnPlantRequested(int[] entityIds, int ownershipId, int clientId, Vector3[] offsets, float duration)
        {
            Vector3 pos = _gridController.SelectComponent.ReturnSpawnPoint();
            _gridController.FSM.SetState(GridController.ControlState.Plant, "GoToPlant", entityIds, ownershipId, clientId, pos, offsets, duration);
        }
    }
}
