using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using WPP.Network;
using WPP.ClientInfo;
using Newtonsoft.Json;

namespace WPP
{
    public class ClientManager : MonoBehaviour
    {
        private object _managerLockObj;
        void Awake()
        {
            _managerLockObj = new object();
            ClientTCP.Instance().ConnectServer();
            PacketHandler.Instance().InitializePacketHandler();
        }

        void Update()
        {
            lock (_managerLockObj)
            {
                if (PacketHandler.Instance().packetQueue.Count > 0)
                {
                    PacketHandler.Instance().HandlePacket(PacketHandler.Instance().packetQueue.Dequeue());
                }
            }
        }

        private void OnApplicationQuit()
        {
            // 덱이 아직 설정 되지 않았을 경우 (회원가입만 하고 게임을 종료한 경우)
            if(ClientData.Instance().decks.decks.Count == 0)
            {
                return;
            }
            ByteBuffer decksBuffer = new ByteBuffer();

            // 클라이언트 덱 정보 JSON화
            string decksString = JsonConvert.SerializeObject(ClientData.Instance().decks);
            decksBuffer.WriteString(decksString);
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_REQUEST_UPDATE_DECKS, decksBuffer.ToArray());
        }
    }
}

