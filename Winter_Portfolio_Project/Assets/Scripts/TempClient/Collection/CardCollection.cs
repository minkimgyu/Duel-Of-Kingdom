using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClientInfo.CardData;

namespace WPP.Collection
{
    internal class CardCollection
    {
        public List<Card> _cardCollection { get; set; }

        public CardCollection()
        {
            _cardCollection = new List<Card>();
        }

        public void AddCard(Card card)
        {
            _cardCollection.Add(card);
        }

        public Card FindCard(int card_id)
        {
            foreach(Card card in _cardCollection)
            {
                if (card.id == card_id)
                    return card;
            }
            return null;
        }
    }
}
