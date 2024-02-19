using System;
using System.Collections.Generic;
using WPP.Collection;

namespace WPP.ClientInfo.Deck
{
    [Serializable]
    public class DecksData
    {
        public List<DeckData> decks;
        public DecksData()
        {
            decks = new List<DeckData>();
        }

        public void AddDeck(DeckData deck)
        {
            if (decks.Count > Constants.MAXIMUM_DECKS)
                return;

            decks.Add(deck);
        }

        public DeckData FindDeck(int deck_id)
        {
            foreach(DeckData deck in decks)
            {
                if (deck.id == deck_id)
                    return deck;
            }
            return null;
        }
    }
}
