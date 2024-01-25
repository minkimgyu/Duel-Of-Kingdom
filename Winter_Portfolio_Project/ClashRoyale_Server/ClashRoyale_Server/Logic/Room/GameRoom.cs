using MySql.Data.MySqlClient.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server;
using WPP.ClashRoyale_Server.Protocol.Server;
using WPP.ClashRoyale_Server.Protocol;
using WPP.ClashRoyale_Server.Logic.Battle;
using System.Net;

namespace WPP.ClashRoyale_Server.Logic.Room
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
            // send roomID and opponent's ip address
            ByteBuffer buffer1 = new ByteBuffer();
            ByteBuffer buffer2 = new ByteBuffer();

            IPEndPoint ep = clients[1].p2pAddress;
            buffer1.WriteInteger(id);
            if (ep != null)
                buffer1.WriteEndPoint(ep);
            ServerTCP.Instance().SendDataTo(Server_PacketTagPackages.S_REQUEST_PLAY_GAME, clients[0].tcp.id, buffer1.ToArray());

            ep = clients[0].p2pAddress;
            buffer2.WriteInteger(id);
            if (ep != null)
                buffer2.WriteEndPoint(ep);
            ServerTCP.Instance().SendDataTo(Server_PacketTagPackages.S_REQUEST_PLAY_GAME, clients[1].tcp.id, buffer2.ToArray());

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
