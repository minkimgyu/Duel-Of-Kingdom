#undef UNITY_EDITOR

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
using WPP.AI.SPAWNER;
using WPP.Collection;
using WPP.DeckManagement;
using WPP.AI;
using WPP.AI.STAT;
using System.Collections;

namespace WPP.Network
{
    class PacketHandler
    {
        private static PacketHandler _instance;
        private delegate void HandleFunc(ref ByteBuffer buffer);
        private Dictionary<int, HandleFunc> packetHandler;
        public Queue<byte[]> packetQueue { get; set; }
        public Queue<byte[]> inGamePacketQueue { get; set; }

        private int _packetLength;
        private int _inGamePacketLength;
        private bool _isSegmented;
        private bool _isInGamePacketSegmentated;
        private object _packetHandlerLockObj;
        private object _inGamePacketHandlerLockObj;
        public object InGamePacketHandlerLockObj { get { return _inGamePacketHandlerLockObj;  } }

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
            _inGamePacketLength = 0;
            _isSegmented = false;
            _isInGamePacketSegmentated = false;
            packetQueue = new Queue<byte[]>();
            inGamePacketQueue = new Queue<byte[]>();
            packetHandler = new Dictionary<int, HandleFunc>();
            _packetHandlerLockObj = new object();
            _inGamePacketHandlerLockObj = new object();
        }
        public void InitializePacketHandler()
        {
            packetHandler.Add((int)Server_PacketTagPackages.S_LOAD_CARD_COLLECTION, LoadCardCollection);
            packetHandler.Add((int)Server_PacketTagPackages.S_ACCEPT_REGISTER_ACCOUNT, HandleRegisterAcception);
            packetHandler.Add((int)Server_PacketTagPackages.S_REJECT_REGISTER_ACCOUNT, HandleRegisterRejection);
            packetHandler.Add((int)Server_PacketTagPackages.S_ACCEPT_LOGIN, HandleLoginAcception);
            packetHandler.Add((int)Server_PacketTagPackages.S_REJECT_LOGIN, HandleLoginRejection);
            packetHandler.Add((int)Server_PacketTagPackages.S_REQUEST_PLAY_GAME, HandleEnterGame);

            packetHandler.Add((int)Server_PacketTagPackages.S_ALERT_OVER_TIME, HandleOverTime);
            packetHandler.Add((int)Server_PacketTagPackages.S_REQUEST_END_GAME, HandleEndGame);

            packetHandler.Add((int)Server_PacketTagPackages.S_REQUSET_HOLE_PUNCHING, HandleHolePunching);
            packetHandler.Add((int)Server_PacketTagPackages.S_REQUEST_SYNCHRONIZATION, HandleSynchronization);

            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_PING, RequestRoundTripTime);
            packetHandler.Add((int)Peer_PacketTagPackages.P_ANSWER_PING, GetRoundTripTime);

            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_SPAWN_CARD, SpawnCard);
            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_SPAWN_TOWER, SpawnTower);
            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_SPAWN_UNIT, SpawnUsingUnitData);
            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_SYNCHRONIZATION, SynchronizeUnits);
            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_DESTROY_UNIT, DestroyUnit);
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

            if (_isSegmented == false)
            {
                _packetLength = ClientTCP.Instance().buffer.ReadInteger(true);
            }

            // 처음으로 패킷이 분할되어 왔을 경우
            if (_packetLength > ClientTCP.Instance().buffer.Count() + 4 && _isSegmented == false)
            {
                _isSegmented = true;
                return;
            }

            if (_isSegmented == true)
            {
                // 패킷을 다 받았다면
                if (_packetLength == ClientTCP.Instance().buffer.Count() + 4)
                {
                    _isSegmented = false;
                    HandleData(ClientTCP.Instance().buffer.ToArray());
                    _packetLength = 0;
                    ClientTCP.Instance().buffer.Dispose();
                    ClientTCP.Instance().buffer = null;
                    Debug.Log("received all packets");
                    return;
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

        public void HandleInGamePacket(byte[] packet)
        {
            if (packet == null || packet.Length == 0)
                return;

            if (ClientTCP.Instance().inGameBuffer == null)
            {
                ClientTCP.Instance().inGameBuffer = new ByteBuffer();
            }

            // packet 복사
            ClientTCP.Instance().inGameBuffer.WriteBytes(packet);

            if (_isInGamePacketSegmentated == false)
            {
                _inGamePacketLength = ClientTCP.Instance().inGameBuffer.ReadInteger(true);
            }

            // 처음으로 패킷이 분할되어 왔을 경우
            if (_inGamePacketLength > ClientTCP.Instance().inGameBuffer.Count() + 4 && _isInGamePacketSegmentated == false)
            {
                _isInGamePacketSegmentated = true;
                return;
            }

            if (_isInGamePacketSegmentated == true)
            {
                // 패킷을 다 받았다면
                if (_inGamePacketLength == ClientTCP.Instance().inGameBuffer.Count() + 4)
                {
                    _isInGamePacketSegmentated = false;
                    HandleData(ClientTCP.Instance().inGameBuffer.ToArray());
                    _inGamePacketLength = 0;
                    ClientTCP.Instance().inGameBuffer.Dispose();
                    ClientTCP.Instance().inGameBuffer = null;
                    Debug.Log("received all packets");
                    return;
                }
            }
            else
            {
                HandleData(ClientTCP.Instance().inGameBuffer.ToArray());
                _packetLength = 0;
                ClientTCP.Instance().inGameBuffer.Dispose();
                ClientTCP.Instance().inGameBuffer = null;
            }
            return;
        }

        public void HandleData(byte[] data)
        {
            lock(_packetHandlerLockObj)
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
        }

        public void LoadCardCollection(ref ByteBuffer buffer)
        {
            string cardCollectionString = buffer.ReadString(true);
#if UNITY_EDITOR
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
            ClientTCP clientTCP = ClientTCP.Instance();

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
            Debug.Log("player_id_in_game: " + player_id_in_game);
            ClientData.Instance().player_id_in_game = player_id_in_game;

            // try to connect with private end point
            clientTCP.ConnectPeer(opponentPrivateEP);
            if (clientTCP.peerSock == null || clientTCP.peerSock.Connected == false)
            {
                // try to connect with public end point
                clientTCP.ConnectPeer(opponentPublicEP);
            }

            clientTCP.SendDataToPeer(Peer_PacketTagPackages.P_REQUEST_PING);
            Debug.Log("send ping");

            SceneManager.LoadScene("CameraTestScene");

            Debug.Log("entered game");
        }

        public void HandleOverTime(ref ByteBuffer buffer)
        {
            Debug.Log("Handle over time");
            BattleManager.Instance().StartOverTime();
        }

        public void HandleEndGame(ref ByteBuffer buffer)
        {
            ClientData.Instance().player_id_in_game = -1;
            ClientTCP.Instance().ClosePeerConnection();
            // initialize new peer socket for next matching
            ClientTCP.Instance().InitializePeerSock();
        }

        public void HandleHolePunching(ref ByteBuffer buffer)
        {
            IPEndPoint externalEP = buffer.ReadEndPoint(true);
            Debug.Log("my public ip: " + externalEP);
            ClientTCP.Instance().peerSockPublicEP = externalEP;

            ClientTCP.Instance().CloseHolePunchingConnection();
        }

        public void RequestRoundTripTime(ref ByteBuffer buffer)
        {
            ClientTCP clientTCP = ClientTCP.Instance();
            clientTCP.pingSentTime = DateTime.Now;
            clientTCP.SendDataToPeer(Peer_PacketTagPackages.P_ANSWER_PING);
            Debug.Log("answer ping");
        }

        public void GetRoundTripTime(ref ByteBuffer buffer)
        {
            ClientTCP clientTCP = ClientTCP.Instance();
            clientTCP.pingAnsweredTime = DateTime.Now;
            clientTCP.rtt = clientTCP.pingAnsweredTime.Subtract(clientTCP.pingSentTime);
            double rttMilliseconds = clientTCP.rtt.TotalMilliseconds;

            Debug.Log("Round-Trip Time: " + rttMilliseconds + " milliseconds");
        }

        public void HandleSynchronization(ref ByteBuffer buffer)
        {
            List<Entity> spawnedEntities = Spawner.Instance().SpawnedEntities;
            ByteBuffer syncBuffer = new ByteBuffer();

            int numOfSpawnedEntites = spawnedEntities.Count;

            syncBuffer.WriteInteger(numOfSpawnedEntites);

            for (int i = 0; i < numOfSpawnedEntites; i++)
            {
                string networkId = spawnedEntities[i].NetwordId;
                float hp = 0;
                if (spawnedEntities[i].CanAttachHpBar() == true)
                {
                    if (spawnedEntities[i] != null)
                    {
                        hp = ((Life)spawnedEntities[i]).HP;
                    }
                }
                 
                Vector3 pos = (spawnedEntities[i]).transform.position;
                Quaternion rotation = (spawnedEntities[i]).transform.rotation;
                syncBuffer.WriteString(networkId);
                syncBuffer.WriteFloat(hp);
                syncBuffer.WriteVector3(pos);
                syncBuffer.WriteQuaternion(rotation);
            }
            ClientTCP.Instance().SendDataToPeer(Peer_PacketTagPackages.P_REQUEST_SYNCHRONIZATION, syncBuffer.ToArray());
        }

        public void SpawnCard(ref ByteBuffer buffer)
        {
            string cardName = buffer.ReadString(true);
            int level = buffer.ReadInteger(true);
            int opponentOwnershipId = buffer.ReadInteger(true);
            int numOfCardsToSpawn = buffer.ReadInteger(true);
            string[] networkIds = new string[numOfCardsToSpawn];
            for(int i=0;i< numOfCardsToSpawn; i++)
            {
                networkIds[i] = buffer.ReadString(true);
            }
            Vector3 pos = buffer.ReadVector3(true);

            CardData cardData = CardCollection.Instance().FindCard(cardName, level);
            float duration = cardData.duration;

            int unitCount = cardData.spawnData.spawnUnitCount;
            SerializableVector2[] offset = cardData.spawnData.spawnOffset;

            Entity[] spawnedEntities = new Entity[unitCount];

            for (int i = 0; i < unitCount; i++)
            {
                spawnedEntities[i] = Spawner.Instance().Instantiate(cardData, duration, opponentOwnershipId, networkIds[i], pos + new Vector3(offset[i]._x, 0, offset[i]._y));
            }

            Spawner.Instance().SpawnClockUI(cardData.type, pos, duration);
        }

        public void SpawnUsingUnitData(ref ByteBuffer buffer)
        {
            string cardName = buffer.ReadString(true);
            int level = buffer.ReadInteger(true);
            int opponentOwnershipId = buffer.ReadInteger(true);
            string networkId = buffer.ReadString(true);
            Vector3 pos = buffer.ReadVector3(true);

            CardData cardData = CardCollection.Instance().FindCard(cardName, level);
            float duration = cardData.duration;

            Spawner.Instance().Instantiate(cardData, duration, opponentOwnershipId, networkId, pos);
            Spawner.Instance().SpawnClockUI(cardData.type, pos, duration);
        }

        public void SpawnTower(ref ByteBuffer buffer)
        {
            int ownershipId = buffer.ReadInteger(true);
            Vector3 kingTowerPos = buffer.ReadVector3(true);
            string kingTowerNetworkId = buffer.ReadString(true);

            Vector3 leftPrincessTowerPos = buffer.ReadVector3(true);
            string leftPrincessTowerNetworkId = buffer.ReadString(true);

            Vector3 rightPrincessTowerPos = buffer.ReadVector3(true);
            string rightPrincessTowerNetworkId = buffer.ReadString(true);

            Spawner.Instance().Instantiate(ownershipId, kingTowerPos, kingTowerNetworkId, leftPrincessTowerPos, leftPrincessTowerNetworkId, rightPrincessTowerPos, rightPrincessTowerNetworkId);
        }

        public void SynchronizeUnits(ref ByteBuffer buffer)
        {
            int numOfEntitiesSpawned = buffer.ReadInteger(true);

            for (int i=0; i< numOfEntitiesSpawned; i++)
            {
                string networkId = buffer.ReadString(true);
                float targetHP = buffer.ReadFloat(true);
                Vector3 targetPos = buffer.ReadVector3(true);
                Quaternion targetRotation = buffer.ReadQuaternion(true);

                // 자신과 동일한 networkId를 지닌 entity를 찾아 동기화를 해준다
                Entity sameEntity = Spawner.Instance().FindSameNetwordIdEntity(networkId);

                if (sameEntity == null)
                {
                    ByteBuffer idBuffer = new ByteBuffer();
                    idBuffer.WriteString(networkId);
                    ClientTCP.Instance().SendDataToPeer(Peer_PacketTagPackages.P_REQUEST_DESTROY_UNIT, idBuffer.ToArray());
                    continue;
                }

                if (!sameEntity.IsMyEntity)
                {
                    if(sameEntity.CanAttachHpBar() == true && (sameEntity as Life).HP != targetHP)
                    {
                        Debug.Log("Synchronize " + sameEntity.Name + (sameEntity as Life).HP + " to " + targetHP);
                        sameEntity.SynchronizeHP(targetHP);
                    }
                    sameEntity.transform.position = targetPos;
                    sameEntity.transform.rotation = targetRotation;
                }
            }
        }

        public void DestroyUnit(ref ByteBuffer buffer)
        {
            string networkId = buffer.ReadString(true);
            Entity sameEntity = Spawner.Instance().FindSameNetwordIdEntity(networkId);
            if(sameEntity != null)
            {
                Debug.Log("destroy " + sameEntity.Name);
                sameEntity.DestroyMyself();
            }
        }

    }

}

