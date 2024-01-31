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
    namespace Example
    {
        public interface ICardDatabase
        {
            Card GetCard(string id);
        }

        public class ExampleCardDatabase : ICardDatabase
        {
            private Dictionary<string, Card> _cards = new();

            public ExampleCardDatabase()
            {
                for(int i = 0; i < 10; ++i)
                {
                    Card card = new Card();
                    card.id = "card_" + i.ToString();

                    _cards.Add(card.id, card);
                }
            }

            public Card GetCard(string id)
            {
                return _cards[id];
            }
        }
    }


    public class DeckSystem : MonoBehaviour
    {
        public event Action OnHandChange;
        public event Action<Card> OnCardUsed;

        [SerializeField] private float _defaultDrawCooldown = 2f;
        private Deck _deck;

        private Card[] _hand;

        private Queue<Card> _cardQueue;
        private Queue<Tuple<Card, int>> _cardsToDraw = new();
        
        private float _drawCooldown;

        private ICardDatabase _cardDatabase;

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
            // TODO : Test Purpose, Remove this after implementing card database manager
            _cardDatabase = new ExampleCardDatabase();

            _deck = deck;

            List<Card> cards = new();

            foreach (string cardId in _deck.CardId)
            {
                Card card = _cardDatabase.GetCard(cardId);
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

        public void UseCard(int index)
        {
            if(index < 0 || index >= _hand.Length)
            {
                Debug.LogError("Invalid index");
                return;
            }
            if (_hand[index] == Card.Empty)
            {
                Debug.Log("Empty card");
                return;
            }
            var usedCard = _hand[index];
            _cardQueue.Enqueue(usedCard);
            _hand[index] = Card.Empty;

            QueueDrawCard(index);
            OnHandChange?.Invoke();
            OnCardUsed?.Invoke(usedCard);
        }

        private void QueueDrawCard(int index)
        {
            _cardsToDraw.Enqueue(new Tuple<Card, int>(_cardQueue.Dequeue(), index));
        }

        public void SetDrawCooldown(float cooldown)
        {
            _drawCooldown = cooldown;
        }

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
