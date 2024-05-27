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
using UnityEngine.SceneManagement;

namespace WPP
{
    public class ClientManager : MonoBehaviour
    {
        private object _managerLockObj;
        public DateTime gameStartTime;
        private static ClientManager _instance = null;
        public static ClientManager Instance { get { return _instance; } } 

        void Awake()
        {
            _instance = this;
            _managerLockObj = new object();
        }

        public void ConnectServer()
        {
            ClientTCP.Instance().ConnectServer();
            PacketHandler.Instance().InitializePacketHandler();
        }

        void Update()
        {
            if (PacketHandler.Instance().packetQueue.Count > 0)
            {
                lock (ClientTCP.Instance().PacketQueueLockObject)
                {
                    byte[] packet = PacketHandler.Instance().packetQueue.Dequeue();
                    PacketHandler.Instance().HandlePacket(packet);
                }
            }
            /*
            if (PacketHandler.Instance().inGamePacketQueue.Count > 0)
            {
                lock (_managerLockObj)
                {
                    PacketHandler.Instance().HandleInGamePacket(PacketHandler.Instance().inGamePacketQueue.Dequeue());

                }
            }
            */
        }

        private void OnApplicationQuit()
        {
            // ���� ���� ���� ���� �ʾ��� ��� (ȸ�����Ը� �ϰ� ������ ������ ���)
            if(ClientData.Instance().decks.decks.Count == 0)
            {
                return;
            }
            ByteBuffer decksBuffer = new ByteBuffer();

            // Ŭ���̾�Ʈ �� ���� JSONȭ
            string decksString = JsonConvert.SerializeObject(ClientData.Instance().decks);
            decksBuffer.WriteString(decksString);
            ClientTCP.Instance().SendDataToServer(Client_PacketTagPackages.C_CLOSE_CONNECTION, decksBuffer.ToArray());
        }
    }
}

