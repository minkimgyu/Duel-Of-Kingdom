using System;
using MySql.Data.MySqlClient;
using WPP.ClashRoyale_Server.Protocol.Server;
using WPP.ClashRoyale_Server.Data.ClientInfo.Tower;
using WPP.ClashRoyale_Server.Data.ClientInfo.Account;
using WPP.ClashRoyale_Server.Data.ClientInfo.Deck;
using WPP.ClashRoyale_Server.Data.ClientInfo.CardData;
using WPP.ClashRoyale_Server.Data.Collection;
using WPP.ClashRoyale_Server.Data.Units;
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
                        int? attack_building_id = reader["attack_building_id"] is DBNull ? (int?)null : Convert.ToInt32(reader["attack_building_id"]);
                        int? spawn_building_id = reader["spawn_building_id"] is DBNull ? (int?)null : Convert.ToInt32(reader["spawn_building_id"]);
                        int? spell_id = reader["spell_id"] is DBNull ? (int?)null : Convert.ToInt32(reader["spell_id"]);

                        CardType type;
                        int unit_id;
                        if (troop_id != null)
                        {
                            type = CardType.troop;
                            unit_id = (int)troop_id;
                        }
                        else if(attack_building_id != null)
                        {
                            type = CardType.attack_building;
                            unit_id = (int)attack_building_id;
                        }
                        else if (spawn_building_id != null)
                        {
                            type = CardType.spawn_building;
                            unit_id = (int)spawn_building_id;
                        }
                        else if (spell_id != null)
                        {
                            type = CardType.spell;
                            unit_id = (int)spell_id;
                        }
                        else
                        {
                            type = 0;
                            unit_id = 0;
                        }

                        Enum.TryParse<CardRarity>((string)reader["rarity"], out CardRarity rarity);
                        int needElixir = Convert.ToInt32(reader["needElixir"]);
                        string gridSizeName = (string)(reader["grid_size_name"]);

                        GridSize gridSize = new GridSize();
                        if (String.Equals(gridSizeName, "troop_default")) gridSize.TroopGrid();
                        if(String.Equals(gridSizeName, "building_default")) gridSize.BuildingGrid();

                        reader.Close();

                        Card card = new Card(card_id, unit_id, type, rarity, needElixir, gridSize);
                        return card;
                    }
                }
            }

            return null;
        }

        public Unit FindUnit(CardType type, int unit_id)
        {
            string tableName;
            string idName;

            // selecting table name
            switch(type)
            {
                case CardType.troop:
                    tableName = "troop";
                    idName = "troop_id";
                    break;
                case CardType.attack_building:
                    tableName = "attack_building";
                    idName = "attack_building_id";
                    break;
                case CardType.spawn_building:
                    tableName = "spawn_building";
                    idName = "spawn_building_id";
                    break;
                case CardType.spell:
                    tableName = "spell";
                    idName = "spell_id";
                    break;
                default:
                    tableName = "";
                    idName = "";
                    break;
            }

            string query = "SELECT * FROM " + tableName + " WHERE " + idName + " = '" + unit_id + "'";
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string name = (string)(reader["name"]);
                int level = Convert.ToInt32(reader["level"]);
                Unit unit;

                switch (type)
                {
                    case CardType.troop:
                        Troop troop = new Troop(unit_id, name, level);
                        troop.hitpoints = Convert.ToInt32(reader["hitpoints"]);
                        troop.damage = Convert.ToSingle(reader["damage"]);
                        troop.hit_speed = Convert.ToSingle(reader["hit_speed"]);
                        troop.range = Convert.ToSingle(reader["range"]);

                        unit = troop;
                        break;
                    case CardType.attack_building:
                        AttackBuilding attackBuilding = new AttackBuilding(unit_id, name, level);
                        attackBuilding.hitpoints = Convert.ToSingle(reader["hitpoints"]);
                        attackBuilding.damage = Convert.ToSingle(reader["damage"]);
                        attackBuilding.life_time = Convert.ToSingle(reader["life_time"]);
                        attackBuilding.hit_speed = Convert.ToSingle(reader["hit_speed"]);
                        attackBuilding.range = Convert.ToSingle(reader["range"]);

                        unit = attackBuilding;
                        break;
                    case CardType.spawn_building:
                        SpawnBuilding spawnBuilding = new SpawnBuilding(unit_id, name, level);
                        spawnBuilding.hitpoints = Convert.ToSingle(reader["hitpoints"]);
                        spawnBuilding.life_time = Convert.ToSingle(reader["life_time"]);
                        spawnBuilding.spawn_unit_id = Convert.ToInt32(reader["spawn_unit_id"]);
                        spawnBuilding.spawn_unit_count = Convert.ToInt32(reader["spawn_unit_count"]);

                        unit = spawnBuilding;
                        break;
                    case CardType.spell:
                        Spell spell = new Spell(unit_id, name, level);

                        unit = spell;
                        break;
                    default:
                        unit = null;
                        break;
                }

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
            int account_id = FindAcountID(username);
            int king_tower_id = 0;
            int left_princess_id = 0;
            int right_princess_id = 0;
            for (int i = 0; i < Constants.MAXIMUM_TOWERS; i++)
            {
                switch(i)
                {
                    case 0:
                        king_tower_id = FindTowerID(TowerType.king_tower, 1);
                        break;
                    case 1:
                        left_princess_id = FindTowerID(TowerType.princess_tower, 1);
                        break;
                    case 2:
                        right_princess_id = FindTowerID(TowerType.princess_tower, 1);
                        break;
                    default:
                        break;
                }
            }

            string query = "INSERT INTO tower_instance(account_id, king_tower_id, left_princess_tower_id, right_princess_tower_id) VALUES('" +
                            account_id + "','" +
                            king_tower_id + "','" +
                            left_princess_id + "','" +
                            right_princess_id + "')";

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
