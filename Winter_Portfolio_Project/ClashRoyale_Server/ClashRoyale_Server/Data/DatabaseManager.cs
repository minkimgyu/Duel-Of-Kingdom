using System;
using MySql.Data.MySqlClient;
using WPP.ClashRoyale_Server.Protocol.Server;
using WPP.ClashRoyale_Server.Data.ClientInfo.Tower;
using WPP.ClashRoyale_Server.Data.ClientInfo.Account;
using WPP.ClashRoyale_Server.Data.ClientInfo.Deck;
using WPP.ClashRoyale_Server.Data.ClientInfo.CardData;
using WPP.ClashRoyale_Server.Data.Collection;
using System.Security.Cryptography;
using ClashRoyale_Server.Database.Units;
using System.IO.Ports;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics.Eventing.Reader;

namespace WPP.ClashRoyale_Server.Data
{
    class DatabaseManager
    {
        private static DatabaseManager _instance = null;

        private MySqlConnection _mySqlConnection;

        private static string _server = "127.0.0.1";
        private static string _server_port = "3306";
        private static string _db_id = "root";
        private static string _db_pw = "0000";
        private static string _db_name = "clashroyale";

        string strConn = string.Format("server={0};port={1};database={2};uid={3};pwd={4};SSL Mode=None;", _server, _server_port, _db_name, _db_id, _db_pw);

