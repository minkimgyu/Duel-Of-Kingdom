using ClashRoyale_Server.Logic.GameRoom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server.Database;
using WPP.ClashRoyale_Server.Database.ClientInfo.Tower;

namespace WPP.ClashRoyale_Server.Protocol.Server
{
    class PacketHandler
    {
        private static PacketHandler _instance = null;
        private delegate void HandleFunc(int clientID,ref ByteBuffer buffer);
        private Dictionary<int, HandleFunc> packetHandler;
        private int packetLength;

        public static PacketHandler Instance()
        {
            if(_instance == null)
            {
                _instance = new PacketHandler();
            }
            return _instance;
        }
        public void InitializePacketHandler()
        {
            packetHandler = new Dictionary<int, HandleFunc>();

            packetHandler.Add((int)Client_PacketTagPackages.C_REQUEST_REGISTER_ACCOUNT, HandleRegister);
            packetHandler.Add((int)Client_PacketTagPackages.C_REQUEST_LOGIN, HandleLogin);
            packetHandler.Add((int)Client_PacketTagPackages.C_REQUEST_ENTER_ROOM, HandleEnterRoom);

            packetHandler.Add((int)Client_PacketTagPackages.C_DESTROY_OPPONENT_KING_TOWER, HandleDestroyTower);
            packetHandler.Add((int)Client_PacketTagPackages.C_DESTROY_OPPONENT_LEFT_TOWER, HandleDestroyTower);
            packetHandler.Add((int)Client_PacketTagPackages.C_DESTROY_OPPONENT_RIGHT_TOWER, HandleDestroyTower);
        }

        public void HandlePacket(int clientID, byte[] packet)
        {
            if (packet.Length == 0)
                return;

            byte[] buffer = (byte[])packet.Clone();

            if (ServerTCP.Instance().clients[clientID].tcp.buffer == null)
            {
                ServerTCP.Instance().clients[clientID].tcp.buffer = new ByteBuffer();
            }

            ServerTCP.Instance().clients[clientID].tcp.buffer.WriteBytes(packet);

            packetLength = ServerTCP.Instance().clients[clientID].tcp.buffer.Count();

            if (packetLength <= 0)
            {
                ServerTCP.Instance().clients[clientID].tcp.buffer.Clear();
                return;
            }

            if(ServerTCP.Instance().clients[clientID].tcp.buffer.Count() > 4) 
            {
                int dataLength = ServerTCP.Instance().clients[clientID].tcp.buffer.ReadInteger(true);
                byte[] data = ServerTCP.Instance().clients[clientID].tcp.buffer.ReadBytes(dataLength - 4, true);
                HandleData(clientID, data);
                return;
            }
        }

        public void HandleData(int clientID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);

            int packetTag = buffer.ReadInteger(true);

            if (packetHandler.TryGetValue(packetTag, out HandleFunc packet))
            {
                Console.WriteLine("Handle " + (Client_PacketTagPackages)packetTag);

                packet.Invoke(clientID, ref buffer);
            }
            else
            {
                Console.WriteLine("Couldn't find tag num");
                return;
            }
        }

        public void HandleRegister(int clientID, ref ByteBuffer buffer)
        {
            string username = buffer.ReadString(true);
            
            if(DatabaseManager.Instance().CheckUsernameExists(username))
            {
                Console.WriteLine("There is already player who has same username '" + username + "'");
                ServerTCP.Instance().SendDataTo(Server_PacketTagPackages.S_REJECT_REGISTER_ACCOUNT, clientID);
                return;
            }

            string password = buffer.ReadString(true);
            Console.WriteLine("HandleRegister on");
            Console.WriteLine("username: " + username + " password: " + password);
            ServerTCP.Instance().SendDataTo(Server_PacketTagPackages.S_ACCEPT_REGISTER_ACCOUNT, clientID);
            DatabaseManager.Instance().AddAccount(username, password);
            DatabaseManager.Instance().AddTowers(username, new Towers());
        }
        public void HandleLogin(int clientID, ref ByteBuffer buffer)
        {
            string username = buffer.ReadString(true);
            string password = buffer.ReadString(true);

            if (DatabaseManager.Instance().HandleLogin(username, password))
            {
                DatabaseManager.Instance().UpdateAccountFromDatabase(clientID, username);
                DatabaseManager.Instance().UpdateTowersFromDatabase(clientID, username);
                Console.WriteLine("HandleLogin on");
                Console.WriteLine("username: " + username + " password: " + password);
                ServerTCP.Instance().SendDataTo(Server_PacketTagPackages.S_ACCEPT_LOGIN, clientID);
                return;
            }
            ServerTCP.Instance().SendDataTo(Server_PacketTagPackages.S_REJECT_LOGIN, clientID);
            return;
        }

        public void HandleEnterRoom(int clientID, ref ByteBuffer buffer)
        {
            ServerTCP server = ServerTCP.Instance();
            server.clients[clientID].state = ClientState.IN_ROOM;
            server.waitingClients.Add(ServerTCP.Instance().clients[clientID]);
            server.waitingClients.OrderByDescending(p => p.accountInfo.trophy).ThenBy(p => p.towers.kingTower.level);
        }

        public void HandleDestroyTower(int clientID, ref ByteBuffer buffer)
        {
            int roomID = buffer.ReadInteger(true);
            int towerIndex = buffer.ReadInteger(true);
            GameRoom gameRoom = GameRoomManager.Instance().FindGameRoom(roomID);

            switch (towerIndex)
            {
                case 0:
                    gameRoom.battle.DestroyKingTower(clientID);
                    break;
                case 1:
                    gameRoom.battle.DestroyPrincessTower(clientID, true);
                    break;
                case 2:
                    gameRoom.battle.DestroyPrincessTower(clientID, false);
                    break;
            }
        }
    }
}
