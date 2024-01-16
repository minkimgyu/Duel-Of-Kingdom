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
        public List<Card> cards { get; set; }

        public Deck(int id)
        {
            this.id = id;
            cards = new List<Card>();
        }

        public void AddCard(Card card)
        {
            if (cards.Count > Constants.MAXIMUM_CARDS_IN_DECK)
                return;

            cards.Add(card);
        }
    }
}
