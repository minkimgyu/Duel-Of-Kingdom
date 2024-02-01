using System;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.SPAWNER;

namespace WPP.AI.GRID.STATE
{
    public class PlantState : State
    {
        GridController _gridController;

        Func<int[], int, int, Vector3, Vector3[], float, Quaternion, Entity[]> OnMultipleSpawnRequested;
        Func<int, int, int, Vector3, float, Quaternion, Entity> OnSingleSpawnRequested;

        public PlantState(GridController gridController, Func<int, int, int, Vector3, float, Quaternion, Entity> onSingleSpawn, Func<int[], int, int, Vector3, Vector3[], float, Quaternion, Entity[]> onMultipleSpawn)
        {
            _gridController = gridController;

            Spawner spawner = UnityEngine.Object.FindObjectOfType<Spawner>();
            if (spawner == null) return;

            OnSingleSpawnRequested = onSingleSpawn;
            OnMultipleSpawnRequested = onMultipleSpawn;
        }

        void GoToReadyState()
        {
            _gridController.FSM.SetState(GridController.ControlState.Ready);
        }

        Quaternion ReturnQuaternionUsingLandFormation(LandFormation landFormation)
        {
            if (LandFormation.C == landFormation) return Quaternion.Euler(new Vector3(0, 180, 0));
            else return Quaternion.identity;
        }

        public override void OnMessageRequested(string info, int entityId, int ownershipId, int clientId, Vector3 pos, float duration) 
        {
            Debug.Log(info);

            Quaternion quaternion = ReturnQuaternionUsingLandFormation(_gridController.LandFormation);
            OnSingleSpawnRequested?.Invoke(entityId, ownershipId, clientId, pos, duration, quaternion);
            GoToReadyState();
        }

        public override void OnMessageRequested(string info, int[] entityIds, int ownershipId, int clientId, Vector3 pos, Vector3[] offsets, float duration) 
        {
            Debug.Log(info);

            Quaternion quaternion = ReturnQuaternionUsingLandFormation(_gridController.LandFormation);
            OnMultipleSpawnRequested?.Invoke(entityIds, ownershipId, clientId, pos, offsets, duration, quaternion);
            GoToReadyState();
        }
    }
}
