using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server.Database;
using WPP.ClashRoyale_Server.Database.ClientInfo.Account;
using WPP.ClashRoyale_Server.Protocol.Client;
using WPP.ClashRoyale_Server.Database.ClientInfo.Tower;
using WPP.ClashRoyale_Server.Database.ClientInfo.Deck;

namespace WPP.ClashRoyale_Server
{
    public enum ClientState
    {
        CONNECTED = 1,
        IN_ROOM,
        IN_GAME,
        DISCONNECTED
    }
    class ClientObject
    {
        public ClientTCP tcp { get; set; }
        public ClientState state { get; set; }
        public ClientAccount accountInfo { get; set; }
        public Towers towers  { get; set; }
        public Decks decks  { get; set; }

        public int gameRoomID { get; set; }

        public ClientObject(TcpClient socket, int id)
        {
            tcp = new ClientTCP(socket, id);
            state = ClientState.DISCONNECTED;
            accountInfo = new ClientAccount();
            towers = new Towers();
            decks = new Decks();
            gameRoomID = 0;
        }
    }
}
