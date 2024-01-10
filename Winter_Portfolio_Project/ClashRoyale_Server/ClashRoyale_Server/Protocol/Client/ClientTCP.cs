using WPP.ClashRoyale_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WPP.ClashRoyale_Server.Protocol.Server;

namespace WPP.ClashRoyale_Server.Protocol.Client
{
    class ClientTCP
    {
        public TcpClient clntSock { get; set; }
        public NetworkStream stream { get; set; }
        private byte[] _receivedData;
        public ByteBuffer buffer { get; set; }
        public int id { get; set; }


        public ClientTCP(TcpClient socket, int id)
        {
            if (socket == null)
                return;

            clntSock = socket;
            clntSock.NoDelay = true;
            clntSock.ReceiveBufferSize = 1024;
            clntSock.SendBufferSize = 1024;

            stream = clntSock.GetStream();

            _receivedData = new byte[1024];
            buffer = null;

            this.id = id;
        }

        public void Receive()
        {
            stream.BeginRead(_receivedData, 0, _receivedData.Length, ReceiveCallbck, null);
        }

        private void ReceiveCallbck(IAsyncResult result)
        {
            try
            {
                int bytesReceived = stream.EndRead(result);

                if(bytesReceived <= 0)
                {
                    CloseConnection();
                    return;
                }

                byte[] dataToHandle = new byte[bytesReceived];
                Buffer.BlockCopy(_receivedData, 0, dataToHandle, 0, bytesReceived);

                PacketHandler.Instance().HandlePacket(id, dataToHandle);

                stream.BeginRead(_receivedData, 0, _receivedData.Length, ReceiveCallbck, null);
                return;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void CloseConnection()
        {
            Console.WriteLine("{0} closed connection", clntSock.Client.RemoteEndPoint);
            stream.Close();
            clntSock.Close();
            clntSock = null;
            return;
        }
    }
}
