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
using WPP.Protocol;

namespace WPP.Network
{
    class ClientTCP
    {
        private static ClientTCP _instance = null;

        private byte[] _receivedPacket;
        private byte[] _receivedPacketForHolePunching;
        public ByteBuffer buffer { get; set; }
        public ByteBuffer inGameBuffer { get; set; }
        public IPEndPoint peerSockPublicEP { get; set; }
        public IPEndPoint peerSockPrivateEP { get; set; }

        public DateTime pingSentTime { get; set; }
        public DateTime pingAnsweredTime { get; set; }
        public TimeSpan rtt { get; set; }

        private object _packetQueueLockObject;

        public object PacketQueueLockObject { get; set; }

        public string IP;

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
            _networkService = new NetworkService();
            InitializeClientSock();
            InitializeClientSocketAsyncEventArgs();
            InitializePeerSocketAsyncEventArgs();
            PacketQueueLockObject = new object();
            Buffer = null;
        }

        public void InitializeClientSock()
        {
            clntSock = new TcpClient();
            clntSock.NoDelay = true;
            clntSock.ReceiveBufferSize = 4096;
            clntSock.SendBufferSize = 4096;
        }
        public void InitializeClientSocketAsyncEventArgs()
        {
            ClntReceiveEventArgs = new SocketAsyncEventArgs();
            ClntReceiveEventArgs.UserToken = this;
            ClntReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(_networkService.ReceiveCompleted);
            _clntReceiveBuffer = new byte[Constants.MAXIMUM_BUFFER_SIZE];
            ClntReceiveEventArgs.SetBuffer(_clntReceiveBuffer, 0, Constants.MAXIMUM_BUFFER_SIZE);

            ClntSendEventArgs = new SocketAsyncEventArgs();
            ClntSendEventArgs.UserToken = this;
            ClntSendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(_networkService.OnSendCompleted);
            _clntSendBuffer = new byte[Constants.MAXIMUM_BUFFER_SIZE];
            ClntSendEventArgs.SetBuffer(_clntSendBuffer, 0, Constants.MAXIMUM_BUFFER_SIZE);
        }

        public void InitializePeerSock()
        {
            try
            {
                peerSock = new TcpClient(new IPEndPoint(IPAddress.Any, 0));
                peerSock.NoDelay = true;
                peerSock.ReceiveBufferSize = 4096;
                peerSock.SendBufferSize = 4096;

                ConnectServerForHolePunching();
            }
            catch (Exception e)
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
                _networkService.BeginSend(ClntSendEventArgs, buffer.ToArray());
                Debug.Log($"send [packet size: {buffer.Count() + 8}] [tag: {(Client_PacketTagPackages)tag}] packet");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ConnectServer()
        {
            //clntSock.BeginConnect(IPAddress.Parse(Constants.SERVER_IP), Constants.SERVER_PORT, ConnectServerCallBack, null);
            clntSock.BeginConnect(IPAddress.Parse(IP), Constants.SERVER_PORT, ConnectServerCallBack, null);
        }

        private void ConnectServerCallBack(IAsyncResult result)
        {
            try
            {
                serverStream = clntSock.GetStream();
                Debug.Log("Connected to server with clntSock");

                serverStream.BeginRead(_receivedPacket, 0, _receivedPacket.Length, ReceivFromServerCallbck, null);
                //SendDataToServer(Client_PacketTagPackages.C_REQUEST_INITIAL_DATA);
                InitializePeerSock();
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

                lock(_packetQueueLockObject)
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
                _networkService.BeginSend(PeerSendEventArgs, buffer.ToArray());
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ConnectServerForHolePunching()
        {
            // for hole punching
            PeerSock.BeginConnect(IPAddress.Parse(IP), Constants.SERVER_PORT, ConnectServerCallBackForHolePunching, null);
        }

        private void ConnectServerCallBackForHolePunching(IAsyncResult result)
        {
            PeerSock.EndConnect(result);

            if (PeerSock.Connected == false)
            {
                Debug.Log("Connection Error");
                return;
            }

            try
            {
                Debug.Log("Connected to server with PeerSock");
                PeerSockPrivateEP = PeerSock.LocalEndPoint as IPEndPoint;

                SendDataToServerForHolePunching(Client_PacketTagPackages.C_REQUEST_HOLE_PUNCHING);
                Debug.Log("C_REQUEST_HOLE_PUNCHING");

                _networkService.BeginReceive(PeerReceiveEventArgs);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return;
            }
        }

        /***************** functions for p2p ******************/

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

                if (PeerSock != null)
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

        // functions to close connection
        private void CloseServerConnection()
        {
            Debug.Log($"serverStream: {ClntSock.RemoteEndPoint} closed connection");
            ClntSock.Close();
            ClntSock = null;
            return;
        }

        public void ClosePeerConnection()
        {
            if (PeerSock == null)
                return;
            Debug.Log($"P2Pstream: {PeerSock.RemoteEndPoint} closed connection");
            PeerSock.Close();
            PeerSock = null;
            return;
        }

        public void CloseHolePunchingConnection()
        {
            Debug.Log($"holePunchingStream: {PeerSock.RemoteEndPoint} closed connection");

            try
            {
                PeerSock.Close();
                PeerSock = null;
            }
            catch (Exception e)
            {
                Debug.Log($"Error closing holePunchingStream: {e.Message}");
            }
        }
    }
}