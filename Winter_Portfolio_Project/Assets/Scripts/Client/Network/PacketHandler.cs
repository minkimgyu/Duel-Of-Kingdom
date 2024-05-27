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
        //public Queue<byte[]> inGamePacketQueue { get; set; }

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
            //inGamePacketQueue = new Queue<byte[]>();
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

            packetHandler.Add((int)Server_PacketTagPackages.S_REQUEST_HOLE_PUNCHING, HandleHolePunching);
            packetHandler.Add((int)Server_PacketTagPackages.S_REQUEST_SYNCHRONIZATION, HandleSynchronization);
            packetHandler.Add((int)Server_PacketTagPackages.S_SEND_PING, AnswerPing);

            //packetHandler.Add((int)Peer_PacketTagPackages.P_SEND_PING, RequestRoundTripTime);
            //packetHandler.Add((int)Peer_PacketTagPackages.P_ANSWER_PING, GetRoundTripTime);

            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_SPAWN_CARD, SpawnCard);
            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_SPAWN_TOWER, SpawnTower);
            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_SPAWN_UNIT, SpawnWithUnitData);
            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_SYNCHRONIZATION, SynchronizeUnits);
            packetHandler.Add((int)Peer_PacketTagPackages.P_REQUEST_DESTROY_UNIT, DestroyUnit);

            packetHandler.Add((int)Peer_PacketTagPackages.P_SEND_COMMANDS, HandleCommands);
        }

        public void HandlePacket(byte[] packet)
        {
            lock (_packetHandlerLockObj)
            {
                if (packet == null || packet.Length == 0)
                    return;

                if (ClientTCP.Instance().Buffer == null)
                {
                    ClientTCP.Instance().Buffer = new ByteBuffer();
                }

                // packet ����
                ClientTCP.Instance().Buffer.WriteBytes(packet);

                if (_isSegmented == false)
                {
                    _packetLength = ClientTCP.Instance().Buffer.ReadInteger(true);
                }

                // ó������ ��Ŷ�� ���ҵǾ� ���� ���
                if (_packetLength > ClientTCP.Instance().Buffer.Count() + 4 && _isSegmented == false)
                {
                    _isSegmented = true;
                    return;
                }

                if (_isSegmented == true)
                {
                    // ��Ŷ�� �� �޾Ҵٸ�
                    if (_packetLength == ClientTCP.Instance().Buffer.Count() + 4)
                    {
                        _isSegmented = false;
                        HandleData(ClientTCP.Instance().Buffer.ToArray());
                        _packetLength = 0;
                        ClientTCP.Instance().Buffer = null;
                        return;
                    }
                }
                else
                {
                    HandleData(ClientTCP.Instance().Buffer.ToArray());
                    _packetLength = 0;
                    ClientTCP.Instance().Buffer = null;
                }
                return;
            }
        }

        public void HandleInGamePacket(byte[] packet)
        {
            if (packet == null || packet.Length == 0)
                return;

            if (ClientTCP.Instance().InGameBuffer == null)
            {
                ClientTCP.Instance().InGameBuffer = new ByteBuffer();
            }

            // packet ����
            ClientTCP.Instance().InGameBuffer.WriteBytes(packet);

            if (_isInGamePacketSegmentated == false)
            {
                _inGamePacketLength = ClientTCP.Instance().InGameBuffer.ReadInteger(true);
            }

            // ó������ ��Ŷ�� ���ҵǾ� ���� ���
            if (_inGamePacketLength > ClientTCP.Instance().InGameBuffer.Count() + 4 && _isInGamePacketSegmentated == false)
            {
                _isInGamePacketSegmentated = true;
                return;
            }

            if (_isInGamePacketSegmentated == true)
            {
                // ��Ŷ�� �� �޾Ҵٸ�
                if (_inGamePacketLength == ClientTCP.Instance().InGameBuffer.Count() + 4)
                {
                    _isInGamePacketSegmentated = false;
                    HandleData(ClientTCP.Instance().InGameBuffer.ToArray());
                    _inGamePacketLength = 0;
                    ClientTCP.Instance().InGameBuffer = null;
                    Debug.Log("received all packets");
                    return;
                }
            }
            else
            {
                HandleData(ClientTCP.Instance().InGameBuffer.ToArray());
                _packetLength = 0;
                ClientTCP.Instance().InGameBuffer = null;
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
                if(packetTag < 30)
                {
                    Debug.Log("Handle " + (Server_PacketTagPackages)packetTag);
                }
                else
                {
                    Debug.Log("Handle " + (Peer_PacketTagPackages)packetTag);
                }
                func.Invoke(ref buffer);
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
            Debug.Log("my private ep: " + ClientTCP.Instance().PeerSockPrivateEP);
            Debug.Log("my public ep: " + ClientTCP.Instance().PeerSockPublicEP);
            Debug.Log("opponent's private ep: " + opponentPrivateEP);
            Debug.Log("opponent's public ep: " + opponentPublicEP);
            GameRoom.Instance().opponentPrivateEP = opponentPrivateEP;
            GameRoom.Instance().opponentPublicEP = opponentPublicEP;

            // set player_id
            int player_id_in_game = buffer.ReadInteger(true);
            Debug.Log("player_id_in_game: " + player_id_in_game);
            ClientData.Instance().player_id_in_game = player_id_in_game;

            // try to connect with private end point
            ClientTCP.Instance().ConnectPeer(opponentPrivateEP);

            //ClientTCP.Instance().SendDataToPeer(Peer_PacketTagPackages.P_SEND_PING);
            //Debug.Log("send ping");
            SceneManager.LoadScene("CameraTestScene");
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
            ClientTCP.Instance().ConnectServerForHolePunching();
        }

        public void HandleHolePunching(ref ByteBuffer buffer)
        {
            IPEndPoint externalEP = buffer.ReadEndPoint(true);
            Debug.Log("my public ip: " + externalEP);
            ClientTCP.Instance().PeerSockPublicEP = externalEP;

            // request initial data after hole punching finishes
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_REQUEST_INITIAL_DATA);
            Debug.Log("C_REQUEST_INITIAL_DATA");

            ClientTCP.Instance().CloseHolePunchingConnection();
        }

        public void AnswerPing(ref ByteBuffer buffer)
        {
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_ANSWER_PING);
        }

        /*public void RequestRoundTripTime(ref ByteBuffer buffer)
        {
            ClientTCP clientTCP = ClientTCP.Instance();
            clientTCP.PingSentTime = DateTime.Now;
            clientTCP.SendDataToPeer(Peer_PacketTagPackages.P_ANSWER_PING);
            Debug.Log("answer ping");
        }

        public void GetRoundTripTime(ref ByteBuffer buffer)
        {
            ClientTCP clientTCP = ClientTCP.Instance();
            clientTCP.PingAnsweredTime = DateTime.Now;
            clientTCP.Rtt = clientTCP.PingAnsweredTime.Subtract(clientTCP.PingSentTime);
            double rttMilliseconds = clientTCP.Rtt.TotalMilliseconds;

            Debug.Log("Round-Trip Time: " + rttMilliseconds + " milliseconds");
        }*/

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

        public void SpawnWithUnitData(ref ByteBuffer buffer)
        {
            string cardName = buffer.ReadString(true);
            int level = buffer.ReadInteger(true);
            int opponentOwnershipId = buffer.ReadInteger(true);
            int numOfCardsToSpawn = buffer.ReadInteger(true);
            string[] networkIds = new string[numOfCardsToSpawn];
            for (int i = 0; i < numOfCardsToSpawn; i++)
            {
                networkIds[i] = buffer.ReadString(true);
            }
            Vector3 pos = buffer.ReadVector3(true);
            Vector3[] offsets = new Vector3[numOfCardsToSpawn];
            for (int i = 0; i < numOfCardsToSpawn; i++)
            {
                offsets[i] = buffer.ReadVector3(true);
            }

            CardData cardData = CardCollection.Instance().FindCard(cardName, level);
            float duration = cardData.duration;

            int unitCount = cardData.spawnData.spawnUnitCount;

            Entity[] spawnedEntities = new Entity[unitCount];

            for (int i = 0; i < unitCount; i++)
            {
                spawnedEntities[i] = Spawner.Instance().Instantiate(cardData, 0, opponentOwnershipId, networkIds[i], pos + new Vector3(offsets[i].x, 0, offsets[i].y));
            }
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

                // �ڽŰ� ������ networkId�� ���� entity�� ã�� ����ȭ�� ���ش�
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

        public void HandleCommands(ref ByteBuffer buffer)
        {
            Debug.Log("HandleCommands");

            int numOfCommands = buffer.ReadInteger(true);

            Debug.Log($"numOfCommands: {numOfCommands}");
            for (int i=0; i< numOfCommands; i++)
            {
                int commandSize = buffer.ReadInteger(true);
                int tag = buffer.ReadInteger(true);
                byte[] command = buffer.ReadBytes(commandSize - 8, true);

                Debug.Log($"commandSize: {commandSize}");
                Debug.Log($"tag: {(Peer_PacketTagPackages)tag}");

                // ������ command���� �� List�� �߰�
                TurnManager.Instance.AddOpponentCommand((Peer_PacketTagPackages)tag, command);
            }

            // ������ Ŀ�ǵ带 �� List�� �߰�
            // ������ Ŀ�ǵ带 �߰��ϴ� ����: ������ ���� ������ Ȯ���ϱ� ����
            // ���� Ŀ�ǵ� ���� => ������ ī�� ��ȯ�� ���� �ʰ� ��� ���� �ǹ� 
            // ���� Ŀ�ǵ� ����x => ������� ���� ���� Ȥ�� ���� ������
            if (numOfCommands == 0)
            {
                int commandSize = buffer.ReadInteger(true);
                int tag = buffer.ReadInteger(true);
                TurnManager.Instance.AddOpponentCommand((Peer_PacketTagPackages)tag, null);
            }
        }
    }

}

