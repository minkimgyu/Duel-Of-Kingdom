﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClashRoyale_Server.Database.ClientInfo.CardData;
using WPP.ClashRoyale_Server;

namespace WPP.ClashRoyale_Server.Database.Collection
{
    internal class CardCollection
    {
        private List<Card> _cardCollection;

        public CardCollection()
        {
            _cardCollection = new List<Card>();
        }

        public void AddCard(Card card)
        {
            _cardCollection.Add(card);
        }
    }
}
