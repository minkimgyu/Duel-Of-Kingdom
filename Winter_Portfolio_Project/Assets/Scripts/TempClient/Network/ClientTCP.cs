using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

namespace WPP.Network
{
    class ClientTCP
    {
        private static ClientTCP _instance = null;
        public TcpClient clntSock { get; set; }
        public TcpClient peerSock { get; set; }
        public NetworkStream serverStream { get; set; }
        public NetworkStream P2Pstream { get; set; }

        private byte[] _receivedPacket;
        public ByteBuffer buffer { get; set; }

        public Queue<byte[]> packetToHandle { get; set; }

        public static ClientTCP Instance()
        {
            if (_instance == null)
            {
                _instance = new ClientTCP();
            }
            return _instance;
        }
        public ClientTCP()
        {
            clntSock = new TcpClient();
            clntSock.NoDelay = true;
            clntSock.ReceiveBufferSize = 4096;
            clntSock.SendBufferSize = 4096;

            peerSock = new TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            peerSock.NoDelay = true;
            peerSock.ReceiveBufferSize = 4096;
            peerSock.SendBufferSize = 4096;

            _receivedPacket = new byte[4096];
            buffer = null;

            packetToHandle = new Queue<byte[]>();
        }
        public void SendDataToServer(Client_PacketTagPackages tag, byte[] data = null)
        {
            try
            {
                ByteBuffer buffer = new ByteBuffer();
                // write packet length
                if (data != null)
                    buffer.WriteInteger(data.Length + 8);
                else
                    buffer.WriteInteger(8);
                // write packet tag
                buffer.WriteInteger((int)tag);

                if (data != null)
                    buffer.WriteBytes(data);
                serverStream.BeginWrite(buffer.ToArray(), 0, buffer.Count(), null, null);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ConnectServer(IPAddress serverIP, int serverPort)
        {
            clntSock.BeginConnect(serverIP, serverPort, ConnectServerCallBack, null);
        }
        private void ConnectServerCallBack(IAsyncResult result)
        {
            clntSock.EndConnect(result);

            if (clntSock.Connected == false)
            {
                Debug.Log("Connection Error");
                return;
            }

            try
            {
                serverStream = clntSock.GetStream();
                Debug.Log("Connected to server");

                serverStream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceivFromServerCallbck, null);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return;
            }
        }
        private void ReceivFromServerCallbck(IAsyncResult result)
        {
            try
            {
                int bytesReceived = serverStream.EndRead(result);

                if (bytesReceived <= 0)
                {
                    CloseServerConnection();
                    return;
                }

                byte[] dataToHandle = new byte[bytesReceived];
                Buffer.BlockCopy(_receivedPacket, 0, dataToHandle, 0, bytesReceived);

                packetToHandle.Enqueue(dataToHandle);

                serverStream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceivFromServerCallbck, null);
                return;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void SendDataToPeer(Peer_PacketTagPackages tag, byte[] data = null)
        {
            try
            {
                ByteBuffer buffer = new ByteBuffer();
                // write packet length
                if (data != null)
                    buffer.WriteInteger(data.Length + 8);
                else
                    buffer.WriteInteger(8);
                // write packet tag
                buffer.WriteInteger((int)tag);

                if (data != null)
                    buffer.WriteBytes(data);
                P2Pstream.BeginWrite(buffer.ToArray(), 0, buffer.Count(), null, null);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ConnectPeer(IPEndPoint ep)
        {
            peerSock.BeginConnect(ep.Address, ep.Port, ConnectPeerCallBack, null);
        }

        private void ConnectPeerCallBack(IAsyncResult result)
        {
            peerSock.EndConnect(result);

            if (peerSock.Connected == false)
            {
                Debug.Log("Connection Error");
                return;
            }

            try
            {
                P2Pstream = peerSock.GetStream();
                Debug.Log("Connected to peer");

                P2Pstream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceivFromPeerCallbck, null);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return;
            }
        }

        private void ReceivFromPeerCallbck(IAsyncResult result)
        {
            try
            {
                int bytesReceived = P2Pstream.EndRead(result);

                if (bytesReceived <= 0)
                {
                    ClosePeerConnection();
                    return;
                }

                byte[] dataToHandle = new byte[bytesReceived];
                Buffer.BlockCopy(_receivedPacket, 0, dataToHandle, 0, bytesReceived);

                packetToHandle.Enqueue(dataToHandle);

                P2Pstream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceivFromPeerCallbck, null);
                return;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        private void CloseServerConnection()
        {
            Console.WriteLine("{0} closed connection", clntSock.Client.RemoteEndPoint);
            serverStream.Close();
            clntSock.Close();
            clntSock = null;
            return;
        }
        private void ClosePeerConnection()
        {
            Console.WriteLine("{0} closed connection", peerSock.Client.RemoteEndPoint);
            P2Pstream.Close();
            peerSock.Close();
            peerSock = null;
            return;
        }

    }
}