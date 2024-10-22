using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using WPP.Collection;
using WPP.SOUND;

namespace WPP.DeckManagement.UI
{
    public class DeckEditorUIController : MonoBehaviour
    {
        [SerializeField] private DeckEditor _deckEditor;
        [Header("Deck UI")]
        [SerializeField] private TextMeshProUGUI _deckTotalPoint;
        [SerializeField] private GameObject _deckCardGrid;
        [SerializeField] private GameObject _deckCardPopupGrid;
        [SerializeField] private int _defaultDeckCardCount = 8;

        [Header("Collection UI")]
        [SerializeField] private GameObject _collectionCardGrid;
        [SerializeField] private GameObject _collectionCardPopupGrid;

        [Header("Card Prefabs")]
        [SerializeField] private GameObject _cardHolder;
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private GameObject _cardPopupPrefab;
        [Space]
        [SerializeField] private CardInfoUI _cardInfoUI;

        //private List<Card> _cardCollection;

        private List<CardUI> _deckCardUIs;
        private List<CardUI> _collectionCardUIs;

        private List<GameObject> _deckPopups;
        private List<GameObject> _collectionPopups;

        /*
        private void Awake()
        {
            LoadCards();
        }
        public void LoadCards()
        {
            _cardCollection = new List<Card>();
            for (int i = 0; i < 10; i++)
            {
                Card card = new Card();
                card.id = "card_" + i;
                _cardCollection.Add(card);
            }
        }
        */

        private void Awake()
        {
            _deckEditor.OnDeckChanged += SetCards;
            _deckEditor.OnDeckChanged += SetCardLevel;
            _deckEditor.OnDeckChanged += SetTotalPoint;
        }

        public void Initialize()
        {
            InitializeGrid();
            _deckEditor.LoadDeck();
            _deckEditor.SelectDeck(0);
        }

        private void InitializeGrid()
        {
            foreach(Transform child in _deckCardGrid.transform)
                Destroy(child.gameObject);

            foreach (Transform child in _deckCardPopupGrid.transform)
                Destroy(child.gameObject);

            foreach (Transform child in _collectionCardGrid.transform)
                Destroy(child.gameObject);

            foreach (Transform child in _collectionCardPopupGrid.transform)
                Destroy(child.gameObject);

            // 1. Instantiate card prefabs

            _deckCardUIs = new();
            _deckPopups = new();
            InstatiateCard(_deckCardGrid.transform, _deckCardPopupGrid.transform,
                _deckCardUIs, _deckPopups, _defaultDeckCardCount, true);

            _collectionCardUIs = new();
            _collectionPopups = new();
            InstatiateCard(_collectionCardGrid.transform, _collectionCardPopupGrid.transform,
                _collectionCardUIs, _collectionPopups, CardDatabase.Cards.Count, false);

            void InstatiateCard(Transform cardGrid, Transform popupGrid, List<CardUI> cardUIs, List<GameObject> popups, int count, bool isDeckCard)
            {
                for (int i = 0; i < count; i++) {
                    GameObject cardHolder = Instantiate(_cardHolder, cardGrid);
                    GameObject card = Instantiate(_cardPrefab, cardHolder.transform);

                    GameObject cardPopupHolder = Instantiate(_cardHolder, popupGrid);
                    GameObject cardPopup = Instantiate(_cardPopupPrefab, cardPopupHolder.transform);

                    cardUIs.Add(card.GetComponentInChildren<CardUI>());
                    popups.Add(cardPopup);

                    card.GetComponentInChildren<CardUI>().Initialize(this, cardPopup, i);
                    cardPopup.GetComponentInChildren<CardUI>().Initialize(this, cardPopup, i);

                    cardPopup.GetComponentInChildren<CardPopupUI>().Initialize(this, i, isDeckCard);

                    cardPopup.SetActive(false);
                }
            }
        }

        public void TurnAllPopupsOff()
        {
            foreach (GameObject popup in _deckPopups)
                popup.SetActive(false);

            foreach (GameObject popup in _collectionPopups)
                popup.SetActive(false);
        }

        public void SelectDeck(int index)
        {
            SoundManager.PlaySFX("Button1");

            TurnAllPopupsOff();
            _deckEditor.SelectDeck(index);
        }

        public void AddCardToDeck(int index)
        {
            SoundManager.PlaySFX("CardDrop");

            _deckEditor.AddCard(CardDatabase.Cards.ElementAt(index).Value);
            TurnAllPopupsOff();
        }

        public void RemoveCardFromDeck(int index)
        {
            SoundManager.PlaySFX("CardDrop");

            _deckEditor.RemoveCard(index);
            TurnAllPopupsOff();
        }

        public void LevelUpCard(int index)
        {
            SoundManager.PlaySFX("Button1");

            _deckEditor.IncreaseCardLevel(index);
        }

        public void LevelDownCard(int index)
        {
            SoundManager.PlaySFX("Button1");

            _deckEditor.DecreaseCardLevel(index);
        }
    
        public void SetCards(Deck deck)
        {
            HashSet<Card> deckCards = new HashSet<Card>(deck.Cards);
            for (int i = 0; i < deck.Cards.Count; i++)
            {
                Card card;
                if (CardDatabase.Cards.TryGetValue(deck.Cards[i].id, out Card found))
                {
                    card = found;
                }
                else
                {
                    card = Card.Empty;
                }

                _deckCardUIs[i].SetCard(card, deck.GetCardLevel(i) / 10f);
                _deckPopups[i].GetComponentInChildren<CardUI>().SetCard(card, deck.GetCardLevel(i) / 10f);

                deckCards.Add(card);
            }

            for (int i = 0; i < CardDatabase.Cards.Count; i++)
            {
                var card = CardDatabase.Cards.ElementAt(i).Value;
                _collectionCardUIs[i].SetCard(card, 0);
                _collectionPopups[i].GetComponentInChildren<CardUI>().SetCard(card, 0);

                _collectionCardUIs[i].SetInteractable(!deckCards.Contains(card));
            }
        }
        
        public void SetCardLevel(Deck deck)
        {
            for (int i = 0; i < deck.Cards.Count; i++)
            {
                _deckPopups[i].GetComponentInChildren<CardPopupUI>().SetCardLevel(deck.GetCardLevel(i));
            }
        }

        public void SetTotalPoint(Deck deck)
        {
            _deckTotalPoint.text = _deckEditor.GetTotalDeckPoint(deck).ToString() + "/" + _deckEditor.MaxPoint;
        }

        public void ShowInfo(int gridIndex)
        {
            _cardInfoUI.ShowCardInfo(_deckEditor.SelectedDeck.Cards[gridIndex], _deckEditor.SelectedDeck.GetCardLevel(gridIndex));
        }
    }
}
