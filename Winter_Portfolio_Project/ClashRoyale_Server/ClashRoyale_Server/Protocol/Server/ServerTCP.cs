using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using WPP.ClashRoyale_Server.Data.ClientInfo.Deck;
using WPP.ClashRoyale_Server.Data.Collection;
using WPP.ClashRoyale_Server.Data;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

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

            _servSock.BeginAcceptTcpClient(AcceptCallback, null);

            for (int i=0; i < Constants.MAXIMUM_PLAYERS; i++)
            {
                if(clients[i].tcp.clntSock == null)
                {
                    Console.WriteLine("Connected");

                    clients[i] = new ClientObject(clntSock, i);
                    clients[i].state = ClientState.CONNECTED;
                    clients[i].tcp.Receive();

                    // Collection 정보 JSON화
                    // 모든 카드 정보를 넣기엔 너무 많기 때문에
                    // 클라이언트에 별도의 데이터 시트를 먼저 만든 다음
                    // 업데이트 된 부분만 별도 적용하는 방법도 생각중
                    CardCollection cardCollection = DatabaseManager.Instance().LoadCardCollection(clients[i].tcp.id, clients[i].accountInfo.username);
                    string cardCollectionString = JsonConvert.SerializeObject(cardCollection);
                    ByteBuffer buffer = new ByteBuffer();
                    buffer.WriteString(cardCollectionString);
                    ServerTCP.Instance().SendDataTo(Server_PacketTagPackages.S_LOAD_CARD_COLLECTION, i, buffer.ToArray());

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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
