using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.JSON;
using WPP.AI.STAT;
using WPP.AI.UI;
using WPP.AI.GRID;
using WPP.ClientInfo;
using WPP.DeckManagement;
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

        Quaternion ReturnQuaternionUsingLandFormation(int playerId)
        {
            // playerId가 0이면 C지형으로 지정
            // 1이면 R지형으로 지정
            // --> 이 둘을 바탕으로 rotation 변수를 지정해준다.

            if (playerId == 0) return Quaternion.Euler(new Vector3(0, 180, 0));
            else return Quaternion.Euler(new Vector3(0, 0, 0));
        }

        // entityId 이거를 string로 해서 받기
        Entity ReturnEntity(string name, int ownershipId, Vector2 pos)
        {
            Entity entity = _entityPrefabs.Find(x => x.Name == name);
            if (entity == null) return null;

            Quaternion rotation = ReturnQuaternionUsingLandFormation(ownershipId);
            Entity spawnedEntity = Instantiate(entity, pos, rotation);

            HpContainerUI hpContainer = Instantiate(_hpContainerUIPrefab);

            int clientId = ClientData.Instance().player_id_in_game; // 본인 클라이언트 아이디를 받아와서 넣어준다.

            spawnedEntity.AttachHpBar(hpContainer);
            spawnedEntity.ResetPlayerId(ownershipId, clientId);
            // 여기에 대기 시간을 추가해준다.

            return spawnedEntity;
        }

        void SpawnClockUI(Vector3 pos, float duration)
        {
            ClockUI clockUI = Instantiate(_clockUIPrefab);
            clockUI.Initialize(pos, duration);
        }

        // 여기 부분에서 시계 모양 아이콘 생성시켜서 준비 시간 보여주기
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

        Entity Instantiate(string name, int level, int ownershipId, Vector2 pos)
        {
            // name과 level이 같은 경우를 찾아서 스폰
            // 추후에 offset을 추가로 보고 적용시켜야할 수도 있기 때문에 그것도 고려해보기
            BaseStat stat = _stats.Find(x => x._name == name && x._level == level);
            if (stat == null) return null;

            Entity entity = ReturnEntity(name, ownershipId, pos);
            if (entity == null) return null;

            stat.Initialize(entity);
            return entity;
        }

        Entity Instantiate(string name, int level, float duration, int ownershipId, Vector2 pos)
        {
            // name과 level이 같은 경우를 찾아서 스폰
            // 추후에 offset을 추가로 보고 적용시켜야할 수도 있기 때문에 그것도 고려해보기
            BaseStat stat = _stats.Find(x => x._name == name && x._level == level);
            if (stat == null) return null;

            Entity entity = ReturnEntity(name, ownershipId, pos);
            if (entity == null) return null;

            entity.ResetDelayAfterSpawn(duration);
            stat.Initialize(entity);
            return entity;
        }

        Entity Instantiate(EntitySpawnData entitySpawnData, int level, float duration, int ownershipId, Vector2 pos)
        {

            // name과 level이 같은 경우를 찾아서 스폰
            // 추후에 offset을 추가로 보고 적용시켜야할 수도 있기 때문에 그것도 고려해보기
            BaseStat stat = _stats.Find(x => x._name == entitySpawnData.entityId && x._level == level);
            if (stat == null) return null;

            Entity entity = ReturnEntity(entitySpawnData.entityId, ownershipId, pos);
            if (entity == null) return null;

            entity.ResetDelayAfterSpawn(duration);
            stat.Initialize(entity);
            return entity;
        }

        /// <summary>
        /// 카드를 사용해서 스폰시키는 경우
        /// </summary>
        public Entity[] Spawn(Card card, int level, int ownershipId, Vector3 pos)
        {
            Entity[] spawnedEntities = new Entity[card.entities.Count];

            for (int i = 0; i < card.entities.Count; i++)
            {
                spawnedEntities[i] = Instantiate(card.entities[i], level, card.duration, ownershipId, pos);
            }

            return spawnedEntities;
        }


        /// <summary>
        /// 카드로 스폰시키지 않는 경우 ex) 타워
        /// duration이 있는 경우 ex) 바바리안 오두막
        /// </summary>
        public Entity Spawn(string name, int level, float duration, int ownershipId, Vector3 pos)
        {
            return Instantiate(name, level, duration, ownershipId, pos);
        }

        /// <summary>
        /// 카드로 스폰시키지 않는 경우 ex) 타워
        /// duration이 없는 경우 ex) 킹, 프린세스 타워
        /// </summary>
        public Entity Spawn(string name, int level, int ownershipId, Vector3 pos)
        {
            return Instantiate(name, level, ownershipId, pos);
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
