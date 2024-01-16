using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server.Protocol;
using WPP.ClashRoyale_Server.Database.ClientInfo.Tower;
using WPP.ClashRoyale_Server.Logic.Room;

namespace WPP.ClashRoyale_Server.Logic.Battle
{
    class BattleLogic
    {
        private readonly GameRoom _gameRoom;
        private readonly BattleTimer _timer;
        public ClientObject winner { get; set; }
        public ClientObject loser { get; set; }
        public BattleLogic(GameRoom room) {
            _gameRoom = room;
            _timer = new BattleTimer();
            _timer.InitializeTimers(this);
        }

        public void StartBattle()
        {
            _timer.StartTimer();
        }

        public void DestroyKingTower(int clientID)
        {
            ClientObject client = _gameRoom.FindClient(clientID);
            client.towers.kingTower.status = TowerStatus.DESTROYED;
            _gameRoom.EndGame();
        }
        public void DestroyPrincessTower(int clientID, bool isLeft) 
        {
            ClientObject client = _gameRoom.FindClient(clientID);
            if(isLeft) // direction 0 is left
            {
                client.towers.leftPrincessTower.status = TowerStatus.DESTROYED;
            }
            else
            {
                client.towers.rightPrincessTower.status = TowerStatus.DESTROYED;
            }
            return;
        }
        public void SetDoubleElixirMode()
        {
            _gameRoom.SendDataToAll(Server_PacketTagPackages.S_ALERT_DOUBLE_ELIXIR_TIME);
        }

        public void SetSuddenDeathMode()
        {
            _gameRoom.SendDataToAll(Server_PacketTagPackages.S_ALERT_SUDDEN_DEATH_TIME);
        }

        public void EndBattle()
        {
            _timer.StopTimer();
            DetermineGameResult();
            CalculateScore();
        }

        public void DetermineGameResult()
        {
            if (_gameRoom.clients[0].towers.kingTower.status == TowerStatus.DESTROYED)
            {
                loser = _gameRoom.clients[0];
                winner = _gameRoom.clients[1];
                return;
            }

            if (_gameRoom.clients[0].towers.numOfTowersDestroyed > _gameRoom.clients[1].towers.numOfTowersDestroyed)
            {
                loser = _gameRoom.clients[0];
                winner = _gameRoom.clients[1];
            }
            else
            {
                loser = _gameRoom.clients[1];
                winner = _gameRoom.clients[0];
            }
            return;
        }

        public void CalculateScore()
        {
            // https://blog.naver.com/wldn154888/221197981397

            int score = (winner.accountInfo.trophy - loser.accountInfo.trophy) / 12 + 30;
            winner.accountInfo.trophy += score;
            loser.accountInfo.trophy -= score;
            return;
        }
    }
}
