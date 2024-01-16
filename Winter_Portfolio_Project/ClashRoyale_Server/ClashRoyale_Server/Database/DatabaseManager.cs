using System;
using MySql.Data.MySqlClient;
using WPP.ClashRoyale_Server.Protocol.Server;
using WPP.ClashRoyale_Server.Database.ClientInfo.Tower;
using WPP.ClashRoyale_Server.Database.ClientInfo.Account;
using WPP.ClashRoyale_Server.Database.ClientInfo.Deck;
using WPP.ClashRoyale_Server.Database.ClientInfo.CardData;

namespace WPP.ClashRoyale_Server.Database
{
    class DatabaseManager
    {
        private static DatabaseManager _instance = null;

        private MySqlConnection _mySqlConnection;

        private static string _server = "localhost";
        private static string _db_id = "root";
        private static string _db_pw = "";
        private static string _db_name = "clashroyale";

        string strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8;", _server, _db_id, _db_pw, _db_name);
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
            InitializeTableSetting();
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

        public void InitializeTableSetting()
        {
            string accountQuery = "ALTER TABLE account AUTO_INCREMENT = 1";
            string towerQuery = "ALTER TABLE tower AUTO_INCREMENT = 1";

            try
            {
                MySqlCommand cmd = new MySqlCommand(accountQuery, _mySqlConnection);
                cmd.ExecuteNonQuery();
                cmd = new MySqlCommand(towerQuery, _mySqlConnection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public int FindAcountID(string username)
        {
            string query = "SELECT account_id FROM account WHERE username = '" + username + "'";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

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

        public Card FindCard(int cardID)
        {
            string query = "SELECT * FROM card WHERE card_id = '" + cardID + "'";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int cardId = Convert.ToInt32(reader["card_id"]);
                int unitId = Convert.ToInt32(reader["unit_id"]);
                Enum.TryParse<CardRarity>((string)reader["rarity"], out CardRarity rarity);
                int needElixir = Convert.ToInt32(reader["needElixir"]);

                Card card = new Card(cardId, unitId, rarity, needElixir);

                reader.Close();
                return card;
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
                if (i == 0)
                {
                    tower_id = FindTowerID(TowerType.kingTower, 1);
                }
                else
                {
                    tower_id = FindTowerID(TowerType.princessTower, 1);
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
            for (int i = 0; i < Constants.MAXIMUM_DECKS; i++)
            {
                int account_id = FindAcountID(username);

                string query = "INSERT INTO deck_instance (account_id, deck_id, card_id_1, card_id_2, card_id_3, card_id_4, card_id_5, card_id_6) VALUES('" +
                            account_id + "','" +
                            (i+1) + "','" +
                            1 + "','" +
                            2 + "','" +
                            3 + "','" +
                            4 + "','" +
                            5 + "','" +
                            6 + "')";

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

                    switch(i)
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
            int[] cardIds = new int[6];
            int deckId = 0;

            for (int i = 0; i < Constants.MAXIMUM_DECKS; i++)
            {
                string query = "SELECT * FROM deck_instance WHERE account_id = '" + id + "' AND deck_id = '" + (i+1) + "'";
                MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    deckId = Convert.ToInt32(reader["deck_id"]);
                    for (int j = 0; j < cardIds.Length; j++)
                    {
                        cardIds[j] = Convert.ToInt32(reader["card_id_" + (j+1)]);
                    }
                }
                reader.Close();

                Deck deck = new Deck(deckId);
                for(int j=0; j<cardIds.Length; j++)
                {
                    deck.AddCard(FindCard(cardIds[j]));
                }

                decks.AddDeck(deck);
            }
            ServerTCP.Instance().clients[clientID].decks = decks;
            return ServerTCP.Instance().clients[clientID].decks;
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

        public bool HandleLogin(string username, string password)
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
