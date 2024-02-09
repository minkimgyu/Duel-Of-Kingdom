using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.ClientInfo;
using WPP.AI.GRID;

namespace WPP.CAMERA
{
    public class CameraController : MonoBehaviour
    {
        //private void Start() => RotateCamera();

        public void Rotate(LandFormation landFormation)
        {
            //int clientId = ClientData.Instance().player_id_in_game; // ���� Ŭ���̾�Ʈ ���̵� �޾ƿͼ� �־��ش�.
            if(landFormation == LandFormation.C) Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            else Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
}
