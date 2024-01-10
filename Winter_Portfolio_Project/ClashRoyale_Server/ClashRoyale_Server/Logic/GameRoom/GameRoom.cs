﻿using MySql.Data.MySqlClient.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server;
using WPP.ClashRoyale_Server.Protocol.Server;
using WPP.ClashRoyale_Server.Protocol;
using WPP.ClashRoyale_Server.Logic.Battle;

namespace ClashRoyale_Server.Logic.GameRoom
{
    public enum GameRoomStatus
    {
        ACTIVE = 1,
        INACTIVE
    }
    class GameRoom
    {
        public int id { get; set; }

        public List<ClientObject> clients { get; set; }
        public GameRoomStatus status { get; set; }
        public BattleLogic battle { get; set; }
        public GameRoom(int id) {
           this.id = id;
            clients = new List<ClientObject>();
            status = GameRoomStatus.ACTIVE;
            battle = new BattleLogic(this);
        }

        public bool CheckFull()
        {
            return clients.Count == 2;
        }

        public void AddClient(ClientObject client)
        {
            client.gameRoomID = id;
            if (clients.Count == 1)
                status = GameRoomStatus.ACTIVE;
            clients.Add(client);
        }

        public ClientObject FindClient(int clientID)
        {
            if (clients.Count < 2)
                return null;

            if (clients[0].tcp.id == clientID)
                return clients[0];
            return clients[1];
        }

        public void SendDataToAll(Server_PacketTagPackages tag, byte[] data = null)
        {
            ServerTCP.Instance().SendDataTo(tag, clients[0].tcp.id, data);
            ServerTCP.Instance().SendDataTo(tag, clients[1].tcp.id, data);
            return;
        }

        public void StartGame()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger(id);
            SendDataToAll(Server_PacketTagPackages.S_REQUEST_PLAY_GAME, buffer.ToArray());
            battle.StartBattle();
        }

        public void EndGame()
        {
            // update changed data to database

            status = GameRoomStatus.INACTIVE;
            SendDataToAll(Server_PacketTagPackages.S_REQUEST_END_GAME);
        }
    }
}
