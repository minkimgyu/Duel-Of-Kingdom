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
using System.IO;
using System;
using WPP.FileReader;
using Unity.VisualScripting;
using WPP.ClientInfo.Card;

namespace WPP.Network
{
    class PacketHandler
    {
        private static PacketHandler _instance;
        private delegate void HandleFunc(ref ByteBuffer buffer);
        private Dictionary<int, HandleFunc> packetHandler;
        public Queue<byte[]> packetQueue { get; set; }
        public Queue<ByteBuffer> inGamePacketQueue { get; set; }

        private int _packetLength;
        private bool _isSegmentated;

        public static PacketHandler Instance()
        {
            if (_instance == null)
            {
                _instance = new PacketHandler();
            }
            return _instance;
        }

        public PacketHandler() {
            _packetLength = 0;
            _isSegmentated = false;
            packetQueue = new Queue<byte[]>();
            inGamePacketQueue = new Queue<ByteBuffer>();
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

            packetHandler.Add((int)Server_PacketTagPackages.HOLE_PUNCHING, HandleHolePunching);
            packetHandler.Add((int)Server_PacketTagPackages.TURN_ON, HandleTurnOn);

            packetHandler.Add((int)Peer_PacketTagPackages.DAMAGE_KT, DamageTower);
            packetHandler.Add((int)Peer_PacketTagPackages.DAMAGE_LPT, DamageTower);
            packetHandler.Add((int)Peer_PacketTagPackages.DAMAGE_RPT, DamageTower);
            packetHandler.Add((int)Peer_PacketTagPackages.SPAWN_CARD, SpawnCard);
        }

        public void HandlePacket(byte[] packet)
        {
            if (packet == null || packet.Length == 0)
                return;

            if (ClientTCP.Instance().buffer == null)
            {
                ClientTCP.Instance().buffer = new ByteBuffer();
            }

            // packet 복사
            ClientTCP.Instance().buffer.WriteBytes(packet);

            if (_isSegmentated == false)
            {
                _packetLength = ClientTCP.Instance().buffer.ReadInteger(true);
                Debug.Log("total packet length: " + _packetLength);
            }
            Debug.Log("received packet length: " + packet.Length);

            // 처음으로 패킷이 분할되어 왔을 경우
            if (_packetLength > ClientTCP.Instance().buffer.Count() + 4 && _isSegmentated == false)
            {
                _isSegmentated = true;
                return;
            }

            if (_isSegmentated == true)
            {
                // 패킷을 다 받았다면
                if (_packetLength == ClientTCP.Instance().buffer.Count() + 4)
                {
                    _isSegmentated = false;
                    HandleData(ClientTCP.Instance().buffer.ToArray());
                    _packetLength = 0;
                    ClientTCP.Instance().buffer.Dispose();
                    ClientTCP.Instance().buffer = null;
                    Debug.Log("received all pakcts");
                }
            }
            else
            {
                HandleData(ClientTCP.Instance().buffer.ToArray());
                _packetLength = 0;
                ClientTCP.Instance().buffer.Dispose();
                ClientTCP.Instance().buffer = null;
            }
            return;
        }

        public void HandleData(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            // skip size
            buffer.ReadInteger(true);
            int packetTag = buffer.ReadInteger(true);

            if (packetHandler.TryGetValue(packetTag, out HandleFunc func))
            {
                func.Invoke(ref buffer);
            }
        }

        public void LoadCardCollection(ref ByteBuffer buffer)
        {
            string cardCollectionString = buffer.ReadString(true);
#if DEBUG
            string jsonFilePath = "Assets\\GameFiles\\card_collection.json";
#else
            string jsonFilePath = Application.persistentDataPath + "/card_collection.json";
#endif
            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }
            File.WriteAllText(jsonFilePath, cardCollectionString);
            JsonParser.Instance().LoadCardCollection();

            ClientTCP.Instance().ConnectServerForHolePunching();
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
            Debug.Log("Handle login");
            string accountString = buffer.ReadString(true);
            ClientData.Instance().account = JsonConvert.DeserializeObject<AccountData>(accountString);
#if UNITY_EDITOR
            string accountFilePath = "Assets\\GameFiles\\account.json";
#else
            string accountFilePath = Application.persistentDataPath + "/account.json";
#endif
            if (File.Exists(accountFilePath))
            {
                File.Delete(accountFilePath);
            }
            File.WriteAllText(accountFilePath, accountString);
            JsonParser.Instance().LoadAccount();

            string towersString = buffer.ReadString(true);
            ClientData.Instance().towers = JsonConvert.DeserializeObject<TowersData>(towersString);
#if UNITY_EDITOR
            string towersFilePath = "Assets\\GameFiles\\towers.json";
#else
            string towersFilePath = Application.persistentDataPath + "/towers.json";
#endif
            if (File.Exists(towersFilePath))
            {
                File.Delete(towersFilePath);
            }
            File.WriteAllText(towersFilePath, towersString);
            JsonParser.Instance().LoadTowers();

