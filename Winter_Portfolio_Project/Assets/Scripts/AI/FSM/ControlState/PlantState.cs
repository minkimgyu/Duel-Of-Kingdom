using System;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.FSM;
using WPP.AI.SPAWNER;
using WPP.DeckManagement;
using WPP.ClientInfo.Card;
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

            //int ownershipId = ClientData.Instance().player_id_in_game;
            int ownershipId = 1; // 서버 연동 전까지 일단 1로 적용
            CardData cardData = CardDatabase.GetCardData(card, level);
            List<EntitySpawnData> entitySpawnDatas = card.entities;

            _spawner.Spawn(card, level, ownershipId, pos);
            GoToReadyState();
        }
    }
}
