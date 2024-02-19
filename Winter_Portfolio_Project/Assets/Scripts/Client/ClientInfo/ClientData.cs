using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClientInfo.Account;
using WPP.ClientInfo.Card;
using WPP.ClientInfo.Deck;
using WPP.ClientInfo.Tower;
using WPP.AI.GRID;

namespace WPP.ClientInfo
{
    class ClientData
    {
        private static ClientData _instance;
        public AccountData account { get; set; }
        public TowersData towers { get; set; }
        public DecksData decks { get; set; }
        public CardsData cards { get; set; }

        public int player_id_in_game;
        public static ClientData Instance()
        {
            if(_instance == null)
            {
                _instance = new ClientData();
            }
            return _instance;
        }

        // 이런 식으로 지형 데이터 받아오기
        public LandFormation LandFormation
        {
            get 
            {
                if (player_id_in_game == 0) return LandFormation.C;
                else return LandFormation.R;
            }
        }

        public ClientData() {
            account = new AccountData();
            towers = new TowersData();
            decks = new DecksData();
            cards = new CardsData();
            player_id_in_game = -1;
        }
    }
}
