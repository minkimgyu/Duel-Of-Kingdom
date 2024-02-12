using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.ClientInfo;

namespace WPP.DeckManagement
{
    public static class DeckManager
    {
        public static Deck CurrentDeck => decks[selectedDeckIndex];
        public static IReadOnlyList<Deck> Decks => decks;
        private static List<Deck> decks;
        private static int selectedDeckIndex = 0;

        public static void LoadPlayerDeck()
        {
            decks = new List<Deck>();
            foreach (var deckData in ClientData.Instance().decks.decks)
            {
                Deck newDeck = new();
                for (int i = 0; i < deckData.cards.Count; i++)
                {
                    var cardData = deckData.cards[i];

                    Card card;
                    int level;
                    if(cardData.unit == null)
                    {
                        card = Card.Empty;
                        level = 0;
                    }
                    else
                    {
                        card = CardDatabase.GetCard(cardData);
                        level = cardData.unit._level;
                    }

                    newDeck.SetCard(i, card);
                    newDeck.SetCardLevel(i, level);
                }
                decks.Add(newDeck);
            }
        }

        public static void SelectDeck(int index)
        {
            selectedDeckIndex = index;
        }
    }
}
