using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPP.ClashRoyale_Server.Protocol
{
    public enum Server_PacketTagPackages
    {
        S_ACCEPT_REGISTER_ACCOUNT = 1,
        S_REJECT_REGISTER_ACCOUNT,

        S_ACCEPT_LOGIN,
        S_REJECT_LOGIN,

        S_REQUEST_PLAY_GAME,

        S_ALERT_DOUBLE_ELIXIR_TIME,
        S_ALERT_SUDDEN_DEATH_TIME,

        S_REQUEST_END_GAME,
    }

    public enum Client_PacketTagPackages
    {
        C_REQUEST_REGISTER_ACCOUNT = 1,
        C_REQUEST_LOGIN,
        C_REQUEST_ENTER_ROOM,

        C_DESTROY_OPPONENT_KING_TOWER,
        C_DESTROY_OPPONENT_LEFT_TOWER,
        C_DESTROY_OPPONENT_RIGHT_TOWER,
    }
}
