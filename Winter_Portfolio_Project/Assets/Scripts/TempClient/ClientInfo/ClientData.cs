using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClientInfo.Account;
using WPP.ClientInfo.Card;
using WPP.ClientInfo.Deck;
using WPP.ClientInfo.Tower;

namespace WPP.ClientInfo
{
    public class ClientData
    {
        private static ClientData _instance;
        public AccountData account { get; set; }
        public TowersData towers { get; set; }
        public DecksData decks { get; set; }

        public int player_id_in_game;
        public static ClientData Instance()
        {
            if(_instance == null)
            {
                _instance = new ClientData();
            }
            return _instance;
        }

        public ClientData() {
            account = new AccountData();
            towers = new TowersData();
            decks = new DecksData();
            player_id_in_game = -1;
        }
    }
}
