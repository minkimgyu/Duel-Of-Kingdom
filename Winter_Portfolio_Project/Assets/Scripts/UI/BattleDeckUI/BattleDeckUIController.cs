using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WPP.DeckManagement;
using WPP.DeckManagement.UI;

namespace WPP.Battle.UI
{
    public class BattleDeckUIController : MonoBehaviour
    {
        [SerializeField] private DeckSystem _deckSystem;
        [SerializeField] private ElixirSystem _elixirSystem;
        [Header("Card")]
        [SerializeField] private CardUI[] _cards;
        //[SerializeField] private RectTransform[] _cards;
        //[SerializeField] private TextMeshProUGUI[] _cardTexts;
        //[SerializeField] private TextMeshProUGUI[] _cardElixir;

        [SerializeField] private TextMeshProUGUI _next;
        [SerializeField] private TextMeshProUGUI _nextElixir;
        [SerializeField] private TextMeshProUGUI _cooldown;
        [Space]
        [SerializeField] private float _selectedCardOffset = 50f;
        [SerializeField] private float _selectedCardScale = 1.2f;
        [Header("Elixir")]
        [SerializeField] private Slider _elixirSlider;
        [SerializeField] private TextMeshProUGUI _elixirText;


        private int _selectedCardIndex = -1;
        private bool _isPlacingCard = false;

        private void OnEnable()
        {
            _deckSystem.OnHandChange += OnCardDrawn;
            _deckSystem.OnCardUsed += (_) => { DeselectCard(); };

            _elixirSystem.OnElixirCountChange += SetElixirBar;
        }

        private void OnCardDrawn()
        {
            var hand = _deckSystem.Hand;

            for (int i = 0; i < _cards.Length; i++)
            {
                if (hand[i].id == null)
                {
                    _cards[i].gameObject.SetActive(false);
                }
                else
                {
                    if (_isPlacingCard && i == _selectedCardIndex) continue;

                    _cards[i].gameObject.SetActive(true);
                    _cards[i].SetCard(hand[i].id, hand[i].cost);
                }
            }

            _next.text = _deckSystem.Next.id;
            _nextElixir.text = _deckSystem.Next.cost.ToString();
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
                var transform = _cards[i].GetComponent<RectTransform>();
                if (i == _selectedCardIndex)
                {
                    transform.anchoredPosition = Vector2.up * _selectedCardOffset;
                    transform.localScale = Vector3.one * _selectedCardScale;
                }
                else
                {
                    transform.anchoredPosition = Vector2.zero;
                    transform.localScale = Vector3.one;
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
        
        private void SetElixirBar(int count)
        {
            _elixirSlider.value = count;
            _elixirText.text = count.ToString();
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
            _elixirSystem.StartRegen();
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
