using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.ClientInfo;
using WPP.AI.GRID;

namespace WPP.CAMERA
{
    public class CameraController : MonoBehaviour
    {
        private void Start() => Rotate(ClientData.Instance().LandFormation);

        public void Rotate(LandFormation landFormation)
        {
            if(landFormation == LandFormation.C) transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            else transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
}
