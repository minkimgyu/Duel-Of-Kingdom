using System;
using System.Net.Sockets;
using UnityEngine;
using WPP.Network;

namespace WPP.Protocol
{
    public class NetworkService
    {
        private object _recvLock = new object();
        private object _sendLock = new object();

        public void BeginReceive(SocketAsyncEventArgs args)
        {
            ClientTCP client = args.UserToken as ClientTCP;

            if (client == null)
                return;

            if(client.ClntReceiveEventArgs.Equals(args))
            {
                bool pending = client.ClntSock.ReceiveAsync(args);
                if (!pending)
                {
                    ReceiveCompleted(null, args);
                }
            }
            else
            {
                bool pending = client.PeerSock.ReceiveAsync(args);
                if (!pending)
                {
                    ReceiveCompleted(null, args);
                }
            }

        }

        public void ReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock(_recvLock)
            {
                if (args.LastOperation == SocketAsyncOperation.Receive)
                {
                    ProcessReceive(args);
                    return;
                }
            }
        }

        void ProcessReceive(SocketAsyncEventArgs args)
        {
            lock(_recvLock)
            {
                ClientTCP client = args.UserToken as ClientTCP;

                // client's connection has already been disconnected.
                if (client == null)
                    return;

                int bytesReceived = args.BytesTransferred;

                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    // since args is using a buffer that is already assigned to the SocketAsyncEventArgsPool
                    // it requires a process of extracting the buffer by copying it separately using offset
                    byte[] receivedBytes = new byte[bytesReceived];

                    System.Buffer.BlockCopy(args.Buffer, 0, receivedBytes, 0, receivedBytes.Length);

                    // put the buffer with the contents to be processed into the packet queue
                    lock (ClientTCP.Instance().PacketQueueLockObject)
                    {
                        PacketHandler.Instance().packetQueue.Enqueue(receivedBytes);
                    }

                    try
                    {
                        bool pending;
                        if (client.ClntReceiveEventArgs.Equals(args))
                        {
                            pending = client.ClntSock.ReceiveAsync(args);
                        }
                        else
                        {
                            pending = client.PeerSock.ReceiveAsync(args);
                        }

                        if (!pending)
                        {
                            ReceiveCompleted(null, args);
                        }
                    }
                    catch (NullReferenceException e)
                    {
                        Debug.Log("null reference exception occurred: " + e.Message);
                    }
                }
            }
        }

        public void BeginSend(SocketAsyncEventArgs args, byte[] buffer)
        {
            lock (_sendLock)
            {
                ClientTCP client = args.UserToken as ClientTCP;

                if (client == null)
                    return;

                args.SetBuffer(buffer, 0, buffer.Length);

                bool pending;
                if (client.ClntSendEventArgs.Equals(args))
                {
                    pending = client.ClntSock.SendAsync(args);
                }
                else
                {
                    pending = client.PeerSock.SendAsync(args);
                }

                if (!pending)
                {
                    OnSendCompleted(null, args);
                }
            }
        }

        public void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock(_sendLock)
            {
                ClientTCP client = args.UserToken as ClientTCP;

                if (client == null)
                    return;
            }
        }
    }
}
