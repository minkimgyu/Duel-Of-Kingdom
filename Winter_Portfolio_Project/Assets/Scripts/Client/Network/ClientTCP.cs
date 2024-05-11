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

        private NetworkService _networkService;
        public Socket ClntSock { get; set; }
        public Socket PeerSock { get; set; }
        public SocketAsyncEventArgs ClntReceiveEventArgs { get; private set; }
        byte[] _clntReceiveBuffer;
        public SocketAsyncEventArgs ClntSendEventArgs { get; private set; }
        byte[] _clntSendBuffer;

        public SocketAsyncEventArgs PeerReceiveEventArgs { get; private set; }
        byte[] _peerReceiveBuffer;
        public SocketAsyncEventArgs PeerSendEventArgs { get; private set; }
        byte[] _peerSendBuffer;
        public ByteBuffer Buffer { get; set; }
        public ByteBuffer InGameBuffer { get; set; }
        public IPEndPoint PeerSockPublicEP { get; set; }
        public IPEndPoint PeerSockPrivateEP { get; set; }

        // variables about ping
        public DateTime PingSentTime { get; set; }
        public DateTime PingAnsweredTime { get; set; }
        public TimeSpan Rtt { get; set; }

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
            ClntSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClntSock.Bind(new IPEndPoint(IPAddress.Any, 0));
            ClntSock.NoDelay = true;
            ClntSock.LingerState = new LingerOption(true, 0);
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
                PeerSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                PeerSock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                PeerSock.NoDelay = true;
                PeerSock.LingerState = new LingerOption(true, 0);

                if (PeerSockPrivateEP == null)
                {
                    PeerSock.Bind(new IPEndPoint(IPAddress.Any, 0));

                }
                else
                {
                    PeerSock.Bind(PeerSockPrivateEP);
                }

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void InitializePeerSocketAsyncEventArgs()
        {
            PeerReceiveEventArgs = new SocketAsyncEventArgs();
            PeerReceiveEventArgs.UserToken = this;
            PeerReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(_networkService.ReceiveCompleted);
            _peerReceiveBuffer = new byte[Constants.MAXIMUM_BUFFER_SIZE];
            PeerReceiveEventArgs.SetBuffer(_peerReceiveBuffer, 0, Constants.MAXIMUM_BUFFER_SIZE);

            PeerSendEventArgs = new SocketAsyncEventArgs();
            PeerSendEventArgs.UserToken = this;
            PeerSendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(_networkService.OnSendCompleted);
            _peerSendBuffer = new byte[Constants.MAXIMUM_BUFFER_SIZE];
            PeerSendEventArgs.SetBuffer(_peerSendBuffer, 0, Constants.MAXIMUM_BUFFER_SIZE);
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
            ClntSock.BeginConnect(IPAddress.Parse(IP), Constants.SERVER_PORT, ConnectServerCallBack, null);
        }

        private void ConnectServerCallBack(IAsyncResult result)
        {
            try
            {
                ClntSock.EndConnect(result);

                if (ClntSock.Connected == false)
                {
                    Debug.Log("Connection Error");
                    return;
                }

                Debug.Log("Connected to server with ClntSock");
                _networkService.BeginReceive(ClntReceiveEventArgs);
                InitializePeerSock();
                ConnectServerForHolePunching();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return;
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

                if (PeerSock != null)
                {
                    _networkService.BeginSend(PeerSendEventArgs, buffer.ToArray());
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
                if (PeerSock == null)
                    InitializePeerSock();

                PeerSock.BeginConnect(ep.Address, ep.Port, ConnectPeerCallBack, ep);
            }
            catch (Exception e)
            {
                Debug.Log($"Error connecting peer: {e.Message}");
            }
        }

        private void ConnectPeerCallBack(IAsyncResult result)
        {
            try
            {
                IPEndPoint ep = (IPEndPoint)result.AsyncState;

                PeerSock.EndConnect(result);

                if (PeerSock.Connected == false)
                {
                    Debug.Log("Connection Error");

                    if (IPEndPoint.Equals(ep, GameRoom.Instance().opponentPrivateEP))
                    {
                        ConnectPeer(GameRoom.Instance().opponentPublicEP as IPEndPoint);
                        return;
                    }

                    if (IPEndPoint.Equals(ep, GameRoom.Instance().opponentPublicEP))
                    {
                        Debug.Log("Connection Error");
                        return;
                    }
                }

                Debug.Log("Connected to peer");

                _networkService.BeginReceive(PeerReceiveEventArgs);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return;
            }
        }

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