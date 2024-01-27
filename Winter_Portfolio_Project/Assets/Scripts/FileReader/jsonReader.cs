using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using WPP.ClientInfo.CardData;
using WPP.Collection;

namespace WPP.FileReader
{
    class JsonParser
    {
        private static JsonParser _instance;
        private string _jsonFilePath;
        private string _jsonData;

        public static JsonParser Instance()
        {
            if( _instance == null )
            {
                _instance = new JsonParser();
            }
            return _instance;
        }

        public JsonParser() 
        {
            _jsonFilePath = "Assets\\GameFile\\card_collection.json";
            _jsonData = File.ReadAllText(_jsonFilePath);
        }

        public Card FindCard(int card_id)
        {
            CardCollection cardCollection = JsonConvert.DeserializeObject<CardCollection>(_jsonData);

            Card card = cardCollection.FindCard(card_id);
            if( card == null )
            {
                return null;
            }
            Console.WriteLine($"Card with id {card_id} found: {JsonConvert.SerializeObject(card)}");
            return card;
        }
    }
}
