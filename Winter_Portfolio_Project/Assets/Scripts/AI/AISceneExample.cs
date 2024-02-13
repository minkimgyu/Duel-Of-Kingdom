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
        /// �÷��̾�1 ID
        /// </summary>
        [SerializeField] int _player1Id = 0;

        /// <summary>
        /// �÷��̾�2 ID
        /// </summary>
        [SerializeField] int _player2Id = 1;

        /// <summary>
        /// ���� Ŭ���̾�Ʈ�� �����ϰ� �ִ� �÷��̾� ID
        /// </summary>
        [SerializeField] int _clientId = 1;

        LandFormation ReturnLandFormation(int playerId)
        {
            if (playerId == 0) return LandFormation.C;
            else return LandFormation.R;
        }

        private void Start()
        {
            // ���̵� ���� ���� �ʱ�ȭ ��������ֱ�
            // ���⼭ ī�޶� ������

            // �߰��� ���̴��� �� ������ Ÿ�� ���� �ٲ��ຸ��
            // �ڱ� Ÿ���� 

            // ���⼭ ���� ���� �ʱ�ȭ


            LandFormation landFormation = ReturnLandFormation(_clientId);

            //_cameraController.Rotate(landFormation);
            _gridController.Initialize(landFormation);
            SpawnTower();
        }

        void SpawnTower()
        {
            // ������ position �̿��ؼ� �ϰ� �̰� ������� �׸��带 ã�� ����� �����غ���
            // �׸���� position�� �ݿø��ؼ� ã��

            //ClientData.Instance().towers

            // Position�� �̰� �����ؼ� ������Ű�� ��

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
            //_gridSelecter�� �̿��ؼ� ���⼭ Update ������ ���� �����ֱ�
            // �� ��ġ�� Entity ���� �����غ���

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
