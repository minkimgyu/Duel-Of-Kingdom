using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WPP.ClientInfo.Account;
using WPP.ClientInfo.Deck;
using WPP.ClientInfo.Tower;
using WPP.ClientInfo;
using Newtonsoft.Json;
using WPP.RoomInfo;
using System.Net;
using WPP.Battle;
using WPP.Battle.Example;
using WPP.FileReader;
using WPP.ClientInfo.CardData;
using System.IO;
using System;

namespace WPP.Network
{
    class PacketHandler
    {
        private static PacketHandler _instance;
        private delegate void HandleFunc(ref ByteBuffer buffer);
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
            packetHandler.Add((int)Server_PacketTagPackages.S_LOAD_CARD_COLLECTION, LoadCardCollection);
            packetHandler.Add((int)Server_PacketTagPackages.S_ACCEPT_REGISTER_ACCOUNT, HandleRegisterAcception);
            packetHandler.Add((int)Server_PacketTagPackages.S_REJECT_REGISTER_ACCOUNT, HandleRegisterRejection);
            packetHandler.Add((int)Server_PacketTagPackages.S_ACCEPT_LOGIN, HandleLoginAcception);
            packetHandler.Add((int)Server_PacketTagPackages.S_REJECT_LOGIN, HandleLoginRejection);
            packetHandler.Add((int)Server_PacketTagPackages.S_REQUEST_PLAY_GAME, HandleEnterGame);

            packetHandler.Add((int)Server_PacketTagPackages.S_ALERT_OVER_TIME, HandleOverTime);
            packetHandler.Add((int)Server_PacketTagPackages.S_REQUEST_END_GAME, HandleEndGame);

            packetHandler.Add((int)Peer_PacketTagPackages.TEST, TestConnection);
            packetHandler.Add((int)Peer_PacketTagPackages.DAMAGE_KT, DamageTower);
            packetHandler.Add((int)Peer_PacketTagPackages.DAMAGE_LPT, DamageTower);
            packetHandler.Add((int)Peer_PacketTagPackages.DAMAGE_RPT, DamageTower);
        }

        public void HandlePacket(byte[] packet)
        {
            if (packet.Length == 0)
                return;

            byte[] buffer = (byte[])packet.Clone();

            if (ClientTCP.Instance().buffer == null)
            {
                ClientTCP.Instance().buffer = new ByteBuffer();
            }

            ClientTCP.Instance().buffer.WriteBytes(packet);

            packetLength = ClientTCP.Instance().buffer.Count();

            if (packetLength <= 0)
            {
                ClientTCP.Instance().buffer.Clear();
                return;
            }

            if(ClientTCP.Instance().buffer.Count() > 4)
            {
                int dataLength = ClientTCP.Instance().buffer.ReadInteger(true);
                byte[] data = new byte[dataLength];
                int remainingDataLength = dataLength - 4;

                // 패킷이 분할되어 왔을 경우
                if (remainingDataLength >= 4096)
                {
                    byte[] fullPacket = ClientTCP.Instance().buffer.ReadBytes(ClientTCP.Instance().buffer.Count(), true);
                    Array.Copy(fullPacket, 0, data, 0, fullPacket.Length);
                    remainingDataLength -= fullPacket.Length;
                    while (remainingDataLength > 0)
                    {
                        fullPacket = ClientTCP.Instance().packetToHandle.Dequeue();
                        Array.Copy(fullPacket, 0, data, dataLength - remainingDataLength - 4, fullPacket.Length);
                        remainingDataLength -= fullPacket.Length;
                    }
                }
                else
                {
                    data = ClientTCP.Instance().buffer.ReadBytes(dataLength - 4, true);
                }
                HandleData(data);
            }
        }

        public void HandleData(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetTag = buffer.ReadInteger(true);

            if (packetHandler.TryGetValue(packetTag, out HandleFunc func))
            {
                func.Invoke(ref buffer);
            }
        }

        public void LoadCardCollection(ref ByteBuffer buffer)
        {
            string cardCollectionString = buffer.ReadString(true);
            string jsonFilePath = "Assets\\GameFile\\card_collection.json";
            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }
            File.WriteAllText(jsonFilePath, cardCollectionString);
        }
        public void HandleRegisterAcception(ref ByteBuffer buffer)
        {
            Debug.Log("register completed");
        }
        public void HandleRegisterRejection(ref ByteBuffer buffer)
        {
            Debug.Log("register failed");
        }
        public void HandleLoginAcception(ref ByteBuffer buffer)
        {
            string accountString = buffer.ReadString(true);
            ClientData.Instance().account = JsonConvert.DeserializeObject<ClientAccount>(accountString);
            string accountFilePath = "Assets\\GameFile\\account.json";
            if (File.Exists(accountFilePath))
            {
                File.Delete(accountFilePath);
            }
            File.WriteAllText(accountFilePath, accountString);

            string towersString = buffer.ReadString(true);
            ClientData.Instance().towers = JsonConvert.DeserializeObject<Towers>(towersString);
            string towersFilePath = "Assets\\GameFile\\towers.json";
            if (File.Exists(towersFilePath))
            {
                File.Delete(towersFilePath);
            }
            File.WriteAllText(towersFilePath, towersString);

            string decksString = buffer.ReadString(true);
            ClientData.Instance().decks = JsonConvert.DeserializeObject<Decks>(decksString);
            string decksFilePath = "Assets\\GameFile\\decks.json";
            if (File.Exists(decksFilePath))
            {
                File.Delete(decksFilePath);
            }
            File.WriteAllText(decksFilePath, decksString);

            SceneManager.LoadScene("Lobby");
            Debug.Log("login completed");
        }
        public void HandleLoginRejection(ref ByteBuffer buffer)
        {
            Debug.Log("login failed");
        }

        public void HandleEnterGame(ref ByteBuffer buffer)
        {
            int roomID = buffer.ReadInteger(true);
            Debug.Log("roomID: " + roomID);
            GameRoom.Instance().roomID = roomID;

            IPEndPoint opponentAddress = buffer.ReadEndPoint(true);
            Debug.Log("myAddress: " + ClientTCP.Instance().clntSock.Client.LocalEndPoint);
            Debug.Log("opponentAddress: " + opponentAddress);
            GameRoom.Instance().opponentAddress = opponentAddress;

            ClientTCP.Instance().ConnectPeer(opponentAddress);

            SceneManager.LoadScene("BattleSystemExample");
            Debug.Log("entered game");
        }

        public void HandleOverTime(ref ByteBuffer buffer)
        {
            BattleManager.Instance().StartOverTime();
            Debug.Log("Handle over time");
        }

        public void HandleEndGame(ref ByteBuffer buffer)
        {
            SceneManager.LoadScene("Lobby");
        }

        public void TestConnection(ref ByteBuffer buffer)
        {
            Debug.Log("test completed");
        }

        public void DamageTower(ref ByteBuffer buffer)
        {
            int towerID = buffer.ReadInteger(true);
            BattleUIExample.Instance().DamageTower(towerID);
        }
    }
}

