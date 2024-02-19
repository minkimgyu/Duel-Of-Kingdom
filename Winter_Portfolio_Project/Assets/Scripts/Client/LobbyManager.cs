using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using WPP.Network;
using System.Net;
using WPP.AI.SPAWNER;
using WPP.ClientInfo;
using WPP.SOUND;

namespace WPP
{
    public class LobbyManager : MonoBehaviour
    {
        public Button battleButton;

        private void Awake()
        {
            battleButton.onClick.AddListener(() => PlayButtonOnClick());
        }

        public void PlayButtonOnClick()
        {
            SceneManager.LoadScene("Loading");
            /*
             로딩 중 게임을 종료 하였을 때의 예외처리 필요
            - 생성된 방 처리
             */

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteEndPoint(ClientTCP.Instance().peerSockPrivateEP);
            buffer.WriteEndPoint(ClientTCP.Instance().peerSockPublicEP);

            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_REQUEST_ENTER_ROOM, buffer.ToArray());
        }
    }

}