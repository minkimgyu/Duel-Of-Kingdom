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

        S_REQUEST_HOLE_PUNCHING,
        S_REQUEST_RELAY,
        S_REQUEST_SYNCHRONIZATION,
        S_SEND_PING,
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
        C_CLOSE_CONNECTION,
        C_ANSWER_PING,
    }

    public enum Peer_PacketTagPackages
    {
        P_SEND_PING = 30,
        P_ANSWER_PING,
        P_REQUEST_SPAWN_CARD,
        P_REQUEST_SPAWN_TOWER,
        P_REQUEST_SPAWN_UNIT,
        P_REQUEST_SYNCHRONIZATION,
        P_REQUEST_DESTROY_UNIT,

        P_SEND_COMMANDS,
    }
}