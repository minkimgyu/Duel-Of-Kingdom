using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using WPP.ClashRoyale_Server.Protocol.Server;
using WPP.ClashRoyale_Server.Database.ClientInfo;
using WPP.ClashRoyale_Server.Database.ClientInfo.Tower;
using System.Net.NetworkInformation;

namespace WPP.ClashRoyale_Server.Database
{
    class DatabaseManager
    {
        private static DatabaseManager _instance = null;

        private MySqlConnection _mySqlConnection;
        private MySqlCommand _cmd;
        private MySqlDataReader _reader;

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
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void InitializeTableSetting()
        {
            string query = "ALTER TABLE account AUTO_INCREMENT = 1";

            try
            {
                _cmd = new MySqlCommand(query, _mySqlConnection);
                _cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public int FindID(string username)
        {
            string query = "SELECT id FROM account WHERE username = '" + username + "'";
            _cmd = new MySqlCommand(query, _mySqlConnection);

            _reader = _cmd.ExecuteReader();

            if (_reader.Read())
            {
                int id = Convert.ToInt32(_reader["id"]);
                _reader.Close();
                return id;
            }
            return -1;
        }

        public void AddAccount(string username, string password)
        {
            /*
            Random random = new Random();
            int trophy = random.Next(400);
            ServerTCP.Instance().clients[clientID].accountInfo.trophy = trophy;
            */

            string query = "INSERT INTO account(username, password, gold, level, exp, trophy) VALUES('" +
                            username + "','" +
                            password + "','" +
                            0 + "','" +
                            1 + "','" +
                            0 + "','" +
                            0 + "')";

            try
            {
                _cmd = new MySqlCommand(query, _mySqlConnection);
                _cmd.ExecuteNonQuery();
            }
            catch(Exception e )
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void AddTowers(string username, Towers towers)
        {
            for(int i=0; i< towers.towers.Count; i++)
            {
                int id = FindID(username);
                string query = "INSERT INTO towers(id, type, level, hp) VALUES('" +
                            id + "','" +
                           (TowerType)towers.towers[i].type + "','" +
                           towers.towers[i].level + "','" +
                           towers.towers[i].hp + "')";

                try
                {
                    _cmd = new MySqlCommand(query, _mySqlConnection);
                    _cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void UpdateAccountFromDatabase(int clientID, string username)
        {
            string query = "SELECT * FROM account WHERE username = '" + username + "'";
            _cmd = new MySqlCommand(query, _mySqlConnection);

            _reader = _cmd.ExecuteReader();

            if (_reader.Read())
            {
                int gold = Convert.ToInt32(_reader["gold"]);
                int level = Convert.ToInt32(_reader["level"]);
                int exp = Convert.ToInt32(_reader["exp"]);
                int trophy = Convert.ToInt32(_reader["trophy"]);

                ServerTCP.Instance().clients[clientID].accountInfo.username = username;
                ServerTCP.Instance().clients[clientID].accountInfo.gold = gold;
                ServerTCP.Instance().clients[clientID].accountInfo.level = level;
                ServerTCP.Instance().clients[clientID].accountInfo.exp = exp;
                ServerTCP.Instance().clients[clientID].accountInfo.trophy = trophy;

                _reader.Close();
                return;
            }
        }

        public void UpdateTowersFromDatabase(int clientID, string username)
        {
            int id = FindID(username);

            string query = "SELECT * FROM towers WHERE id = '" + id + "'";
            _cmd = new MySqlCommand(query, _mySqlConnection);

            _reader = _cmd.ExecuteReader();

            for (int i = 0; i < ServerTCP.Instance().clients[clientID].towers.towers.Count; i++)
            {
                _reader.Read();

                Enum.TryParse<TowerType>((string)_reader["type"], out TowerType type);
                int level = Convert.ToInt32(_reader["level"]);
                int hp = Convert.ToInt32(_reader["hp"]);

                Tower tower = ServerTCP.Instance().clients[clientID].towers.towers[i];

                tower.type = type;
                tower.level = level;
                tower.hp = hp;
            }

            _reader.Close();
            return;
        }

        public bool CheckUsernameExists(string username)
        {
            string query = "SELECT username FROM account WHERE username = '" + username + "'";
            _cmd = new MySqlCommand(query, _mySqlConnection);
            _reader = _cmd.ExecuteReader();
            if (_reader.HasRows)
            {
                _reader.Close();
                return true;
            }
            _reader.Close();
            return false;
        }

        public bool HandleLogin(string username, string password)
        {
            string query = "SELECT username, password FROM account WHERE username = '" + username + "'" + " AND " +
                            "password = '" + password + "'";

            _cmd = new MySqlCommand(query, _mySqlConnection);
            _reader = _cmd.ExecuteReader();

            if (_reader.HasRows)
            {
                _reader.Close();
                return true;
            }
            _reader.Close();
            return false;
        }

    }
}
