using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.GRID;
using WPP.DRAWING;
using WPP.AI.SPAWNER;

namespace WPP.AI
{
    public class AISceneExample : MonoBehaviour
    {
        [SerializeField] FillComponent _gridFiller;
        [SerializeField] SpawnAreaDrawer _spawnRect;

        [SerializeField] SelectComponent _gridSelecter;

        [SerializeField] Spawner _spawner;

        [SerializeField] int _player1Id = 1;
        [SerializeField] int _player2Id = 2;

        [SerializeField] int _spawnId = 1;

        private void Start()
        {
            SpawnTower();
        }

        void SpawnTower()
        {
            // 스폰은 position 이용해서 하고 이걸 기반으로 그리드를 찾는 방식을 적용해보자
            // 그리드는 position을 반올림해서 찾자
            _spawner.Spawn(10, _player1Id, new Vector3(10, 1, 6), LandFormation.C);
            _spawner.Spawn(10, _player1Id, new Vector3(-1, 1, 6), LandFormation.C);
            _spawner.Spawn(11, _player1Id, new Vector3(4.51f, 1, 9.51f), LandFormation.C);

            _spawner.Spawn(10, _player2Id, new Vector3(10, 1, -14), LandFormation.R);
            _spawner.Spawn(10, _player2Id, new Vector3(-1, 1, -14), LandFormation.R);
            _spawner.Spawn(11, _player2Id, new Vector3(4.51f, 1, -17.49f), LandFormation.R);
        }

        void SpawnEntity(int id, float duration)
        {
            Vector3 spawnPos = _gridSelecter.ReturnSpawnPoint();
            //Debug.Log(spawnPos);
            _spawner.Spawn(id, _spawnId, spawnPos, duration);
        }

        void SpawnEntity(int[] ids, Vector3[] offsets, float duration)
        {
            Vector3 spawnPos = _gridSelecter.ReturnSpawnPoint();
            _spawner.Spawn(ids, _spawnId, spawnPos, offsets, duration);
        }

        private void Update()
        {
            //_gridSelecter를 이용해서 여기서 Update 돌려서 범위 구해주기
            // 그 위치에 Entity 스폰 적용해보기

            _gridSelecter.SelectGrid();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SpawnEntity(1, 1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                int[] ids = new int[3] { 0, 1, 2 };
                Vector3[] offsets = new Vector3[3] 
                { 
                    new Vector3(1, 0, -1),
                    new Vector3(0, 0, 1),
                    new Vector3(-1, 0, -1),
                };

                SpawnEntity(ids, offsets, 1.5f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SpawnEntity(9, 2.5f);
            }

            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    _gridFiller.OnLandFormationAssigned(LandFormation.C);
            //    _spawnRect.Erase();

            //    _spawnRect.Draw();
            //}
            //else if (Input.GetKeyDown(KeyCode.S))
            //{
            //    _gridFiller.OnTowerConditionChanged(LandFormation.C, TowerCondition.LeftDestroy);
            //    _spawnRect.Erase();

            //    _spawnRect.Draw();
            //}
            //else if (Input.GetKeyDown(KeyCode.D))
            //{
            //    _gridFiller.OnTowerConditionChanged(LandFormation.C, TowerCondition.RightDestroy);
            //    _spawnRect.Erase();

            //    _spawnRect.Draw();
            //}


            //if (Input.GetKeyDown(KeyCode.H))
            //{
            //    _gridFiller.OnLandFormationAssigned(LandFormation.R);
            //    _spawnRect.Erase();

            //    _spawnRect.Draw();
            //}
            //else if (Input.GetKeyDown(KeyCode.J))
            //{
            //    _gridFiller.OnTowerConditionChanged(LandFormation.R, TowerCondition.LeftDestroy);
            //    _spawnRect.Erase();

            //    _spawnRect.Draw();
            //}
            //else if (Input.GetKeyDown(KeyCode.K))
            //{
            //    _gridFiller.OnTowerConditionChanged(LandFormation.R, TowerCondition.RightDestroy);
            //    _spawnRect.Erase();

            //    _spawnRect.Draw();
            //}
        }
    }
}
