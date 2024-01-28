using System;
using System.Collections.Generic;

namespace WPP.ClientInfo.Deck
{
    public class Decks
    {
        public List<Deck> decks { get; set; }
        public Decks()
        {
            decks = new List<Deck>();
        }

        public void AddDeck(Deck deck)
        {
            if (decks.Count > Constants.MAXIMUM_DECKS)
                return;

            decks.Add(deck);
        }
    }
}