        public static DatabaseManager Instance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseManager();
            }
            return _instance;
        }

        public void InitializeMySQLServer()
        {
            InitializeMySqlConnection();
            ConnectToMySqlServer();
        }

        private void InitializeMySqlConnection()
        {
            try
            {
                _mySqlConnection = new MySqlConnection(strConn);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectToMySqlServer()
        {
            try
            {
                _mySqlConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void CloseConnection()
        {
            try
            {
                _mySqlConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public int FindAcountID(string username)
        {
            string storedProcedureName = "GetAccount";
            MySqlCommand cmd = new MySqlCommand(storedProcedureName, _mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@in_username", username);

            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int id = Convert.ToInt32(reader["account_id"]);
                reader.Close();
                return id;
            }
            reader.Close();
            return -1;
        }

        public int FindTowerID(TowerType type, int level)
        {
            string query = "SELECT tower_id FROM tower WHERE type = '" + type + "' AND level = '" + level + "'";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int tower_id = Convert.ToInt32(reader["tower_id"]);
                reader.Close();
                return tower_id;
            }
            reader.Close();
            return -1;
        }

        public Tower FindTower(int towerID)
        {
            string query = "SELECT * FROM tower WHERE tower_id = '" + towerID + "'";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Enum.TryParse<TowerType>((string)reader["type"], out TowerType type);
                int level = Convert.ToInt32(reader["level"]);
                int hp = Convert.ToInt32(reader["hp"]);

                Tower tower = new Tower(type, level, hp);
                reader.Close();
                return tower;
            }
            reader.Close();
            return null;
        }

        public Card FindCard(int card_id)
        {
            string storedProcedureName = "GetCard";

            using (MySqlCommand cmd = new MySqlCommand(storedProcedureName, _mySqlConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@in_card_id", card_id);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int? troop_id = reader["troop_id"] is DBNull ? (int?)null : Convert.ToInt32(reader["troop_id"]);
                        int? building_id = reader["building_id"] is DBNull ? (int?)null : Convert.ToInt32(reader["building_id"]);
                        int? spell_id = reader["spell_id"] is DBNull ? (int?)null : Convert.ToInt32(reader["spell_id"]);

                        CardType type;
                        if (troop_id != null)
                        {
                            type = CardType.Troop;
                        }
                        else if(building_id != null)
                        {
                            type = CardType.Building;
                        }
                        else if (spell_id != null)
                        {
                            type = CardType.Spell;
                        }else
                        {
                            type = 0;
                        }


                        Enum.TryParse<CardRarity>((string)reader["rarity"], out CardRarity rarity);
                        int needElixir = Convert.ToInt32(reader["needElixir"]);

                        reader.Close();

                        // girdSize 추가 필요
                        Card card = new Card(card_id, type, rarity, needElixir, new Vector2Int().One());
                        return card;
                    }
                }
            }

            return null;
        }

        public Unit FindUnit(int unitID)
        {
            string query = "SELECT * FROM unit WHERE unit_id = '" + unitID + "'";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int unitId = Convert.ToInt32(reader["unit_id"]);
                Enum.TryParse<UnitType>((string)reader["type"], out UnitType unitType);
                int level = Convert.ToInt32(reader["level"]);

                Unit unit = new Unit(unitId, unitType, level);

                reader.Close();
                return unit;
            }
            reader.Close();
            return null;
        }

        public void AddAccount(string username, string password)
        {
            string query = "INSERT INTO account(username, password, gold, level, exp, trophy) VALUES('" +
                            username + "','" +
                            password + "','" +
                            0 + "','" +
                            1 + "','" +
                            0 + "','" +
                            0 + "')";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void AddTowers(string username)
        {
            for (int i = 0; i < Constants.MAXIMUM_TOWERS; i++)
            {
                int account_id = FindAcountID(username);
                int tower_id;

                switch(i)
                {
                    case 0:
                        tower_id = FindTowerID(TowerType.king_tower, 1);
                        break;
                    case 1:
                        tower_id = FindTowerID(TowerType.left_princess_tower, 1);
                        break;
                    case 2:
                        tower_id = FindTowerID(TowerType.right_princess_tower, 1);
                        break;
                    default:
                        tower_id = 0;
                        break;
                }

                string query = "INSERT INTO tower_instance(account_id, tower_id) VALUES('" +
                            account_id + "','" +
                            tower_id + "')";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void AddDecks(string username)
        {
            int account_id = FindAcountID(username);

            for (int i = 0; i < Constants.MAXIMUM_DECKS; i++)
            {
                string query = "INSERT INTO deck_instance (account_id, deck_id, card_id_1, card_id_2, card_id_3, card_id_4, card_id_5, card_id_6, card_id_7, card_id_8) VALUES('" +
                            account_id + "','" +
                            (i + 1) + "','" +
                            1 + "','" +
                            1 + "','" +
                            1 + "','" +
                            1 + "','" +
                            1 + "','" +
                            1 + "','" +
                            1 + "','" +
                            1 + "')";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public ClientAccount LoadAccount(int clientID, string username)
        {
            string query = "SELECT * FROM account WHERE username = '" + username + "'";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string password = Convert.ToString(reader["password"]);
                int gold = Convert.ToInt32(reader["gold"]);
                int level = Convert.ToInt32(reader["level"]);
                int exp = Convert.ToInt32(reader["exp"]);
                int trophy = Convert.ToInt32(reader["trophy"]);

                ServerTCP.Instance().clients[clientID].accountInfo.username = username;
                ServerTCP.Instance().clients[clientID].accountInfo.password = password;
                ServerTCP.Instance().clients[clientID].accountInfo.gold = gold;
                ServerTCP.Instance().clients[clientID].accountInfo.level = level;
                ServerTCP.Instance().clients[clientID].accountInfo.exp = exp;
                ServerTCP.Instance().clients[clientID].accountInfo.trophy = trophy;
            }

            reader.Close();
            return ServerTCP.Instance().clients[clientID].accountInfo;
        }

        public Towers LoadTowers(int clientID, string username)
        {
            int id = FindAcountID(username);

            string query = "SELECT * FROM tower WHERE tower_id IN (SELECT tower_id FROM tower_instance WHERE account_id = '" + id + "')";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

            MySqlDataReader reader = cmd.ExecuteReader();

            for (int i = 0; i < Constants.MAXIMUM_TOWERS; i++)
            {
                if (reader.Read())
                {
                    Enum.TryParse<TowerType>((string)reader["type"], out TowerType type);
                    int level = Convert.ToInt32(reader["level"]);
                    int hp = Convert.ToInt32(reader["hp"]);

                    Tower tower = new Tower(type, level, hp);

                    switch (i)
                    {
                        case 0:
                            ServerTCP.Instance().clients[clientID].towers.kingTower = tower;
                            break;
                        case 1:
                            ServerTCP.Instance().clients[clientID].towers.leftPrincessTower = tower;
                            break;
                        case 2:
                            ServerTCP.Instance().clients[clientID].towers.rightPrincessTower = tower;
                            break;
                    }
                }
            }
            reader.Close();
            return ServerTCP.Instance().clients[clientID].towers;
        }

        public Decks LoadDecks(int clientID, string username)
        {
            int id = FindAcountID(username);

            Decks decks = new Decks();
            int[] card_ids = new int[6];
            int deckId = 0;

            for (int i = 0; i < Constants.MAXIMUM_DECKS; i++)
            {
                string query = "SELECT * FROM deck_instance WHERE account_id = '" + id + "' AND deck_id = '" + (i + 1) + "'";
                MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    deckId = Convert.ToInt32(reader["deck_id"]);
                    for (int j = 0; j < card_ids.Length; j++)
                    {
                        card_ids[j] = Convert.ToInt32(reader["card_id_" + (j + 1)]);
                    }
                }
                reader.Close();

                Deck deck = new Deck(deckId);
                for (int j = 0; j < card_ids.Length; j++)
                {
                    deck.AddCard(card_ids[j]);
                }

                decks.AddDeck(deck);
            }
            ServerTCP.Instance().clients[clientID].decks = decks;
            return ServerTCP.Instance().clients[clientID].decks;
        }

        public CardCollection LoadCardCollection(int clientID, string username)
        {
            CardCollection cardCollection = new CardCollection();

            string query = "SELECT * FROM card";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);
            MySqlDataReader reader = cmd.ExecuteReader();

            int[] card_ids = new int[Constants.MAXIMUM_CARDS];

            for (int i = 0; i < Constants.MAXIMUM_CARDS; i++)
            {
                if (reader.Read())
                {
                    int card_id = Convert.ToInt32(reader["card_id"]);
                    card_ids[i] = card_id;
                }
            }
            reader.Close();

            for (int i = 0; i < Constants.MAXIMUM_CARDS; i++)
            {
                cardCollection.AddCard(FindCard(card_ids[i]));
            }

            return cardCollection;
        }

        public bool CheckRowExists(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }
        public bool CheckUsernameExists(string username)
        {
            string query = "SELECT username FROM account WHERE username = '" + username + "'";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }

        public bool CheckAccountExists(string username, string password)
        {
            string query = "SELECT username, password FROM account WHERE username = '" + username + "'" + " AND " +
                            "password = '" + password + "'";

            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }

    }
}
