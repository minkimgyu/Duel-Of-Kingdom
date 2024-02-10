using System;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.SPAWNER;
using WPP.DeckManagement;
using WPP.ClientInfo;

namespace WPP.AI.GRID.STATE
{
    public class PlantState : State
    {
        GridController _gridController;
        Spawner _spawner;

        public PlantState(GridController gridController, Spawner spawner)
        {
            _gridController = gridController;
            _spawner = spawner;
        }

        void GoToReadyState()
        {
            _gridController.FSM.SetState(GridController.ControlState.Ready);
        }

        public override void OnMessageRequested(string info, Card card, int level, Vector3 pos)
        {
            Debug.Log(info);

            int ownershipId = ClientData.Instance().player_id_in_game;
            _spawner.Spawn(card, level, ownershipId, pos);
            GoToReadyState();
        }
    }
}
