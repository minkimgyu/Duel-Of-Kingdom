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
            if (entity == null) return; // 오브젝트가 없다면 return;

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
            // playerId가 0이면 C지형으로 지정
            // 1이면 R지형으로 지정
            // --> 이 둘을 바탕으로 rotation 변수를 지정해준다.

            if (playerId == 0) return Quaternion.Euler(new Vector3(0, 180, 0));
            else return Quaternion.Euler(new Vector3(0, 0, 0));
        }

        Vector3 ReturnMagicProjectileStartPoint(int playerId)
        {
            // playerId가 0이면 C지형으로 지정
            // 1이면 R지형으로 지정
            // --> 이 둘을 바탕으로 rotation 변수를 지정해준다.

            if (playerId == 0) return _cMagicProjectileStartPoint.position;
            else return _rMagicProjectileStartPoint.position;
        }

        string ReturnNetworkId(int ownershipId)
        {
            return ownershipId.ToString() + _spawnCount.ToString();
        }

        // entityId 이거를 string로 해서 받기
        Entity ReturnEntity(string name, int ownershipId, Vector3 pos)
        {
            Entity entity = _entityPrefabs.Find(x => x.Name == name);
            if (entity == null) return null;

            _spawnCount++; // 여기서 스폰 카운트를 올려준다.
            string networkId = ReturnNetworkId(ownershipId); // 이거를 생성되는 오브젝트에 부여해야한다 --> 따로 서버를 통해서 넘겨서 받아주기
                                                             // 매개변수로 받아야할 듯?

            int clientId = ClientData.Instance().player_id_in_game; // 본인 클라이언트 아이디를 받아와서 넣어준다.

            // 나중에 여기 변경
            Quaternion rotation = ReturnQuaternionUsingLandFormation(clientId);
            Entity spawnedEntity = Instantiate(entity, pos, rotation);


            // 체력바를 붙일 수 있는 경우에만 진행
            if (spawnedEntity.CanAttachHpBar() == true)
            {
                SpawnHpContainerUI(spawnedEntity);
            }

            spawnedEntity.InitializeListRemover(RemoveFromListInSpawner);
            spawnedEntity.ResetId(ownershipId, clientId, networkId);
            // 여기에 대기 시간을 추가해준다.

            Vector3 magicStartPosition = ReturnMagicProjectileStartPoint(1);
            spawnedEntity.ResetMagicStartPosition(magicStartPosition);
            // 여기에 화살 마법 스폰 위치를 추가해준다.

            _spawnedEntities.Add(spawnedEntity);
            return spawnedEntity;
        }

        // 타워는 1, 나머지 오브젝트는 0.5로 진행
        void SpawnHpContainerUI(Entity entity)
        {
            float scaleRatio = entity.ReturnHpContainerScale();

            HpContainerUI hpContainer = Instantiate(_hpContainerUIPrefab);
            hpContainer.OnScaleChangeRequested(scaleRatio);
            entity.AttachHpBar(hpContainer);
        }

        public void SpawnClockUI(Vector3 pos, float duration)
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

        Entity Instantiate(string name, BaseStat stat, int ownershipId, Vector3 pos)
        {
            Entity entity = ReturnEntity(name, ownershipId, pos);
            if (entity == null) return null;

            stat.ResetData(entity);
            return entity;
        }

        Entity Instantiate(string name, int level, int ownershipId, Vector3 pos)
        {
            //// name과 level이 같은 경우를 찾아서 스폰
            //// 추후에 offset을 추가로 보고 적용시켜야할 수도 있기 때문에 그것도 고려해보기
            //BaseStat stat = _stats.Find(x => x._name == name && x._level == level);
            //if (stat == null) return null;

            //Entity entity = ReturnEntity(name, ownershipId, pos);
            //if (entity == null) return null;

            //stat.ResetData(entity);
            //return entity;

            return null;
        }

        public void Instantiate(int ownershipId, Vector3 kingTowerPos, Vector3 leftPrincessTowerPos, Vector3 rightPrincessTowerPos)
        {
            // name과 level이 같은 경우를 찾아서 스폰
            // 추후에 offset을 추가로 보고 적용시켜야할 수도 있기 때문에 그것도 고려해보기
            //BaseStat stat = _stats.Find(x => x._name == name && x._level == level);
            //if (stat == null) return null;

            //entity.ResetDelayAfterSpawn(duration);
            //stat.ResetData(entity);
            //return entity;
            TowersData towersData = ClientData.Instance().towers;

            Entity kingTower = ReturnEntity("king_tower", ownershipId, kingTowerPos);
            if (kingTower == null) return;
            towersData.kingTower.towerUnit.ResetData(kingTower);

            Entity leftPrincessTower = ReturnEntity("princess_tower", ownershipId, leftPrincessTowerPos);
            if (leftPrincessTower == null) return;
            towersData.leftPrincessTower.towerUnit.ResetData(leftPrincessTower);

            Entity rightPrincessTower = ReturnEntity("princess_tower", ownershipId, rightPrincessTowerPos);
            if (rightPrincessTower == null) return;
            towersData.rightPrincessTower.towerUnit.ResetData(rightPrincessTower);
        }

        public Entity Instantiate(CardData cardData, float duration, int ownershipId, Vector3 pos)
        {
            Entity entity = ReturnEntity(cardData.unit._name, ownershipId, pos);
            if (entity == null) return null;

            entity.ResetDelayAfterSpawn(duration);

            if (cardData.unit == null) return null;
            cardData.unit.ResetData(entity);

            return entity;
        }

        /// <summary>
        /// 카드를 사용해서 스폰시키는 경우
        /// </summary>
        public Entity[] Spawn(Card card, int level, int ownershipId, Vector3 pos)
        {
            CardData cardData = CardCollection.Instance().FindCard(card.id, level);
            float duration = cardData.duration;

            int unitCount = cardData.spawnData.spawnUnitCount;
            SerializableVector2[] offset = cardData.spawnData.spawnOffset;

            Entity[] spawnedEntities = new Entity[unitCount];

            for (int i = 0; i < unitCount; i++)
            {
                spawnedEntities[i] = Instantiate(cardData, duration, ownershipId, pos + new Vector3(offset[i]._x, 0, offset[i]._y));
            }

            SpawnClockUI(pos, duration);
            ClientTCP.Instance().SpawnCard(card, level, ownershipId, pos);
            return spawnedEntities;
        }

        public Entity[] Spawn(string cardId, int level, int ownershipId, Vector3 pos, Vector3[] offsets)
        {
            CardData cardData = CardCollection.Instance().FindCard(cardId, level);
            float duration = cardData.duration;

            for (int i = 0; i < offsets.Length; i++)
            {
                Instantiate(cardData, duration, ownershipId, pos + new Vector3(offsets[i].x, 0, offsets[i].y)); // test
                ClientTCP.Instance().SpawnUnit(cardId, level, ownershipId, pos + offsets[i]);
            }

            return new Entity[0];
        }



        /// <summary>
        /// 카드로 스폰시키지 않는 경우 ex) 타워
        /// duration이 있는 경우 ex) 바바리안 오두막
        /// </summary>
        //public Entity Spawn(string name, int level, float duration, int ownershipId, Vector3 pos)
        //{
        //    SpawnClockUI(pos, duration);
        //    return Instantiate(name, level, duration, ownershipId, pos);
        //}

        /// <summary>
        /// 카드로 스폰시키지 않는 경우 ex) 타워
        /// duration이 없는 경우 ex) 킹, 프린세스 타워
        /// </summary>
        //public Entity Spawn(string name, int level, int ownershipId, Vector3 pos)
        //{
        //    return Instantiate(name, level, ownershipId, pos);
        //}


        public Entity Spawn(string name, BaseStat stat, int ownershipId, Vector3 pos)
        {
            return Instantiate(name, stat, ownershipId, pos);
        }

        public void SpawnTower(int ownershipId, Vector3 kingTowerPos, Vector3 leftPrincessTowerPos, Vector3 rightPrincessTowerPos)
        {
            ClientTCP.Instance().SpawnTower(ownershipId, kingTowerPos, leftPrincessTowerPos, rightPrincessTowerPos);
            Instantiate(ownershipId, kingTowerPos, leftPrincessTowerPos, rightPrincessTowerPos);
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
