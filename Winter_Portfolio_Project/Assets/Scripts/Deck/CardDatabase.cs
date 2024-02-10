using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using WPP.ClientInfo.Card;
using WPP.Collection;
using WPP.FileReader;
using WPP.Units;

namespace WPP.DeckManagement
{
    public class CardDatabase : MonoBehaviour
    {
        public static IReadOnlyDictionary<string, Card> Cards => _cards;

        private static Dictionary<string, Card> _cards = new(); // <CardData.name (Card.id), Card>
        private static Dictionary<Tuple<Card, int>, CardData> _cardDatas = new(); // <<Card, level>, CardData>

        [SerializeField] private UnityEvent _onCardDatabaseLoaded;

        private static bool isInitialized = false;
        private void Awake()
        {
            if(isInitialized)
            {
                Destroy(gameObject);
                return;
            }
            isInitialized = true;
        }
        
        private void Start()
        {
            JsonParser.Instance().LoadDecks();
            JsonParser.Instance().LoadCardCollection();
            JsonParser.Instance().LoadCardInstances();

            Load();

            DeckManager.LoadPlayerDeck();

            _onCardDatabaseLoaded?.Invoke();
        }

        // TODO : fill card data
        private void Load()
        {
            foreach (var cardData in CardCollection.Instance().cardCollection)
            {
                if(_cards.ContainsKey(cardData.unit.name))
                {
                    _cardDatas.Add(new(_cards[cardData.unit.name], cardData.unit.level), cardData);
                    continue;
                }

                var card = new Card();
                
                card.id = cardData.unit.name;
                card.cost = cardData.needElixir;
                card.gridSize = new(cardData.gridSize.top, cardData.gridSize.down, cardData.gridSize.left, cardData.gridSize.right);

                _cards.Add(cardData.unit.name, card);
                _cardDatas.Add(new(card, cardData.unit.level), cardData);
            }
        }

        public static CardData GetCardData(Card card, int level)
        {
            return _cardDatas[new(card, level)];
        }

        public static Card GetCard(string name)
        {
            return _cards[name];
        }

        public static Card GetCard (CardData cardData)
        {
            return _cards[cardData.unit.name];
        }
    }
}
