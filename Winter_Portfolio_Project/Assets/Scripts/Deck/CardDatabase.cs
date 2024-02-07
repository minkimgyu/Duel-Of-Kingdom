using Newtonsoft.Json;
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
        private static Dictionary<string, Dictionary<int, int>> _unitDataId = new(); // <CardData.name (Card.id), <level, UnitData.id>>

        [SerializeField] private UnityEvent _onCardDatabaseLoaded;
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
                var card = new Card();
                
                card.id = cardData.unit.name;
                card.cost = cardData.needElixir;
                card.gridSize = new(cardData.gridSize.top, cardData.gridSize.down, cardData.gridSize.left, cardData.gridSize.right);

                if(_cards.TryAdd(cardData.unit.name, card))
                {
                    _unitDataId.Add(cardData.unit.name, new());
                }
                _unitDataId[cardData.unit.name][cardData.unit.level] = cardData.unit.id;
            }
        }

        public static int GetUnitDataID(Card card, int level)
        {
            return _unitDataId[card.id][level];
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
