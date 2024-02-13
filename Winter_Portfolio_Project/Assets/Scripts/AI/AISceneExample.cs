using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.GRID;
using WPP.DRAWING;
using WPP.AI.SPAWNER;
using WPP.CAMERA;
using WPP.AI.STAT;
using WPP.ClientInfo;

namespace WPP.AI
{
    public class AISceneExample : MonoBehaviour
    {
        [SerializeField] Spawner _spawner;

        [SerializeField] GridController _gridController;
        [SerializeField] CameraController _cameraController;

        [SerializeField] Transform _arrowEndPosition;
        
        /// <summary>
        /// 플레이어1 ID
        /// </summary>
        [SerializeField] int _player1Id = 0;

        /// <summary>
        /// 플레이어2 ID
        /// </summary>
        [SerializeField] int _player2Id = 1;

        /// <summary>
        /// 현재 클라이언트를 조작하고 있는 플레이어 ID
        /// </summary>
        [SerializeField] int _clientId = 1;

        LandFormation ReturnLandFormation(int playerId)
        {
            if (playerId == 0) return LandFormation.C;
            else return LandFormation.R;
        }

        private void Start()
        {
            // 아이디에 따라 지형 초기화 적용시켜주기
            // 여기서 카메라도 돌리기

            // 추가로 쉐이더도 좀 만져서 타워 색도 바꿔줘보자
            // 자기 타워면 

            // 여기서 본인 지형 초기화


            LandFormation landFormation = ReturnLandFormation(_clientId);

            //_cameraController.Rotate(landFormation);
            _gridController.Initialize(landFormation);
            SpawnTower();
        }

        void SpawnTower()
        {
            // 스폰은 position 이용해서 하고 이걸 기반으로 그리드를 찾는 방식을 적용해보자
            // 그리드는 position을 반올림해서 찾자

            //ClientData.Instance().towers

            // Position은 이거 참고해서 스폰시키면 됨

            //Entity cRightTower = _spawner.Spawn(10, _player1Id, _clientId, new Vector3(10, 1, 6), LandFormation.C);
            //cRightTower.IsLeft(false);

            //Entity cLeftTower = _spawner.Spawn(10, _player1Id, _clientId, new Vector3(-1, 1, 6), LandFormation.C);
            //cLeftTower.IsLeft(true);

            //_spawner.Spawn(11, _player1Id, _clientId, new Vector3(4.51f, 1, 9.51f), LandFormation.C);

            //Entity rRightTower = _spawner.Spawn(10, _player2Id, _clientId, new Vector3(10, 1, -14), LandFormation.R);
            //rRightTower.IsLeft(false);

            //Entity rLeftTower = _spawner.Spawn(10, _player2Id, _clientId, new Vector3(-1, 1, -14), LandFormation.R);
            //rLeftTower.IsLeft(true);

            //_spawner.Spawn(11, _player2Id, _clientId, new Vector3(4.51f, 1, -17.49f), LandFormation.R);
        }

        private void Update()
        {
            //_gridSelecter를 이용해서 여기서 Update 돌려서 범위 구해주기
            // 그 위치에 Entity 스폰 적용해보기

            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    OffsetRect offsetRect1 = new OffsetRect(0, 0, 0, 0);
            //    //OffsetRect offsetRect2 = new OffsetRect(1, 1, 1, 1);

            //    //_gridController.OnSelect(offsetRect1);
            //    _gridController.OnSelect(offsetRect1);
            //}
            //else if(Input.GetKeyDown(KeyCode.S))
            //{
            //    float radius = 3f;
            //    _gridController.OnSelect(radius);
            //}
            //else if (Input.GetKeyDown(KeyCode.D))
            //{
            //    _gridController.OnCancelSelect();
            //}
            //else if (Input.GetKeyDown(KeyCode.F))
            //{
            //    _spawner.Spawn("arrow", new ProjectileMagicStat(-1, -1, "arrow", 3, 3f, 300, 25f), 1, _arrowEndPosition.position);
            //}
            //else if(Input.GetKeyDown(KeyCode.D))
            //{
            //    int[] ids = new int[3] { 0, 1, 2 };
            //    Vector3[] offsets = new Vector3[3] { new Vector3(1, 0, -1), new Vector3(0, 0, 1), new Vector3(-1, 0, -1) };
            //    float duration = 3f;

            //    _gridController.FSM.OnPlant(ids, _player2Id, _clientId, offsets, duration);
            //}
            //else if (Input.GetKeyDown(KeyCode.F))
            //{
            //    _gridController.FSM.OnCancelSelect();
            //}


            //else if (Input.GetKeyDown(KeyCode.I))
            //{
            //    int id = 9;
            //    float duration = 1.5f;

            //    _gridController.FSM.OnPlant(id, _player2Id, _clientId, duration);
            //}
            //else if (Input.GetKeyDown(KeyCode.O))
            //{
            //    int id = 4;
            //    float duration = 1.5f;

            //    _gridController.FSM.OnPlant(id, _player2Id, _clientId, duration);
            //}
        }
    }
}
