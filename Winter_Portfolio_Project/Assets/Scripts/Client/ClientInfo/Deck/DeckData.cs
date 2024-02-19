using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClientInfo.Card;

namespace WPP.ClientInfo.Deck
{
    public class DeckData
    {
        public int id { get; set; }
        public List<CardData> cards { get; set; }

        public DeckData(int id)
        {
            this.id = id;
            cards = new List<CardData>();
        }

        public void AddCard(CardData card)
        {
            if (cards.Count > Constants.MAXIMUM_CARDS_IN_DECK)
                return;

            cards.Add(card);
        }
    }
}
