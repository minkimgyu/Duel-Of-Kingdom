using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.Deck.UI;

namespace WPP.Deck
{
    public class DeckEditor : MonoBehaviour
    {
        public event System.Action<Deck> OnDeckChanged;

        private Deck[] _deck;
        private int currentDeck;
        public Deck SelectedDeck => _deck[currentDeck];

        public void LoadDeck()
        {
            _deck = new Deck[3];
            for (int i = 0; i < 3; i++)
            {
                _deck[i] = new Deck();
            }
        }

        public void SelectDeck(int deckIndex)
        {
            currentDeck = deckIndex;
            OnDeckChanged?.Invoke(SelectedDeck);
        }

        public void SetCard(int cardIndex, Card card)
        {
            if (IsInDeck(card.id))
            {
                Debug.Log("Card is already in deck");
                return;
            }
            _deck[currentDeck].SetCard(cardIndex, card);
            OnDeckChanged?.Invoke(SelectedDeck);
        }

        public void AddCard(string id)
        {
            if(IsInDeck(id))
            {
                Debug.Log("Card is already in deck");
                return;
            }

            for (int i = 0; i < _deck[currentDeck].CardId.Count; i++)
            {
                if (_deck[currentDeck].IsEmpty(i))
                {
                    _deck[currentDeck].SetCard(i, id);
                    OnDeckChanged?.Invoke(SelectedDeck);
                    return;
                }
            }
            Debug.Log("Deck is full");
        }
        public void AddCard(Card card) => AddCard(card.id);

        public void RemoveCard(int cardIndex)
        {
            _deck[currentDeck].SetEmpty(cardIndex);
            OnDeckChanged?.Invoke(SelectedDeck);
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
            OnDeckChanged?.Invoke(SelectedDeck);
        }

        public bool IsInDeck(string id)
        {
            for (int i = 0; i < _deck[currentDeck].CardId.Count; i++)
            {
                if (_deck[currentDeck].GetCardId(i) == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
