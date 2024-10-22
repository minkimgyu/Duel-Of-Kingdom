using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.DeckManagement.UI;

namespace WPP.DeckManagement
{
    public class DeckEditor : MonoBehaviour
    {
        [SerializeField] private int _maxPoint = 40;
        public int MaxPoint => _maxPoint;
        public event System.Action<Deck> OnDeckChanged;

        private Deck[] _deck;
        private int currentDeck;
        public Deck SelectedDeck => _deck[currentDeck];

        private void Awake()
        {
            OnDeckChanged += SaveDeck;
        }

        private void OnDestroy()
        {
            OnDeckChanged -= SaveDeck;
        }

        public void LoadDeck()
        {
            _deck = new Deck[DeckManager.Decks.Count];
            for (int i = 0; i < _deck.Length; i++)
            {
                _deck[i] = DeckManager.Decks[i];
            }
        }

        public void SelectDeck(int deckIndex)
        {
            currentDeck = deckIndex;
            DeckManager.SelectDeck(deckIndex);

            OnDeckChanged?.Invoke(SelectedDeck);
        }

        public void SetCard(int cardIndex, Card card)
        {
            if (IsInDeck(card))
            {
                Debug.Log("Card is already in deck");
                return;
            }
            _deck[currentDeck].SetCard(cardIndex, card);
            OnDeckChanged?.Invoke(SelectedDeck);
        }

        public void AddCard(Card card)
        {
            if(IsInDeck(card))
            {
                Debug.Log("Card is already in deck");
                return;
            }

            for (int i = 0; i < _deck[currentDeck].Cards.Count; i++)
            {
                if (_deck[currentDeck].IsEmpty(i))
                {
                    _deck[currentDeck].SetCardLevel(i, 1);
                    _deck[currentDeck].SetCard(i, card);
                    OnDeckChanged?.Invoke(SelectedDeck);
                    return;
                }
            }
            Debug.Log("Deck is full");
        }

        public void RemoveCard(int cardIndex)
        {
            _deck[currentDeck].SetCardLevel(cardIndex, 0);
            _deck[currentDeck].SetEmpty(cardIndex);
            OnDeckChanged?.Invoke(SelectedDeck);
        }

        public void IncreaseCardLevel(int cardIndex)
        {
            int level = _deck[currentDeck].GetCardLevel(cardIndex);
            if (level >= 10)
            {
                Debug.Log("Card is already max level");
                return;
            }
            if (GetTotalDeckPoint() >= _maxPoint)
            {
                Debug.Log("Cannot level up beyond total of " + _maxPoint + " points");
                return;
            }

            _deck[currentDeck].SetCardLevel(cardIndex, _deck[currentDeck].GetCardLevel(cardIndex) + 1);
            OnDeckChanged?.Invoke(SelectedDeck);
        }

        public void DecreaseCardLevel(int cardIndex)
        {
            int level = _deck[currentDeck].GetCardLevel(cardIndex);
            if (level <= 1)
            {
                Debug.Log("Card is already min level");
                return;
            }

            _deck[currentDeck].SetCardLevel(cardIndex, _deck[currentDeck].GetCardLevel(cardIndex) - 1);
            OnDeckChanged?.Invoke(SelectedDeck);
        }
    
        public void ClearDeck()
        {
            for (int i = 0; i < _deck[currentDeck].Cards.Count; i++)
            {
                _deck[currentDeck].SetCardLevel(i, 0);
                _deck[currentDeck].SetEmpty(i);
            }
            OnDeckChanged?.Invoke(SelectedDeck);
        }

        public int GetTotalDeckPoint() => GetTotalDeckPoint(SelectedDeck);

        public int GetTotalDeckPoint(Deck deck)
        {
            int totalPoint = 0;
            for (int i = 0; i < deck.Cards.Count; i++)
            {
                if (!deck.IsEmpty(i))
                {
                    totalPoint += deck.GetCardLevel(i);
                }
            }
            return totalPoint;
        }

        public bool IsInDeck(Card card)
        {
            for (int i = 0; i < _deck[currentDeck].Cards.Count; i++)
            {
                if (_deck[currentDeck].GetCardId(i) == card.id)
                {
                    return true;
                }
            }
            return false;
        }
    
        public void SaveDeck(Deck deck)
        {
            foreach (var card in deck.Cards)
            {
                if (card == Card.Empty)
                {
                    Debug.Log("Deck is not full, not saving");
                    return;
                }
            }
            DeckManager.SaveCurrentDeck();
        }
    }
}
