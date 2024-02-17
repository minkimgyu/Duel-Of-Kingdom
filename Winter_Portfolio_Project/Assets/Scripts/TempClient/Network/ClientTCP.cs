using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using WPP.DeckManagement;
using WPP.RoomInfo;
using WPP.ClientInfo;
using WPP.DeckManagement.UI;

namespace WPP.Network
{
    class ClientTCP
    {
        private static ClientTCP _instance = null;
        public TcpClient clntSock { get; set; }
        public TcpClient peerSock { get; set; }
        public NetworkStream serverStream { get; set; }
        public NetworkStream holePunchingStream { get; set; }
        public NetworkStream P2Pstream { get; set; }

        private byte[] _receivedPacket;
        public ByteBuffer buffer { get; set; }
        public ByteBuffer inGameBuffer { get; set; }
        public IPEndPoint peerSockPublicEP { get; set; }
        public IPEndPoint peerSockPrivateEP { get; set; }

        public DateTime pingSentTime { get; set; }
        public DateTime pingAnsweredTime { get; set; }
        public TimeSpan rtt { get; set; }

        private object _tcpLockObject;

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
            InitializeClientSock();
            InitializePeerSock();
            _tcpLockObject = new object();
            _receivedPacket = new byte[4096];
            buffer = null;
        }

        public void InitializeClientSock()
        {
            clntSock = new TcpClient();
            clntSock.NoDelay = true;
            clntSock.ReceiveBufferSize = 4096;
            clntSock.SendBufferSize = 4096;
        }

        public void InitializePeerSock()
        {
            try
            {
                peerSock = new TcpClient(new IPEndPoint(IPAddress.Any, 0));
                peerSock.NoDelay = true;
                peerSock.ReceiveBufferSize = 4096;
                peerSock.SendBufferSize = 4096;
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
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
                Debug.Log($"send [packet size: {buffer.Count() + 8}] [tag: {(Client_PacketTagPackages)tag}] packet");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ConnectServer()
        {
            clntSock.BeginConnect(IPAddress.Parse(Constants.SERVER_IP), Constants.SERVER_PORT, ConnectServerCallBack, null);
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
                Debug.Log("Connected to server with clntSock");

                serverStream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceivFromServerCallbck, null);
                SendDataToServer(Client_PacketTagPackages.C_REQUEST_INITIAL_DATA);
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

                lock(_tcpLockObject)
                {
                    PacketHandler.Instance().packetQueue.Enqueue(dataToHandle);
                }

                serverStream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceivFromServerCallbck, null);
                return;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        // functions for hole punching
        public void SendDataToServerForHolePunching(Client_PacketTagPackages tag, byte[] data = null)
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
                holePunchingStream.BeginWrite(buffer.ToArray(), 0, buffer.Count(), null, null);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ConnectServerForHolePunching()
        {
            // for hole punching
            peerSock.BeginConnect(IPAddress.Parse(Constants.SERVER_IP), Constants.SERVER_PORT, ConnectServerCallBackForHolePunching, null);
        }

        private void ConnectServerCallBackForHolePunching(IAsyncResult result)
        {
            peerSock.EndConnect(result);

            if (peerSock.Connected == false)
            {
                Debug.Log("Connection Error");
                return;
            }

            try
            {
                holePunchingStream = peerSock.GetStream();
                Debug.Log("Connected to server with peerSock");
                peerSockPrivateEP = peerSock.Client.LocalEndPoint as IPEndPoint;

                SendDataToServerForHolePunching(Client_PacketTagPackages.C_REQUEST_HOLE_PUNCHING);
                holePunchingStream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceiveFromServerCallbckForHolePunching, null);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return;
            }
        }

        private void ReceiveFromServerCallbckForHolePunching(IAsyncResult result)
        {
            try
            {
                if (holePunchingStream == null)
                    return;

                int bytesReceived = holePunchingStream.EndRead(result);

                if (bytesReceived <= 0)
                {
                    CloseHolePunchingConnection();
                    return;
                }

                byte[] dataToHandle = new byte[bytesReceived];
                Buffer.BlockCopy(_receivedPacket, 0, dataToHandle, 0, bytesReceived);

                lock(_tcpLockObject)
                {
                    PacketHandler.Instance().packetQueue.Enqueue(dataToHandle);
                }

                holePunchingStream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceiveFromServerCallbckForHolePunching, null);
                return;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        // functions for p2p

        public ByteBuffer CreateBufferToSend(Peer_PacketTagPackages tag, byte[] data = null)
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

            return buffer;
        }

