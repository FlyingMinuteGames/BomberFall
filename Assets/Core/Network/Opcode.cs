using UnityEngine;
using System.Collections;



/*
 *  - MSG -> Server/Client message
 *  - CMSG -> Client Message
 *  - SMSG -> Server Message
 */
public enum Opcode
{
    MSG_PLAYER_MOVE,
    CMSG_PLAYER_DROP_BOMB,
    SMSG_PLAYER_DIE,
    SMSG_PLAY_ANNOUNCEMENT,
    SMSG_SEND_MAPS_DATA,
    SMSG_CREATE_PLAYER,
    CMSG_CONNECT,
    SMSG_PLAYER_CONNECTED,
    SMSG_SEND_MAP,
    SMSG_INSTANTIATE_OBJ,
    SMSG_BOMB_EXPLODE,
    SMSG_START_GAME,
    MSG_SEND_MESSAGE,
    MSG_UPDATE_POSITION, // no handle, preparation for movement interpolation
    MSG_JUMP,
    SMSG_CHANGE_PHASE,
    SMSG_OFF_POWER_PICK_UP,
    SMSG_DESPAWN,
    CMSG_OFF_POWER_USE,
    SMSG_SPEED_UP,
    SMSG_OFF_POWER_USED,
    SMSG_UPDATE_SCORES,
    SMSG_PLAYER_DESPAWN,
    SMSG_GAME_ENDED,
    SMSG_PLAYER_RESPAWN
}
