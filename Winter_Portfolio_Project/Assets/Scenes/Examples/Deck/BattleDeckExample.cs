using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WPP.Battle;

namespace WPP.DeckManagement.Example
{
    public class BattleDeckExample : MonoBehaviour
    {
        [SerializeField] private DeckSystem _deckSystem;
        [SerializeField] private TextMeshProUGUI[] _cardTexts;
        [SerializeField] private TextMeshProUGUI _next;
        private void Start()
        {
            Deck deck = new Deck();
            for (int i = 0; i < 8; i++)
            {
                Card card = new();
                card.id = "card_" + i;
                deck.SetCard(i, card);
            }
            _deckSystem.Init(deck);
        }

        private void OnEnable()
        {
            _deckSystem.OnHandChange += OnCardDrawn;
        }
        private void OnDisable()
        {
            _deckSystem.OnHandChange -= OnCardDrawn;
        }

        private void OnCardDrawn()
        {
            var hand = _deckSystem.Hand;
            for (int i = 0; i < _cardTexts.Length; i++)
            {
                _cardTexts[i].text = hand[i].id;
            }
            _next.text = _deckSystem.Next.id;
        }
    }
}
