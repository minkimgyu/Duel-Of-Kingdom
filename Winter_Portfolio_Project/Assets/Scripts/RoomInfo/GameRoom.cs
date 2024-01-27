using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WPP.Network;

namespace WPP.RoomInfo
{
    internal class GameRoom
    {
        private static GameRoom _instance;
        public int roomID { get; set; }
        public EndPoint opponentAddress { get; set; }
        public static GameRoom Instance()
        {
            if( _instance == null)
            {
                _instance = new GameRoom();
            }
            return _instance;
        }
    }
}
