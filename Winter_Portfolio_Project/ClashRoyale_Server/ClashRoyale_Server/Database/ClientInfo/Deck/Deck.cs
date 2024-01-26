using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server.Database.ClientInfo.CardData;

namespace WPP.ClashRoyale_Server.Database.ClientInfo.Deck
{
    class Deck
    {
        public int id { get; set; }
        public List<int> card_ids { get; set; }

        public Deck(int id)
        {
            this.id = id;   
            card_ids = new List<int>();
        }

        public void AddCard(int card_id)
        {
            if (card_ids.Count > Constants.MAXIMUM_CARDS_IN_DECK)
                return;

            card_ids.Add(card_id);
        }
    }
}
