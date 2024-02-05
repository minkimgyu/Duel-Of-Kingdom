#undef DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using WPP.ClientInfo.Card;
using WPP.Units;
using WPP.Collection;
using UnityEngine;
using WPP.ClientInfo.Deck;
using WPP.ClientInfo;
using WPP.ClientInfo.Account;
using WPP.ClientInfo.Tower;
using Newtonsoft.Json.Linq;

namespace WPP.FileReader
{
    class JsonParser
    {
        private static JsonParser _instance;
        private string _cardCollectionPath;
        private string _accountPath;
        private string _towersPath;
        private string _decksPath;
        private string _cardInstancesPath;
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
#if UNITY_EDITOR
            _cardCollectionPath = "Assets\\GameFile\\card_collection.json";
            _accountPath = "Assets\\GameFile\\account.json";
            _towersPath = "Assets\\GameFile\\towers.json";
            _decksPath = "Assets\\GameFile\\decks.json";
            _cardInstancesPath = "Assets\\GameFile\\cards.json";

#else
            _cardCollectionPath = Application.persistentDataPath + "/card_collection.json";
            _accountPath = Application.persistentDataPath + "/account.json";
            _towersPath = Application.persistentDataPath + "/towers.json";
            _decksPath = Application.persistentDataPath + "/decks.json";
            _cardInstancesPath = Application.persistentDataPath + "/cards.json";
#endif
        }

        public void LoadCardCollection()
        {
            _jsonData = File.ReadAllText(_cardCollectionPath);
            CardCollection.Instance().InitializeFromJson(_jsonData);
            return;
        }
        public void LoadAccount()
        {
            _jsonData = File.ReadAllText(_accountPath);
            ClientData.Instance().account = JsonConvert.DeserializeObject<AccountData>(_jsonData);
            return;
        }
        public void LoadTowers()
        {
            _jsonData = File.ReadAllText(_towersPath);
            ClientData.Instance().towers = JsonConvert.DeserializeObject<TowersData>(_jsonData);
            return;
        }

        public void LoadDecks()
        {
            _jsonData = File.ReadAllText(_decksPath);
            ClientData.Instance().decks = JsonConvert.DeserializeObject<DecksData>(_jsonData);
            return;
        }

        public void LoadCardInstances()
        {
            _jsonData = File.ReadAllText(_cardInstancesPath);
            ClientData.Instance().cards = JsonConvert.DeserializeObject<CardsData>(_jsonData);
            return;
        }
    }
}
