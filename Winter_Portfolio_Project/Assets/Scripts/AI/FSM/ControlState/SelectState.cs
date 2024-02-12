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

        // 여기서 타워가 파괴된 경우 이벤트로 받아서 채크해주기

        public override void OnStateUpdate()
        {
            _gridController.SelectComponent.SelectGrid(); // 작동시켜주기
        }

        public override void OnTowerConditionChangeRequested()
        {
            _gridController.FillComponent.EraseSpawnImpossibleRect();
            _gridController.FillComponent.DrawSpawnImpossibleRect();
            // 범위를 지웠다가 다시 그려줘야함
        }

        public override void OnMessageRequested(string info, OffsetRect offsetFromCenter)
        {
            Debug.Log(info);
            _gridController.SelectComponent.ResetRect(offsetFromCenter); // 여기서 그려줌
            _gridController.FillComponent.DrawSpawnImpossibleRect();
        }

        public override void OnMessageRequested(string info, float radius)
        {
            Debug.Log(info);
            _gridController.SelectComponent.ResetRect(radius); // 여기서 그려줌
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
