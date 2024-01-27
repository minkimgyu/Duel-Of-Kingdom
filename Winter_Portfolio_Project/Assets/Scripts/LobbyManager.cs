using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using WPP.Network;
using System.Net;

namespace WPP
{
    public class LobbyManager : MonoBehaviour
    {
        public Button playButton;

        private void Awake()
        {
            playButton.onClick.AddListener(() => PlayButtonOnClick());
        }

        public void PlayButtonOnClick()
        {
            SceneManager.LoadScene("Loading");
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteEndPoint(ClientTCP.Instance().peerSock.Client.LocalEndPoint as IPEndPoint);
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_REQUEST_ENTER_ROOM, buffer.ToArray());
        }
    }

}