using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.DeckManagement;

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

        public override void OnMessageRequested(string info, float radius)
        {
            Debug.Log(info);
            _gridController.SelectComponent.ResetRect(radius); // ���⼭ �׷���
            _gridController.FillComponent.DrawSpawnImpossibleRect();
        }

        public override void OnStateExit()
        {
            _gridController.SelectComponent.EraseArea();
            _gridController.FillComponent.EraseSpawnImpossibleRect();
        }

        public override void OnCancelSelectRequested() => _gridController.FSM.SetState(GridController.ControlState.Ready);

        public override void OnPlantRequested(Card card, int level)
        {
            Vector3 pos = _gridController.SelectComponent.ReturnSpawnPoint();
            _gridController.FSM.SetState(GridController.ControlState.Plant, "GoToPlant", card, level, pos);
        }
    }
}
