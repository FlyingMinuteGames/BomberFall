using UnityEngine;
using System.Collections;



/*
 *
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

    

}
