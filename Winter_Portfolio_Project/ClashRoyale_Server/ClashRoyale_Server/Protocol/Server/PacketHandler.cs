using System;
using System.Collections.Generic;
using System.Linq;
using WPP.ClashRoyale_Server.Logic.Room;
using WPP.ClashRoyale_Server.Data;
using WPP.ClashRoyale_Server.Data.ClientInfo.Tower;
using WPP.ClashRoyale_Server.Data.ClientInfo.Account;
using WPP.ClashRoyale_Server.Data.ClientInfo.Deck;
using Newtonsoft.Json;

namespace WPP.ClashRoyale_Server.Protocol.Server
{
    class PacketHandler
    {
        private static PacketHandler _instance = null;
        private delegate void HandleFunc(int clientID, ref ByteBuffer buffer);
        private Dictionary<int, HandleFunc> packetHandler;
        private int packetLength;

        public static PacketHandler Instance()
        {
            if (_instance == null)
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
            packetHandler.Add((int)Client_PacketTagPackages.C_REQUEST_END_GAME, EndGame);
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

            if (ServerTCP.Instance().clients[clientID].tcp.buffer.Count() > 4)
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

            if (DatabaseManager.Instance().CheckUsernameExists(username))
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
            DatabaseManager.Instance().AddTowers(username);
            DatabaseManager.Instance().AddDecks(username);
        }
        public void HandleLogin(int clientID, ref ByteBuffer buffer)
        {
            string username = buffer.ReadString(true);
            string password = buffer.ReadString(true);
            buffer.Dispose();

            if (DatabaseManager.Instance().CheckAccountExists(username, password))
            {
                ClientAccount account = DatabaseManager.Instance().LoadAccount(clientID, username);
                Towers towers = DatabaseManager.Instance().LoadTowers(clientID, username);
                Decks decks = DatabaseManager.Instance().LoadDecks(clientID, username);
                Console.WriteLine("[Player '" + username + "' ] logged in");

                ByteBuffer loginBuffer = new ByteBuffer();

                // 클라이언트 계정 정보 JSON화
                string accountString = JsonConvert.SerializeObject(account);
                loginBuffer.WriteString(accountString);

                // 클라이언트 타워 정보 JSON화
                string towersString = JsonConvert.SerializeObject(towers);
                loginBuffer.WriteString(towersString);

                // 클라이언트 덱 정보 JSON화
                string decksString = JsonConvert.SerializeObject(decks);
                loginBuffer.WriteString(decksString);

                ServerTCP.Instance().SendDataTo(Server_PacketTagPackages.S_ACCEPT_LOGIN, clientID, loginBuffer.ToArray());
                return;
            }
            ServerTCP.Instance().SendDataTo(Server_PacketTagPackages.S_REJECT_LOGIN, clientID);
            return;
        }

        public void HandleEnterRoom(int clientID, ref ByteBuffer buffer)
        {
            ServerTCP server = ServerTCP.Instance();
            server.clients[clientID].state = ClientState.IN_ROOM;
            server.clients[clientID].p2pAddress = buffer.ReadEndPoint(true);
            server.waitingClients.Add(server.clients[clientID]);
            server.waitingClients.OrderByDescending(p => p.accountInfo.trophy).ThenBy(p => p.towers.kingTower.level);
        }

        public void EndGame(int clientID, ref ByteBuffer buffer)
        {
            int roomID = buffer.ReadInteger(true);
            GameRoom room = GameRoomManager.Instance().FindGameRoom(roomID);
            room.EndGame();
            return;
        }
    }
}