        public void SendDataToPeer(Peer_PacketTagPackages tag, byte[] data = null)
        {
            try
            {
                ByteBuffer buffer = CreateBufferToSend(tag, data);

                if(P2Pstream != null)
                {
                    P2Pstream.BeginWrite(buffer.ToArray(), 0, buffer.Count(), null, null);
                }
                else
                {
                    // try to relay
                    Debug.Log("Send packet through relay server");
                    ByteBuffer bufferToRelay = new ByteBuffer();
                    bufferToRelay.WriteInteger(GameRoom.Instance().roomID);
                    bufferToRelay.WriteBytes(buffer.ToArray());
                    SendDataToServer(Client_PacketTagPackages.C_REQUEST_RELAY, bufferToRelay.ToArray());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ConnectPeer(IPEndPoint ep)
        {
            try
            {
                // BeginConnect for a new connection
                peerSock = new TcpClient(peerSockPrivateEP);
                peerSock.BeginConnect(ep.Address, ep.Port, ConnectPeerCallBack, null);
            }
            catch (Exception e)
            {
                Debug.Log($"Error connecting peer: {e.Message}");
            }
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
                    // initialize new peer socket for next matching
                    ClientTCP.Instance().InitializePeerSock();
                    return;
                }

                byte[] dataToHandle = new byte[bytesReceived];
                Buffer.BlockCopy(_receivedPacket, 0, dataToHandle, 0, bytesReceived);

                lock (PacketHandler.Instance().InGamePacketHandlerLockObj)
                {
                    PacketHandler.Instance().inGamePacketQueue.Enqueue(dataToHandle);
                }

                P2Pstream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceivFromPeerCallbck, null);
                return;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        /*
        ByteBuffer GetSpawnBuffer(string cardId, int level, int ownershipId, Vector3 pos)
        {
            ByteBuffer bufferToSend = new ByteBuffer();
            bufferToSend.WriteString(cardId);
            bufferToSend.WriteInteger(level);
            bufferToSend.WriteInteger(ownershipId);
            bufferToSend.WriteVector3(pos);
            return bufferToSend;
        }

        public void SpawnCard(Card card, int level, int ownershipId, Vector3 pos)
        {
            ByteBuffer bufferToSend = GetSpawnBuffer(card.id, level, ownershipId, pos);
            SendDataToPeer(Peer_PacketTagPackages.P_REQUEST_SPAWN_CARD, bufferToSend.ToArray());
        }

        public void SpawnUnit(string cardId, int level, int ownershipId, Vector3 pos)
        {
            ByteBuffer bufferToSend = GetSpawnBuffer(cardId, level, ownershipId, pos);
            SendDataToPeer(Peer_PacketTagPackages.P_REQUEST_SPAWN_UNIT, bufferToSend.ToArray());
        }

        public void SpawnTower(int ownershipId, Vector3 kingTowerPos, Vector3 leftPrincessTowerPos, Vector3 rightPrincessTowerPos)
        {
            ByteBuffer bufferToSend = new ByteBuffer();
            bufferToSend.WriteInteger(ownershipId);
            bufferToSend.WriteVector3(kingTowerPos);
            bufferToSend.WriteVector3(leftPrincessTowerPos);
            bufferToSend.WriteVector3(rightPrincessTowerPos);
            SendDataToPeer(Peer_PacketTagPackages.P_REQUEST_SPAWN_TOWER, bufferToSend.ToArray());
        }
        */
        // functions to close connection
        private void CloseServerConnection()
        {
            Debug.Log($"serverStream: {clntSock.Client.RemoteEndPoint} closed connection");
            serverStream.Close();
            serverStream = null;
            clntSock.Close();
            clntSock = null;
            return;
        }

        public void ClosePeerConnection()
        {
            if (peerSock == null || P2Pstream == null)
                return;
            Debug.Log($"P2Pstream: {peerSock.Client.RemoteEndPoint} closed connection");
            P2Pstream.Close();
            P2Pstream = null;
            peerSock.Close();
            peerSock = null;
            return;
        }

        public void CloseHolePunchingConnection()
        {
            if (holePunchingStream == null)
                return;

            Debug.Log($"holePunchingStream: {peerSock.Client.RemoteEndPoint} closed connection");

            try
            {
                holePunchingStream.Close();
                holePunchingStream = null;
                peerSock.Close();
                peerSock = null;
            }
            catch (Exception e)
            {
                Debug.Log($"Error closing holePunchingStream: {e.Message}");
            }
        }
    }
}