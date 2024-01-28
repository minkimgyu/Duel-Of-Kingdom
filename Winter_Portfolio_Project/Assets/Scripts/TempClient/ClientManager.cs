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

namespace WPP
{
    public class ClientManager : MonoBehaviour
    {
        [SerializeField] private string _serverIP;
        [SerializeField] private int _serverPort;

        void Awake()
        {
            ClientTCP.Instance().ConnectServer(IPAddress.Parse(_serverIP), _serverPort);
            PacketHandler.Instance().InitializePacketHandler();
        }

        void Update()
        {
            if (ClientTCP.Instance().packetToHandle.Count > 0)
            {
                PacketHandler.Instance().HandlePacket(ClientTCP.Instance().packetToHandle.Dequeue());
            }
        }
    }
}

