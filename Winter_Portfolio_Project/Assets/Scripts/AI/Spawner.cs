using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.JSON;
using WPP.AI.STAT;
using WPP.AI.UI;
using WPP.AI.GRID;
using System;

namespace WPP.AI.SPAWNER
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] HpContainerUI _hpContainerUIPrefab;
        [SerializeField] ClockUI _clockUIPrefab;

        [SerializeField] List<Entity> _entityPrefabs;
        List<BaseStat> _stats;

        JsonParser _jsonParser;

        void Awake()
        {
            _jsonParser = GetComponent<JsonParser>();
            _stats = _jsonParser.Load();
        }

        // 공통되는 부분
        Entity ReturnEntity(int entityId, int ownershipId, int clientId, Vector3 pos, Quaternion quaternion)
        {
            Entity entity = _entityPrefabs.Find(x => x.Id == entityId);
            if (entity == null) return null;

            Entity spawnedEntity = Instantiate(entity, pos, quaternion);

            HpContainerUI hpContainer = Instantiate(_hpContainerUIPrefab);

            spawnedEntity.AttachHpBar(hpContainer);
            spawnedEntity.ResetPlayerId(ownershipId, clientId);
            // 여기에 대기 시간을 추가해준다.

            return spawnedEntity;
        }

        // 여기 부분에서 시계 모양 아이콘 생성시켜서 준비 시간 보여주기
        Entity Instantiate(int entityId, int ownershipId, int clientId, Vector3 pos, Quaternion quaternion)
        {
            BaseStat stat = _stats.Find(x => x._id == entityId);
            if (stat == null) return null;

            Entity entity = ReturnEntity(entityId, ownershipId, clientId, pos, quaternion);
            if (entity == null) return null;

            stat.Initialize(entity);
            return entity;
        }

        Entity Instantiate(int entityId, int ownershipId, int clientId, Vector3 pos, Quaternion quaternion, float duration)
        {
            BaseStat stat = _stats.Find(x => x._id == entityId);
            if (stat == null) return null;

            Entity entity = ReturnEntity(entityId, ownershipId, clientId, pos, quaternion);
            if (entity == null) return null;

            entity.ResetDelayAfterSpawn(duration);
            stat.Initialize(entity);
            return entity;
        }

        void SpawnClockUI(Vector3 pos, float duration)
        {
            ClockUI clockUI = Instantiate(_clockUIPrefab);
            clockUI.Initialize(pos, duration);
        }

        public Entity Spawn(int entityId, int ownershipId, int clientId, Vector3 pos)
        {
            return Instantiate(entityId, ownershipId, clientId, pos, Quaternion.identity);
        }

        public Entity Spawn(int entityId, int ownershipId, int clientId, Vector3 pos, LandFormation myFormation)
        {
            if (LandFormation.C == myFormation)
            {
                Quaternion reverse = Quaternion.Euler(new Vector3(0, 180, 0));
                return Instantiate(entityId, ownershipId, clientId, pos, reverse);
            }
            else
            {
                return Instantiate(entityId, ownershipId, clientId, pos, Quaternion.identity);
            }
        }

        public Entity Spawn(int entityId, int ownershipId, int clientId, Vector3 pos, float duration)
        {
            SpawnClockUI(pos, duration);
            return Instantiate(entityId, ownershipId, clientId, pos, Quaternion.identity, duration);
        }

        public Entity Spawn(int entityId, int ownershipId, int clientId, Vector3 pos, float duration, Quaternion quaternion)
        {
            SpawnClockUI(pos, duration);
            return Instantiate(entityId, ownershipId, clientId, pos, quaternion, duration);
        }

        public Entity[] Spawn(int[] entityIds, int playerId, int clientId, Vector3 pos, Vector3[] offsets, float duration)
        {
            if (entityIds.Length != offsets.Length) return null;
            Entity[] entities = new Entity[entityIds.Length];

            for (int i = 0; i < entityIds.Length; i++)
                entities[i] = Instantiate(entityIds[i], playerId, clientId, pos + offsets[i], Quaternion.identity, duration);

            SpawnClockUI(pos, duration);

            return entities;
        }

        public Entity[] Spawn(int[] entityIds, int ownershipId, int clientId, Vector3 pos, Vector3[] offsets, float duration, Quaternion quaternion)
        {
            if (entityIds.Length != offsets.Length) return null;
            Entity[] entities = new Entity[entityIds.Length];

            for (int i = 0; i < entityIds.Length; i++)
                entities[i] = Instantiate(entityIds[i], ownershipId, clientId, pos + offsets[i], quaternion, duration);

            SpawnClockUI(pos, duration);
            return entities;
        }

        public Entity[] Spawn(int entityId, int ownershipId, int clientId, Vector3 pos, Vector3[] offsets, Quaternion quaternion)
        {
            Entity[] entities = new Entity[offsets.Length];

            for (int i = 0; i < offsets.Length; i++)
                entities[i] = Instantiate(entityId, ownershipId, clientId, pos + offsets[i], quaternion);

            return entities;
        }
    }
}
