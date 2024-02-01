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

        [SerializeField] int _myPlayerId;

        JsonParser _jsonParser;

        private void Awake()
        {
            _jsonParser = GetComponent<JsonParser>();
            _stats = _jsonParser.Load();
        }

        // 공통되는 부분
        Entity ReturnEntity(int entityId, int playerId, Vector3 pos, Quaternion quaternion)
        {
            Entity entity = _entityPrefabs.Find(x => x.Id == entityId);
            if (entity == null) return null;

            Entity spawnedEntity = Instantiate(entity, pos, quaternion);

            HpContainerUI hpContainer = Instantiate(_hpContainerUIPrefab);

            spawnedEntity.AttachHpBar(hpContainer);
            spawnedEntity.ResetPlayerId(playerId);
            // 여기에 대기 시간을 추가해준다.

            return spawnedEntity;
        }

        // 여기 부분에서 시계 모양 아이콘 생성시켜서 준비 시간 보여주기
        void Instantiate(int entityId, int playerId, Vector3 pos, Quaternion quaternion)
        {
            BaseStat stat = _stats.Find(x => x._id == entityId);
            if (stat == null) return;

            Entity entity = ReturnEntity(entityId, playerId, pos, quaternion);
            if (entity == null) return;

            stat.Initialize(entity);
        }

        void Instantiate(int entityId, int playerId, Vector3 pos, Quaternion quaternion, float duration)
        {
            BaseStat stat = _stats.Find(x => x._id == entityId);
            if (stat == null) return;

            Entity entity = ReturnEntity(entityId, playerId, pos, quaternion);
            if (entity == null) return;

            entity.ResetDelayAfterSpawn(duration);
            stat.Initialize(entity);
        }

        void SpawnClockUI(Vector3 pos, float duration)
        {
            ClockUI clockUI = Instantiate(_clockUIPrefab);
            clockUI.Initialize(pos, duration);
        }

        public void Spawn(int entityId, int playerId, Vector3 pos)
        {
            Instantiate(entityId, playerId, pos, Quaternion.identity);
        }

        public void Spawn(int entityId, int playerId, Vector3 pos, LandFormation myFormation)
        {
            if (LandFormation.C == myFormation)
            {
                Quaternion reverse = Quaternion.Euler(new Vector3(0, 180, 0));
                Instantiate(entityId, playerId, pos, reverse);
            }
            else
            {
                Instantiate(entityId, playerId, pos, Quaternion.identity);
            }
        }

        public void Spawn(int entityId, int playerId, Vector3 pos, float duration)
        {
            Instantiate(entityId, playerId, pos, Quaternion.identity, duration);
            SpawnClockUI(pos, duration);
        }

        public void Spawn(int entityId, int playerId, Vector3 pos, float duration, Quaternion quaternion)
        {
            Instantiate(entityId, playerId, pos, quaternion, duration);
            SpawnClockUI(pos, duration);
        }

        public void Spawn(int entityId, int playerId, Vector3 pos, float duration, LandFormation myFormation)
        {
            if (LandFormation.C == myFormation)
            {
                Quaternion reverse = Quaternion.Euler(new Vector3(0, 180, 0));
                Instantiate(entityId, playerId, pos, reverse, duration);
            }
            else
            {
                Instantiate(entityId, playerId, pos, Quaternion.identity, duration);
            }

            SpawnClockUI(pos, duration);
        }

        public void Spawn(int[] entityIds, int playerId, Vector3 pos, Vector3[] offsets, float duration)
        {
            if (entityIds.Length != offsets.Length) return;

            for (int i = 0; i < entityIds.Length; i++)
                Instantiate(entityIds[i], playerId, pos + offsets[i], Quaternion.identity, duration);

            SpawnClockUI(pos, duration);
        }

        public void Spawn(int entityId, int playerId, Vector3 pos, Vector3[] offsets, Quaternion quaternion)
        {
            for (int i = 0; i < offsets.Length; i++)
                Instantiate(entityId, playerId, pos + offsets[i], quaternion);
        }
    }
}
