using System;
using System.Collections.Generic;
using System.Threading;
using WPP.ClashRoyale_Server;
using System.Timers;
using WPP.ClashRoyale_Server.Protocol.Server;
using MySql.Data.MySqlClient.Memcached;

namespace WPP.ClashRoyale_Server.Logic.Room
{
    class GameRoomManager
    {
        private static GameRoomManager _instance;
        private List<GameRoom> _gameRooms;
        private HashSet<GameRoom> _roomsToRemove;
        private int _gameRoomId;
        private System.Timers.Timer _removeTimer;
        public static GameRoomManager Instance()
        {
            if (_instance == null)
            {
                _instance = new GameRoomManager();
            }
            return _instance;
        }
        public GameRoomManager() {
            _gameRooms = new List<GameRoom>();
            _roomsToRemove = new HashSet<GameRoom>();
            _removeTimer = new System.Timers.Timer(10000);
            _removeTimer.Elapsed += RemoveInactiveRoom;
            _gameRoomId = 1;
        }

        public GameRoom FindGameRoom(int roomID)
        {
            foreach (var room in _gameRooms)
            {
                if(room.id == roomID)
                    return room;
            }
            return null;
        }

        public GameRoom CreateGameRoom()
        {
            GameRoom gameRoom = new GameRoom(_gameRoomId);
            ++_gameRoomId;
            return gameRoom;
        }

        public void EnterGameRoom(ClientObject client1, ClientObject client2)
        {
            GameRoom gameRoom = CreateGameRoom();
            _gameRooms.Add(gameRoom);
            gameRoom.AddClient(client1);
            gameRoom.AddClient(client2);
            if (gameRoom.CheckFull())
            {
                StartGame(client1.gameRoomID);
            }
            else
            {
                Console.WriteLine("roomID: {0} is full", _gameRoomId);
            }
            return;
        }

        public void StartGame(int roomID)
        {
            GameRoom gameRoom = FindGameRoom(roomID);
            gameRoom.StartGame();
        }

        public void RemoveInactiveRoom(Object source, ElapsedEventArgs e)
        {
            if (_gameRooms.Count == 0)
                return;

            foreach(GameRoom room in _gameRooms)
            {
                if(room.status == GameRoomStatus.INACTIVE)
                {
                    _roomsToRemove.Add(room);
                }
            }
            _gameRooms.RemoveAll(_roomsToRemove.Contains);
        }
    }
}
