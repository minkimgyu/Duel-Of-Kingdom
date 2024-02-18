using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.JSON;
using WPP.AI.STAT;
using WPP.AI.UI;
using WPP.AI.GRID;
using WPP.ClientInfo;
using WPP.DeckManagement;
using WPP.Collection;
using WPP.ClientInfo.Card;
using WPP.ClientInfo.Tower;
using System;
using WPP.Network;
using Unity.VisualScripting;
using WPP.SOUND;

namespace WPP.AI.SPAWNER
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] HpContainerUI _hpContainerUIPrefab;
        [SerializeField] ClockUI _clockUIPrefab;

        [SerializeField] Transform _cMagicProjectileStartPoint;
        [SerializeField] Transform _rMagicProjectileStartPoint;

        [SerializeField] List<Entity> _entityPrefabs;
        [SerializeField] List<Entity> _spawnedEntities;
        public List<Entity> SpawnedEntities { get { return _spawnedEntities; } }

        private object _spawnerLockObj;

        public void RemoveFromListInSpawner(string networkId)
        {
            Entity entity = _spawnedEntities.Find(x => x.NetwordId == networkId);
            if (entity == null) return; // ������Ʈ�� ���ٸ� return;
            if(entity.CanAttachHpBar() == true)
            {
                if (entity != null)
                {
                   ((Life)entity).Die();
                }
            }

            _spawnedEntities.Remove(entity);
        }

        public Entity FindSameNetwordIdEntity(string networkId)
        {
            return _spawnedEntities.Find(x => x.NetwordId == networkId);
        }

        private static Spawner _instance;

        int _spawnCount = 0;

        public static Spawner Instance()
        {
            return _instance;
        }

        void Awake()
        {
            _instance = this;
            _spawnerLockObj = new object();
            //_stats = _jsonParser.Load();

        }

        void Start()
        {
            Vector3 kingTowerPos;
            Vector3 leftPrincessTowerPos;
            Vector3 rightPrincessTowerPos;
            if (ClientData.Instance().player_id_in_game == 0)
            {
                kingTowerPos = new Vector3(4.51f, 1, 9.51f);
                leftPrincessTowerPos = new Vector3(-1, 1, 6);
                rightPrincessTowerPos = new Vector3(10, 1, 6);
            }
            else
            {
                kingTowerPos = new Vector3(4.51f, 1, -17.49f);
                leftPrincessTowerPos = new Vector3(-1, 1, -14);
                rightPrincessTowerPos = new Vector3(10, 1, -14);
            }
            SpawnTower(ClientData.Instance().player_id_in_game, kingTowerPos, leftPrincessTowerPos, rightPrincessTowerPos);
        }

        Quaternion ReturnQuaternionUsingLandFormation(int playerId)
        {
            // playerId�� 0�̸� C�������� ����
            // 1�̸� R�������� ����
            // --> �� ���� �������� rotation ������ �������ش�.

            if (playerId == 0) return Quaternion.Euler(new Vector3(0, 180, 0));
            else return Quaternion.Euler(new Vector3(0, 0, 0));
        }

        Vector3 ReturnMagicProjectileStartPoint(int playerId)
        {
            // playerId�� 0�̸� C�������� ����
            // 1�̸� R�������� ����
            // --> �� ���� �������� rotation ������ �������ش�.

            if (playerId == 0) return _cMagicProjectileStartPoint.position;
            else return _rMagicProjectileStartPoint.position;
        }

        string ReturnNetworkId(int ownershipId, string name, int spawnCount)
        {
            //return ownershipId.ToString() + _spawnCount.ToString();
            return ownershipId.ToString() + name + spawnCount.ToString();
        }

        // entityId �̰Ÿ� string�� �ؼ� �ޱ�
        Entity ReturnEntity(string name, int ownershipId, string networkId, Vector3 pos)
        {
            Entity entity = _entityPrefabs.Find(x => x.Name == name);
            if (entity == null) return null;
            /*
            _spawnCount++; // ���⼭ ���� ī��Ʈ�� �÷��ش�.
            string networkId = ReturnNetworkId(ownershipId); // �̰Ÿ� �����Ǵ� ������Ʈ�� �ο��ؾ��Ѵ� --> ���� ������ ���ؼ� �Ѱܼ� �޾��ֱ�
                                                             // �Ű������� �޾ƾ��� ��?
            */

            int clientId = ClientData.Instance().player_id_in_game; // ���� Ŭ���̾�Ʈ ���̵� �޾ƿͼ� �־��ش�.

            // ���߿� ���� ����
            Quaternion rotation = ReturnQuaternionUsingLandFormation(clientId);
            Entity spawnedEntity = Instantiate(entity, pos, rotation);
            

            spawnedEntity.InitializeListRemover(RemoveFromListInSpawner);
            spawnedEntity.ResetId(ownershipId, clientId, networkId);
            // ���⿡ ��� �ð��� �߰����ش�.

            // ü�¹ٸ� ���� �� �ִ� ��쿡�� ����
            if (spawnedEntity.CanAttachHpBar() == true)
            {
                SpawnHpContainerUI(spawnedEntity);
            }

            Vector3 magicStartPosition = ReturnMagicProjectileStartPoint(ownershipId);
            spawnedEntity.ResetMagicStartPosition(magicStartPosition);
            // ���⿡ ȭ�� ���� ���� ��ġ�� �߰����ش�.

            _spawnedEntities.Add(spawnedEntity);
            return spawnedEntity;
        }

        // Ÿ���� 1, ������ ������Ʈ�� 0.5�� ����
        void SpawnHpContainerUI(Entity entity)
        {
            float scaleRatio = entity.ReturnHpContainerScale();

            HpContainerUI hpContainer = Instantiate(_hpContainerUIPrefab);
            hpContainer.OnScaleChangeRequested(scaleRatio);
            entity.AttachHpBar(hpContainer);
        }

        public void SpawnClockUI(CardType type, Vector3 pos, float duration)
        {
            if (type == CardType.spell)
                return;

            SoundManager.PlaySFX("ClockTicking");
            ClockUI clockUI = Instantiate(_clockUIPrefab);
            clockUI.Initialize(pos, duration);
        }

        // ���� �κп��� �ð� ��� ������ �������Ѽ� �غ� �ð� �����ֱ�
        //Entity Instantiate(int entityId, int ownershipId, Vector2 pos, Quaternion quaternion)
        //{
        //    BaseStat stat = _stats.Find(x => x._id == entityId);
        //    if (stat == null) return null;

        //    Entity entity = ReturnEntity(entityId, ownershipId, pos, quaternion);
        //    if (entity == null) return null;

        //    stat.Initialize(entity);
        //    return entity;
        //}

        //Entity Instantiate(int entityId, int ownershipId, Vector2 pos, Quaternion quaternion, float duration)
        //{
        //    BaseStat stat = _stats.Find(x => x._id == entityId);
        //    if (stat == null) return null;

        //    Entity entity = ReturnEntity(entityId, ownershipId, pos, quaternion);
        //    if (entity == null) return null;

        //    entity.ResetDelayAfterSpawn(duration);
        //    stat.Initialize(entity);
        //    return entity;
        //}

        public void Instantiate(int ownershipId, Vector3 kingTowerPos, string kingTowerNetworkId, Vector3 leftPrincessTowerPos, string leftPrincessTowerNetworkId, Vector3 rightPrincessTowerPos, string rightPrincessTowerNetworkId)
        {
            // name�� level�� ���� ��츦 ã�Ƽ� ����
            // ���Ŀ� offset�� �߰��� ���� ������Ѿ��� ���� �ֱ� ������ �װ͵� �����غ���
            //BaseStat stat = _stats.Find(x => x._name == name && x._level == level);
            //if (stat == null) return null;

            //entity.ResetDelayAfterSpawn(duration);
            //stat.ResetData(entity);
            //return entity;
            TowersData towersData = ClientData.Instance().towers;

            Entity kingTower = ReturnEntity("king_tower", ownershipId, kingTowerNetworkId, kingTowerPos);
            if (kingTower == null) return;
            towersData.kingTower.towerUnit.ResetData(kingTower);

            Entity leftPrincessTower = ReturnEntity("princess_tower", ownershipId, leftPrincessTowerNetworkId, leftPrincessTowerPos);
            if (leftPrincessTower == null) return;
            leftPrincessTower.IsLeft(true);

            towersData.leftPrincessTower.towerUnit.ResetData(leftPrincessTower);

            Entity rightPrincessTower = ReturnEntity("princess_tower", ownershipId, rightPrincessTowerNetworkId, rightPrincessTowerPos);
            if (rightPrincessTower == null) return;
            rightPrincessTower.IsLeft(false);

            towersData.rightPrincessTower.towerUnit.ResetData(rightPrincessTower);
        }

        public Entity Instantiate(CardData cardData, float duration, int ownershipId, string networkId, Vector3 pos)
        {
            Entity entity = ReturnEntity(cardData.unit._name, ownershipId, networkId, pos);
            if (entity == null) return null;

            entity.ResetDelayAfterSpawn(duration);

            if (cardData.unit == null) return null;
            cardData.unit.ResetData(entity);

            return entity;
        }

        ByteBuffer GetSpawnBuffer(string cardId, int level, int ownershipId, int numOfCardsToSpawn, string[] networkIds, Vector3 pos)
        {
            ByteBuffer bufferToSend = new ByteBuffer();
            bufferToSend.WriteString(cardId);
            bufferToSend.WriteInteger(level);
            bufferToSend.WriteInteger(ownershipId);
            bufferToSend.WriteInteger(numOfCardsToSpawn);
            for(int i=0; i< numOfCardsToSpawn; i++)
            {
                bufferToSend.WriteString(networkIds[i]);
            }
            bufferToSend.WriteVector3(pos);
            return bufferToSend;
        }

        /// <summary>
        /// ī�带 ����ؼ� ������Ű�� ���
        /// </summary>
        public void Spawn(Card card, int level, int ownershipId, Vector3 pos)
        {
            if (ownershipId != ClientData.Instance().player_id_in_game)
                return;

            CardData cardData = CardCollection.Instance().FindCard(card.id, level);
            float duration = cardData.duration;

            int unitCount = cardData.spawnData.spawnUnitCount;
            string[] networkIds = new string[unitCount];
            SerializableVector2[] offset = cardData.spawnData.spawnOffset;

            for (int i = 0; i < unitCount; i++)
            {
                ++_spawnCount;
                networkIds[i] = ReturnNetworkId(ownershipId, cardData.unit._name, _spawnCount);

                Instantiate(cardData, duration, ownershipId, networkIds[i], pos + new Vector3(offset[i]._x, 0, offset[i]._y));
            }

            SoundManager.PlaySFX("Spawn");

            SpawnClockUI(cardData.type, pos, duration);

            ByteBuffer bufferToSend = GetSpawnBuffer(card.id, level, ownershipId, unitCount, networkIds, pos);
            ClientTCP.Instance().SendDataToPeer(Peer_PacketTagPackages.P_REQUEST_SPAWN_CARD, bufferToSend.ToArray());
            return;
        }

        public void Spawn(string cardId, int level, int ownershipId, Vector3 pos, Vector3[] offsets)
        {
            if (ownershipId != ClientData.Instance().player_id_in_game)
                return;

            CardData cardData = CardCollection.Instance().FindCard(cardId, level);
            float duration = cardData.duration;

            int unitCount = offsets.Length;
            string[] networkIds = new string[unitCount];

            for (int i = 0; i < offsets.Length; i++)
            {
                ++_spawnCount;
                string networkId = ReturnNetworkId(ownershipId, cardData.unit._name, _spawnCount);
                networkIds[i] = networkId;

                Instantiate(cardData, 0, ownershipId, networkId, pos + new Vector3(offsets[i].x, 0, offsets[i].y));
            }

            SoundManager.PlaySFX("Spawn");

            ByteBuffer bufferToSend = GetSpawnBuffer(cardId, level, ownershipId, unitCount, networkIds, pos);
            ClientTCP.Instance().SendDataToPeer(Peer_PacketTagPackages.P_REQUEST_SPAWN_UNIT, bufferToSend.ToArray());

            return;
        }

        /// <summary>
        /// ī��� ������Ű�� �ʴ� ��� ex) Ÿ��
        /// duration�� �ִ� ��� ex) �ٹٸ��� ���θ�
        /// </summary>
        //public Entity Spawn(string name, int level, float duration, int ownershipId, Vector3 pos)
        //{
        //    SpawnClockUI(pos, duration);
        //    return Instantiate(name, level, duration, ownershipId, pos);
        //}

        /// <summary>
        /// ī��� ������Ű�� �ʴ� ��� ex) Ÿ��
        /// duration�� ���� ��� ex) ŷ, �������� Ÿ��
        /// </summary>
        //public Entity Spawn(string name, int level, int ownershipId, Vector3 pos)
        //{
        //    return Instantiate(name, level, ownershipId, pos);
        //}
       

        public void SpawnTower(int ownershipId, Vector3 kingTowerPos, Vector3 leftPrincessTowerPos, Vector3 rightPrincessTowerPos)
        {
            ++_spawnCount;
            string kingTowerNetworkId = ReturnNetworkId(ownershipId, "king_tower", _spawnCount);
            ++_spawnCount;
            string leftPrincessTowerNetworkId = ReturnNetworkId(ownershipId, "princess_tower", _spawnCount);
            ++_spawnCount;
            string rightPrincessTowerNetworkId = ReturnNetworkId(ownershipId, "princess_tower", _spawnCount);
            Instantiate(ownershipId, kingTowerPos, kingTowerNetworkId, leftPrincessTowerPos, leftPrincessTowerNetworkId, rightPrincessTowerPos, rightPrincessTowerNetworkId);

            ByteBuffer bufferToSend = new ByteBuffer();
            bufferToSend.WriteInteger(ownershipId);

            bufferToSend.WriteVector3(kingTowerPos);
            bufferToSend.WriteString(kingTowerNetworkId);

            bufferToSend.WriteVector3(leftPrincessTowerPos);
            bufferToSend.WriteString(leftPrincessTowerNetworkId);

            bufferToSend.WriteVector3(rightPrincessTowerPos);
            bufferToSend.WriteString(rightPrincessTowerNetworkId);

            ClientTCP.Instance().SendDataToPeer(Peer_PacketTagPackages.P_REQUEST_SPAWN_TOWER, bufferToSend.ToArray());
        }

        //public Entity Spawn(int entityId, int ownershipId, Vector3 pos)
        //{
        //    return Instantiate(entityId, ownershipId, pos, Quaternion.identity);
        //}

        //public Entity Spawn(int entityId, int ownershipId, Vector3 pos, LandFormation myFormation)
        //{
        //    if (LandFormation.C == myFormation)
        //    {
        //        Quaternion reverse = Quaternion.Euler(new Vector3(0, 180, 0));
        //        return Instantiate(entityId, ownershipId, pos, reverse);
        //    }
        //    else
        //    {
        //        return Instantiate(entityId, ownershipId, pos, Quaternion.identity);
        //    }
        //}


        //public Entity Spawn(int entityId, int ownershipId, Vector3 pos, float duration)
        //{
        //    SpawnClockUI(pos, duration);
        //    return Instantiate(entityId, ownershipId, pos, Quaternion.identity, duration);
        //}

        //public Entity Spawn(int entityId, int ownershipId, Vector3 pos, float duration, Quaternion quaternion)
        //{
        //    SpawnClockUI(pos, duration);
        //    return Instantiate(entityId, ownershipId, pos, quaternion, duration);
        //}

        //public Entity[] Spawn(int[] entityIds, int playerId, Vector3 pos, Vector3[] offsets, float duration)
        //{
        //    if (entityIds.Length != offsets.Length) return null;
        //    Entity[] entities = new Entity[entityIds.Length];

        //    for (int i = 0; i < entityIds.Length; i++)
        //        entities[i] = Instantiate(entityIds[i], playerId, pos + offsets[i], Quaternion.identity, duration);

        //    SpawnClockUI(pos, duration);

        //    return entities;
        //}

        //public Entity[] Spawn(List<EntitySpawnData> entitySpawnDatas, int level, int playerId, Vector3 pos, float duration, LandFormation myLandFormation)
        //{
        //    Entity[] entities = new Entity[entitySpawnDatas.Count];

        //    for (int i = 0; i < entitySpawnDatas.Count; i++)
        //    {
        //        Vector3 offsets = new Vector3(entitySpawnDatas[i].spawnOffset.x, 0, entitySpawnDatas[i].spawnOffset.y);
        //        entities[i] = Instantiate(entitySpawnDatas[i], playerId, pos + offsets, Quaternion.identity, duration);
        //    }

        //    SpawnClockUI(pos, duration);

        //    return entities;
        //}

        //public Entity[] Spawn(int[] entityIds, int ownershipId, Vector3 pos, Vector3[] offsets, float duration, Quaternion quaternion)
        //{
        //    if (entityIds.Length != offsets.Length) return null;
        //    Entity[] entities = new Entity[entityIds.Length];

        //    for (int i = 0; i < entityIds.Length; i++)
        //        entities[i] = Instantiate(entityIds[i], ownershipId, pos + offsets[i], quaternion, duration);

        //    SpawnClockUI(pos, duration);
        //    return entities;
        //}

        //public Entity[] Spawn(int entityId, int ownershipId, Vector3 pos, Vector3[] offsets, Quaternion quaternion)
        //{
        //    Entity[] entities = new Entity[offsets.Length];

        //    for (int i = 0; i < offsets.Length; i++)
        //        entities[i] = Instantiate(entityId, ownershipId, pos + offsets[i], quaternion);

        //    return entities;
        //}
    }
}
