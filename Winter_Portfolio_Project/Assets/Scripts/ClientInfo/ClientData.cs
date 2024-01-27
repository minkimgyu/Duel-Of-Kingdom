using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClientInfo.Account;
using WPP.ClientInfo.CardData;
using WPP.ClientInfo.Deck;
using WPP.ClientInfo.Tower;

namespace WPP.ClientInfo
{
    public class ClientData
    {
        private static ClientData _instance;
        public ClientAccount account { get; set; }
        public Towers towers { get; set; }
        public Decks decks { get; set; }
        public static ClientData Instance()
        {
            if(_instance == null)
            {
                _instance = new ClientData();
            }
            return _instance;
        }

        public ClientData() {
            account = new ClientAccount();
            towers = new Towers();
            decks = new Decks();
        }
    }
}
