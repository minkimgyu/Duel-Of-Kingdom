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
        void Awake()
        {
            ClientTCP.Instance().ConnectServer();
            PacketHandler.Instance().InitializePacketHandler();
        }

        void Update()
        {
            if (ClientTCP.Instance().packetQueue.Count > 0)
            {
                PacketHandler.Instance().HandlePacket(ClientTCP.Instance().packetQueue.Dequeue());
            }
        }

    }
}

