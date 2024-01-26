using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Deck
{
    public class DeckEditor : MonoBehaviour
    {
        private Deck[] _deck;
        private int currentDeck;
        public Deck SelectedDeck => _deck[currentDeck];

        public void LoadDeck()
        {
            _deck = new Deck[3];
            for (int i = 0; i < 3; i++)
            {
                _deck[i] = new Deck();

                for (int j = 0; j < 8; j++)
                {
                    Card card = new Card();
                    card.id = "deck_" + i + "_troop_" + j;

                    _deck[i].SetCard(j, card);
                }
            }
        }

        public void SelectDeck(int deckIndex)
        {
            currentDeck = deckIndex;
        }

        public void SetCard(int cardIndex, Card card)
        {
            _deck[currentDeck].SetCard(cardIndex, card);
        }

        public void AddCard(Card card)
        {
            for (int i = 0; i < _deck[currentDeck].CardId.Count; i++)
            {
                if (_deck[currentDeck].IsEmpty(i))
                {
                    _deck[currentDeck].SetCard(i, card);
                    break;
                }
            }
        }

        public void RemoveCard(int cardIndex)
        {
            _deck[currentDeck].SetEmpty(cardIndex);
        }

        public void SetCardLevel(int cardIndex, int level)
        {
            _deck[currentDeck].SetCardLevel(cardIndex, level);
        }
    
        public void ClearDeck()
        {
            for (int i = 0; i < _deck[currentDeck].CardId.Count; i++)
            {
                _deck[currentDeck].SetEmpty(i);
            }
        }
    }
}
