using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.GRID;
using WPP.AI.SPAWNER;
using WPP.CAMERA;
using WPP.ClientInfo.Card;
using WPP.DeckManagement;

namespace WPP.Batte.Example
{
    public class BattleSceneExample : MonoBehaviour
    {
        [SerializeField] Spawner _spawner;
        [SerializeField] GridController _gridController;
        [SerializeField] CameraController _cameraController;

        [SerializeField] int _player1Id = 0;
        [SerializeField] int _player2Id = 1;
        [SerializeField] int _clientId = 1;

        public void Initialize()
        {
            LandFormation landFormation = ReturnLandFormation(_clientId);
            _gridController.Initialize(landFormation);

            SpawnTower();

            LandFormation ReturnLandFormation(int playerId)
            {
                if (playerId == 0) return LandFormation.C;
                else return LandFormation.R;
            }
        }

        void SpawnTower()
        {



            _spawner.SpawnTower(_player1Id, new Vector3(4.51f, 1, 9.51f), new Vector3(-1, 1, 6), new Vector3(10, 1, 6));
            _spawner.SpawnTower(_player2Id, new Vector3(4.51f, 1, -17.49f), new Vector3(-1, 1, -14), new Vector3(10, 1, -14));


            //ClientData.Instance().towers

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
    }
}
