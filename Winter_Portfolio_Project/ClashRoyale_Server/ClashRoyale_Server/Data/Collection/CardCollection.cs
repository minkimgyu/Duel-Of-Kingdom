using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server.Data.ClientInfo.CardData;
using WPP.ClashRoyale_Server;

namespace WPP.ClashRoyale_Server.Data.Collection
{
    class CardCollection
    {
        public List<Card> cardCollection { get; set; }

        public CardCollection()
        {
            cardCollection = new List<Card>();
        }

        public void AddCard(Card card)
        {
            cardCollection.Add(card);
        }
    }
}
