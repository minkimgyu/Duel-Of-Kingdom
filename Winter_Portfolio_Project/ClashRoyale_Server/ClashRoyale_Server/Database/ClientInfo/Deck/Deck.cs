using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server.Database.CardData;

namespace WPP.ClashRoyale_Server.Database.ClientInfo.Deck
{
    class Deck
    {
        public Card[] cards;

        public Deck()
        {
            cards = new Card[Constants.MAXIMUM_CARDS_IN_DECK];
        }
    }
}