            string decksString = buffer.ReadString(true);
            ClientData.Instance().decks = JsonConvert.DeserializeObject<DecksData>(decksString);
#if UNITY_EDITOR
            string decksFilePath = "Assets\\GameFiles\\decks.json";
#else
            string decksFilePath = Application.persistentDataPath + "/decks.json";
#endif
            if (File.Exists(decksFilePath))
            {
                File.Delete(decksFilePath);
            }
            File.WriteAllText(decksFilePath, decksString);
            JsonParser.Instance().LoadDecks();

            string cardInstancesString = buffer.ReadString(true);
            ClientData.Instance().cards = JsonConvert.DeserializeObject<CardsData>(cardInstancesString);
#if UNITY_EDITOR
            string cardInstancessFilePath = "Assets\\GameFiles\\cards.json";
#else
            string cardInstancessFilePath = Application.persistentDataPath + "/cards.json";
#endif
            if (File.Exists(cardInstancessFilePath))
            {
                File.Delete(cardInstancessFilePath);
            }
            File.WriteAllText(cardInstancessFilePath, cardInstancesString);
            JsonParser.Instance().LoadCardInstances();

            SceneManager.LoadScene("HomeUIScene");
        }

        public void HandleLoginRejection(ref ByteBuffer buffer)
        {
            Debug.Log("login failed");
        }

        public void HandleEnterGame(ref ByteBuffer buffer)
        {
            // set room_id
            int roomID = buffer.ReadInteger(true);
            Debug.Log("roomID: " + roomID);
            GameRoom.Instance().roomID = roomID;

            // set opponent player's IP address
            IPEndPoint opponentPrivateEP = buffer.ReadEndPoint(true);
            IPEndPoint opponentPublicEP = buffer.ReadEndPoint(true);
            Debug.Log("my private ep: " + ClientTCP.Instance().peerSockPrivateEP);
            Debug.Log("my public ep: " + ClientTCP.Instance().peerSockPublicEP);
            Debug.Log("opponent's private ep: " + opponentPrivateEP);
            Debug.Log("opponent's public ep: " + opponentPublicEP);
            GameRoom.Instance().opponentPrivateEP = opponentPrivateEP;
            GameRoom.Instance().opponentPublicEP = opponentPublicEP;

            // set player_id
            int player_id_in_game = buffer.ReadInteger(true);
            ClientData.Instance().player_id_in_game = player_id_in_game;

            // try to connect with private end point
            ClientTCP.Instance().ConnectPeer(opponentPrivateEP);
            if (ClientTCP.Instance().peerSock == null || ClientTCP.Instance().peerSock.Connected == false)
            {
                // try to connect with public end point
                ClientTCP.Instance().ConnectPeer(opponentPublicEP);
            }

            SceneManager.LoadScene("EnableNetworkBattleScene");
            Debug.Log("entered game");
        }

        public void HandleOverTime(ref ByteBuffer buffer)
        {
            BattleManager.Instance().StartOverTime();
            Debug.Log("Handle over time");
        }

        public void HandleEndGame(ref ByteBuffer buffer)
        {
            ClientData.Instance().player_id_in_game = -1;
            ClientTCP.Instance().ClosePeerConnection();
            // initialize new peer socket for next matching
            ClientTCP.Instance().InitializePeerSock();
            SceneManager.LoadScene("Lobby");
        }

        public void DamageTower(ref ByteBuffer buffer)
        {
            int towerID = buffer.ReadInteger(true);
            BattleUIExample.Instance().DamageTower(towerID);
        }

        public void HandleHolePunching(ref ByteBuffer buffer)
        {
            IPEndPoint externalEP = buffer.ReadEndPoint(true);
            Debug.Log("my public ip: " + externalEP);
            ClientTCP.Instance().peerSockPublicEP = externalEP;

            ClientTCP.Instance().CloseHolePunchingConnection();
        }

        public void HandleTurnOn(ref ByteBuffer buffer)
        {
            if(inGamePacketQueue.Count > 0)
            {
                ByteBuffer inGameBuffer = inGamePacketQueue.Dequeue();
                int card_id = inGameBuffer.ReadInteger(true);
                //Vector3 spawnLocation = buffer.ReadVector3(true);

                // spawn with spawner class
                Debug.Log($"spawn {card_id}");
            }

            Debug.Log("turn on");
        }

        public void SpawnCard(ref ByteBuffer buffer)
        {
            inGamePacketQueue.Enqueue(buffer);
        }
    }

}

