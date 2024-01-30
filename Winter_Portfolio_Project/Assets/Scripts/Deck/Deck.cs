using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Deck
{
    public class Deck
    {
        public Deck()
        {
            cardId = new(8);
            for (int i = 0; i < 8; i++)
            {
                cardId.Add(Card.Empty.id);
            }
            cardLevel = new(8);
            for (int i = 0; i < 8; i++)
            {
                cardLevel.Add(0);
            }
        }

        private List<string> cardId;
        private List<int> cardLevel;

        public IReadOnlyList<string> CardId => cardId;

        public string GetCardId(int index) => cardId[index];
        public int GetCardLevel(int index) => cardLevel[index];

        public void SetCard(int index, string id) => cardId[index] = id;
        public void SetCard(int index, Card card) => SetCard(index, card.id);

        public void SetCardLevel(int index, int level) => cardLevel[index] = level;

        public void SetEmpty(int index) => cardId[index] = Card.Empty.id;

        public bool IsEmpty(int index) => cardId[index] == Card.Empty.id;
    }
}
