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
        private static ClientManager _instance;
        private object _managerLockObj;
        public string serverIP = "192.168.1.81";
        public short serverPort = 8000;

        public static ClientManager Instance()
        {
            return _instance;
        }
        void Awake()
        {
            _instance = this;
            _managerLockObj = new object();
            //ClientTCP.Instance().ConnectServer();
            //PacketHandler.Instance().InitializePacketHandler();
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
                lock (_managerLockObj)
                {
                    PacketHandler.Instance().HandlePacket(PacketHandler.Instance().packetQueue.Dequeue());

                }
            }

            if (PacketHandler.Instance().inGamePacketQueue.Count > 0)
            {
                lock (_managerLockObj)
                {
                    PacketHandler.Instance().HandleInGamePacket(PacketHandler.Instance().inGamePacketQueue.Dequeue());

                }
            }
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

