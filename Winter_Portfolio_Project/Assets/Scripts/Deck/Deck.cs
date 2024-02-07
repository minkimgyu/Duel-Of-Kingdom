using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.DeckManagement
{
    public class Deck
    {
        public Deck()
        {
            cards = new(8);
            for (int i = 0; i < 8; i++)
            {
                cards.Add(Card.Empty);
            }
            cardLevel = new(8);
            for (int i = 0; i < 8; i++)
            {
                cardLevel.Add(1);
            }
        }

        private List<Card> cards;
        private List<int> cardLevel;

        public IReadOnlyList<Card> Cards => cards;

        public string GetCardId(int index) => cards[index].id;
        public int GetCardLevel(int index) => cardLevel[index];
        public int GetCardLevel(Card card) => cardLevel[cards.IndexOf(card)];

        public void SetCard(int index, Card card) => cards[index] = card;

        public void SetCardLevel(int index, int level) => cardLevel[index] = level;

        public void SetEmpty(int index) => cards[index] = Card.Empty;

        public bool IsEmpty(int index) => cards[index].id == Card.Empty.id;
    }
}
