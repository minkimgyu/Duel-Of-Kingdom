using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WPP.DeckManagement;

namespace WPP.Battle.UI
{
    public class BattleDeckUIController : MonoBehaviour
    {
        [SerializeField] private DeckSystem _deckSystem;
        [Space]
        [SerializeField] private RectTransform[] _cards;
        [SerializeField] private TextMeshProUGUI[] _cardTexts;
        [SerializeField] private TextMeshProUGUI _next;
        [SerializeField] private TextMeshProUGUI _cooldown;
        [Space]
        [SerializeField] private float _selectedCardOffset = 50f;
        [SerializeField] private float _selectedCardScale = 1.2f;
        
        private int _selectedCardIndex = -1;
        private bool _isPlacingCard = false;

        private void OnEnable()
        {
            _deckSystem.OnHandChange += OnCardDrawn;
            _deckSystem.OnCardUsed += (_) => { DeselectCard(); };
        }

        private void OnCardDrawn()
        {
            var hand = _deckSystem.Hand;

            for (int i = 0; i < _cardTexts.Length; i++)
            {
                if (hand[i].id == null)
                {
                    _cards[i].gameObject.SetActive(false);
                }
                else
                {
                    if (_isPlacingCard && i == _selectedCardIndex) continue;

                    _cards[i].gameObject.SetActive(true);
                    _cardTexts[i].text = hand[i].id;
                }
            }

            _next.text = _deckSystem.Next.id;
        }

        public void SelectCard(int index)
        {
            if(_selectedCardIndex == index)
            {
                DeselectCard();
                return;
            }
            CancelPlacingCard();
            
            _selectedCardIndex = index;
            UpdateCardTransform();
        }

        public void DeselectCard()
        {
            _selectedCardIndex = -1;
            _isPlacingCard = false;
            UpdateCardTransform();
        }

        private void UpdateCardTransform()
        {
            for(int i = 0; i < _cards.Length; ++i)
            {
                if (i == _selectedCardIndex)
                {
                    _cards[i].anchoredPosition = Vector2.up * _selectedCardOffset;
                    _cards[i].localScale = Vector3.one * _selectedCardScale;
                }
                else
                {
                    _cards[i].anchoredPosition = Vector2.zero;
                    _cards[i].localScale = Vector3.one;
                }
            }
        }

        public void StartPlacingCard()
        {
            if(_selectedCardIndex == -1 || _isPlacingCard) return;
            _cards[_selectedCardIndex].gameObject.SetActive(false);
            _isPlacingCard = true;
        }

        public void CancelPlacingCard()
        {
            if (_selectedCardIndex == -1 || _isPlacingCard == false) return;
            _cards[_selectedCardIndex].gameObject.SetActive(true);
            _isPlacingCard = false;

            UpdateCardTransform();
        }

        // Test Purpose
        private void Start()
        {
            Deck deck = new Deck();
            for (int i = 0; i < 8; i++)
            {
                deck.SetCard(i, "card_" + i.ToString());
            }
            _deckSystem.Init(deck);
        }

        private void Update()
        {
            if(_deckSystem.LeftCooldown > 0f)
                _cooldown.text = _deckSystem.LeftCooldown.ToString("F1");
            else _cooldown.text = "";

            if(Input.GetMouseButtonDown(1))
            {
                if(_isPlacingCard)
                {
                    CancelPlacingCard();
                }
                else
                {
                    StartPlacingCard();
                }
            }
            if(_selectedCardIndex != -1 && Input.GetKeyDown(KeyCode.Space))
            {
                _deckSystem.UseCard(_selectedCardIndex);
            }
        }
    }
}
