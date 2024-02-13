using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.Network
{
    public enum Server_PacketTagPackages
    {
        S_LOAD_CARD_COLLECTION,
        S_ACCEPT_REGISTER_ACCOUNT,
        S_REJECT_REGISTER_ACCOUNT,

        S_ACCEPT_LOGIN,
        S_REJECT_LOGIN,

        S_REQUEST_PLAY_GAME,

        S_ALERT_OVER_TIME,

        S_REQUEST_END_GAME,

        HOLE_PUNCHING,
        S_REQUEST_RELAY,
        TURN_ON,
    }

    public enum Client_PacketTagPackages
    {
        C_REQUEST_INITIAL_DATA,
        C_REQUEST_REGISTER_ACCOUNT,
        C_REQUEST_LOGIN,
        C_REQUEST_ENTER_ROOM,

        C_DESTROY_OPPONENT_KING_TOWER,
        C_DESTROY_OPPONENT_LEFT_TOWER,
        C_DESTROY_OPPONENT_RIGHT_TOWER,

        C_REQUEST_HOLE_PUNCHING,
        C_REQUEST_RELAY,
        C_REQUEST_UPDATE_DECKS,
    }

    public enum Peer_PacketTagPackages
    { 
        DAMAGE_KT = 30,
        DAMAGE_LPT,
        DAMAGE_RPT,
        SPAWN_CARD,
        SPAWN_TOWER,
    }
}