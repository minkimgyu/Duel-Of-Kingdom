using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Network
{
    public enum Server_PacketTagPackages
    {
        S_LOAD_CARD_COLLECTION = 1,
        S_ACCEPT_REGISTER_ACCOUNT,
        S_REJECT_REGISTER_ACCOUNT,

        S_ACCEPT_LOGIN,
        S_REJECT_LOGIN,

        S_REQUEST_PLAY_GAME,

        S_ALERT_OVER_TIME,

        S_REQUEST_END_GAME,
    }

    public enum Client_PacketTagPackages
    {
        C_REQUEST_REGISTER_ACCOUNT = 1,
        C_REQUEST_LOGIN,
        C_REQUEST_ENTER_ROOM,

        C_ALERT_WIN,
        C_ALERT_LOOSE
    }

    public enum Peer_PacketTagPackages
    { 
        TEST = 10,
        DAMAGE_KT,
        DAMAGE_LPT,
        DAMAGE_RPT
    }
}