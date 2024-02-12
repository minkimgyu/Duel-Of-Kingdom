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
using System;

namespace WPP.AI.SPAWNER
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] HpContainerUI _hpContainerUIPrefab;
        [SerializeField] ClockUI _clockUIPrefab;

        [SerializeField] Transform _cMagicProjectileStartPoint;
        [SerializeField] Transform _rMagicProjectileStartPoint;

        [SerializeField] List<Entity> _entityPrefabs;
        List<BaseStat> _stats;

        JsonParser _jsonParser;

        void Awake()
        {
            _jsonParser = GetComponent<JsonParser>();
            _stats = _jsonParser.Load();
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

        // entityId �̰Ÿ� string�� �ؼ� �ޱ�
        Entity ReturnEntity(string name, int ownershipId, Vector3 pos)
        {
            Entity entity = _entityPrefabs.Find(x => x.Name == name);
            if (entity == null) return null;

            // ���߿� ���� ����
            Quaternion rotation = ReturnQuaternionUsingLandFormation(1);
            Entity spawnedEntity = Instantiate(entity, pos, rotation);


            int clientId = ClientData.Instance().player_id_in_game; // ���� Ŭ���̾�Ʈ ���̵� �޾ƿͼ� �־��ش�.

            // ü�¹ٸ� ���� �� �ִ� ��쿡�� ����
            if(spawnedEntity.CanAttachHpBar() == true)
            {
                HpContainerUI hpContainer = Instantiate(_hpContainerUIPrefab);
                spawnedEntity.AttachHpBar(hpContainer);
            }

            spawnedEntity.ResetPlayerId(ownershipId, 1);
            // ���⿡ ��� �ð��� �߰����ش�.

            Vector3 magicStartPosition = ReturnMagicProjectileStartPoint(1);
            spawnedEntity.ResetMagicStartPosition(magicStartPosition);
            // ���⿡ ȭ�� ���� ���� ��ġ�� �߰����ش�.

            return spawnedEntity;
        }

        void SpawnClockUI(Vector3 pos, float duration)
        {
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

        Entity Instantiate(string name, BaseStat stat, int ownershipId, Vector3 pos)
        {
            Entity entity = ReturnEntity(name, ownershipId, pos);
            if (entity == null) return null;

            stat.ResetData(entity);
            return entity;
        }

        Entity Instantiate(string name, int level, int ownershipId, Vector3 pos)
        {
            // name�� level�� ���� ��츦 ã�Ƽ� ����
            // ���Ŀ� offset�� �߰��� ���� ������Ѿ��� ���� �ֱ� ������ �װ͵� ����غ���
            BaseStat stat = _stats.Find(x => x._name == name && x._level == level);
            if (stat == null) return null;

            Entity entity = ReturnEntity(name, ownershipId, pos);
            if (entity == null) return null;

            stat.ResetData(entity);
            return entity;
        }

        Entity Instantiate(string name, int level, float duration, int ownershipId, Vector3 pos)
        {
            // name�� level�� ���� ��츦 ã�Ƽ� ����
            // ���Ŀ� offset�� �߰��� ���� ������Ѿ��� ���� �ֱ� ������ �װ͵� ����غ���
            BaseStat stat = _stats.Find(x => x._name == name && x._level == level);
            if (stat == null) return null;

            Entity entity = ReturnEntity(name, ownershipId, pos);
            if (entity == null) return null;

            entity.ResetDelayAfterSpawn(duration);
            stat.ResetData(entity);
            return entity;
        }

        Entity Instantiate(CardData cardData, float duration, int ownershipId, Vector3 pos)
        {
            Entity entity = ReturnEntity(cardData.unit._name, ownershipId, pos);
            if (entity == null) return null;

            if (cardData.unit == null) return null;
            cardData.unit.ResetData(entity);

            entity.ResetDelayAfterSpawn(duration);
            return entity;
        }

        /// <summary>
        /// ī�带 ����ؼ� ������Ű�� ���
        /// </summary>
        public Entity[] Spawn(Card card, int level, int ownershipId, Vector3 pos)
        {
            CardData cardData = CardCollection.Instance().FindCard(card.id, level);
            float duration = cardData.duration;

            int unitCount = cardData.spawnData.spawnUnitCount;
            Vector2[] offset = cardData.spawnData.spawnOffset;

            Entity[] spawnedEntities = new Entity[unitCount];

            for (int i = 0; i < unitCount; i++)
            {
                spawnedEntities[i] = Instantiate(cardData, duration, ownershipId, pos + new Vector3(offset[i].x, 0, offset[i].y));
            }

            SpawnClockUI(pos, duration);

            return spawnedEntities;
        }


        /// <summary>
        /// ī��� ������Ű�� �ʴ� ��� ex) Ÿ��
        /// duration�� �ִ� ��� ex) �ٹٸ��� ���θ�
        /// </summary>
        public Entity Spawn(string name, int level, float duration, int ownershipId, Vector3 pos)
        {
            SpawnClockUI(pos, duration);
            return Instantiate(name, level, duration, ownershipId, pos);
        }

        /// <summary>
        /// ī��� ������Ű�� �ʴ� ��� ex) Ÿ��
        /// duration�� ���� ��� ex) ŷ, �������� Ÿ��
        /// </summary>
        public Entity Spawn(string name, int level, int ownershipId, Vector3 pos)
        {
            return Instantiate(name, level, ownershipId, pos);
        }


        public Entity Spawn(string name, BaseStat stat, int ownershipId, Vector3 pos)
        {
            return Instantiate(name, stat, ownershipId, pos);
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
