using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace WPP.ClashRoyale_Server.Protocol.Server
{
    internal class ServerTCP
    {
        private static ServerTCP _instance = null;
        private TcpListener _servSock;
        public ClientObject[] clients { get; set; }

        public List<ClientObject> waitingClients { get; set; }
        public static ServerTCP Instance()
        {
            if (_instance == null)
            {
                _instance = new ServerTCP();
            }
            return _instance;
        }

        public ServerTCP()
        {
            waitingClients = new List<ClientObject>();
        }

        public void InitializeServer()
        {
            InitializeServerSocket();
            InitalizeClients();
        }

        private void InitializeServerSocket()
        {
            _servSock = new TcpListener(IPAddress.Any, 7000);
            _servSock.Start(); // handle bind and listen
            _servSock.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), null);
            return;
        }

        private void InitalizeClients()
        {
            clients = new ClientObject[Constants.MAXIMUM_PLAYERS];
            for(int i=0; i< Constants.MAXIMUM_PLAYERS; i++)
            {
                clients[i] = new ClientObject(null, 0);
            }
            return;
        }

        private void AcceptCallback(IAsyncResult result)
        {
            TcpClient clntSock = _servSock.EndAcceptTcpClient(result);
            Console.WriteLine("Connected");

            _servSock.BeginAcceptTcpClient(AcceptCallback, null);

            for (int i=0; i < Constants.MAXIMUM_PLAYERS; i++)
            {
                if(clients[i].tcp.clntSock == null)
                {
                    clients[i] = new ClientObject(clntSock, i);
                    clients[i].state = ClientState.CONNECTED;
                    clients[i].tcp.Receive();
                    break;
                }
            }
            return;
        }

        public void SendDataTo(Server_PacketTagPackages tag, int clientID, byte[] data = null)
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
                clients[clientID].tcp.stream.BeginWrite(buffer.ToArray(), 0, buffer.Count(), null, null);
                buffer.Dispose();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
