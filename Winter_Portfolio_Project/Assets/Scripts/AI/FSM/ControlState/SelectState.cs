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

        public override void OnStateExit()
        {
            _gridController.SelectComponent.EraseArea();
            _gridController.FillComponent.EraseSpawnImpossibleRect();
        }

        public override void OnCancelSelectRequested() => _gridController.FSM.SetState(GridController.ControlState.Ready); 

        // 여기서 스폰 이후 딜레이, 스폰시킬 오브젝트 id, 스폰 오프셋 등등 스폰에 필요한 변수를 받는다
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
