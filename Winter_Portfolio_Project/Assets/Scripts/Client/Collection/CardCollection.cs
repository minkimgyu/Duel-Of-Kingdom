using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPP.ClientInfo.Card;

namespace WPP.Collection
{
    [Serializable]
    internal class CardCollection
    {
        private static CardCollection _instance;

        public List<CardData> cardCollection;

        public static CardCollection Instance()
        {
            if( _instance == null )
            {
                _instance = new CardCollection();
            }
            return _instance;
        }

        public CardCollection()
        {
            cardCollection = new List<CardData>();
        }

        public void InitializeFromJson(string jsonData)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            _instance = JsonConvert.DeserializeObject<CardCollection>(jsonData, settings);
        }

        public void AddCard(CardData card)
        {
            cardCollection.Add(card);
        }

        public CardData FindCard(int card_id)
        {
            foreach (CardData card in cardCollection)
            {
                if (card.id == card_id)
                    return card;
            }
            return null;
        }

        public CardData FindCard(string name, int level)
        {
            foreach (CardData card in cardCollection)
            {
                if (card.unit._name == name && card.unit._level == level)
                    return card;
            }
            return null;
        }

        public CardData FindCard(int unitId, int level)
        {
            foreach (CardData card in cardCollection)
            {
                if (card.unit._id == unitId && card.unit._level == level)
                    return card;
            }
            return null;
        }
    }
}
