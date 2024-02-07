using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;
using WPP.Battle.Example;
using WPP.DeckManagement;

namespace WPP.Battle
{
    public class DeckSystem : MonoBehaviour
    {
        public event Action OnHandChange;
        public event Action<Card, int> OnCardUsed;

        [SerializeField] private ElixirSystem _elixirSystem;
        [SerializeField] private float _defaultDrawCooldown = 2f;
        private Deck _deck;

        private Card[] _hand;

        private Queue<Card> _cardQueue;
        private Queue<Tuple<Card, int>> _cardsToDraw = new();
        
        private float _drawCooldown;

        public Card Next
        {
            get
            {
                if(_cardsToDraw.Count > 0)
                {
                    return _cardsToDraw.Peek().Item1;
                }
                else
                {
                    return _cardQueue.Peek();
                }
            }
        }
        public IReadOnlyList<Card> Hand => Array.AsReadOnly(_hand);

        public void Init(Deck deck)
        {
            _deck = deck;

            List<Card> cards = new();

            foreach (var card in _deck.Cards)
            {
                cards.Add(card);
            }

            _cardQueue = new Queue<Card>(cards.OrderBy(x => UnityEngine.Random.value));
            
            _hand = new Card[4];
            for (int i = 0; i < 4; i++)
            {
                _hand[i] = _cardQueue.Dequeue();
            }

            SetDrawCooldown(_defaultDrawCooldown);
            OnHandChange?.Invoke();
        }

        public bool UseCard(int index)
        {
            if(index < 0 || index >= _hand.Length)
            {
                Debug.LogError("Invalid index");
                return false;
            }
            if (_hand[index] == Card.Empty)
            {
                Debug.Log("Empty card");
                return false; 
            }

            if (_hand[index].cost > _elixirSystem.ElixirCount)
            {
                Debug.Log("Not enough elixir");
                return false;
            }
            _elixirSystem.SpendElixir(_hand[index].cost);

            var usedCard = _hand[index];
            _cardQueue.Enqueue(usedCard);
            _hand[index] = Card.Empty;

            QueueDrawCard(index);
            OnCardUsed?.Invoke(usedCard, _deck.GetCardLevel(usedCard));
            OnHandChange?.Invoke();
            return true;
        }

        public int GetCardLevel(int index) => _deck.GetCardLevel(_hand[index]);

        private void QueueDrawCard(int index)
        {
            _cardsToDraw.Enqueue(new Tuple<Card, int>(_cardQueue.Dequeue(), index));
        }

        public void SetDrawCooldown(float cooldown)
        {
            _drawCooldown = cooldown;
        }

        public float LeftCooldown => Mathf.Clamp(_drawCooldown - (Time.time - _lastDrawTime), 0, _drawCooldown);

        private float _lastDrawTime = -99f;
        private void Update()
        {
            if(Time.time - _lastDrawTime >= _drawCooldown && _cardsToDraw.Count > 0)
            {
                _lastDrawTime = Time.time;
                
                var cardToDraw = _cardsToDraw.Dequeue();
                _hand[cardToDraw.Item2] = cardToDraw.Item1;

                OnHandChange?.Invoke();
            }
        }
    }
}
