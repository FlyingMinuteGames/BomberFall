using UnityEngine;
using System.Collections;
using System.IO;
public class PacketBuilder  {

    public static Packet BuildMovePlayerPacket(int guid, int moveFlag ,Vector3 pos)
    {
        Packet p = new Packet(4+4+4*3,(int)Opcode.MSG_PLAYER_MOVE);
        p.Write(guid);
        p.Write(moveFlag);
        p.Write(pos);
        return p;
    }

    public static Packet BuildConnectPacket(int flag, int sessionId)
    {
        Packet p = new Packet(4 * 2, (int)Opcode.CMSG_CONNECT);
        /* if flag == 0
         * 
         */
        p.Write(flag);
        p.Write(sessionId);
        return p;
    }

    public static Packet BuildPlayerConnectPacket(int sessionId,int guid, int player_index, GameIntel gintel)
    {
        Packet p = new Packet(12+1+4+1+4,(int)Opcode.SMSG_PLAYER_CONNECTED);
        p.Write(sessionId);
        p.Write(player_index);
        p.Write(guid);
        p.Write((byte)gintel.game_mode);
        p.Write(gintel.game_duration);
        p.Write((byte)gintel.nb_players);
        int powerUpEnable = 0;
        foreach (var power in gintel.power_ups)
            powerUpEnable |= 1 << (int)power;
        p.Write(powerUpEnable);
        

        return p;
    }

    public static Packet BuildSendMapPacket(Maps map)
    { 
        MemoryStream stream = new MemoryStream();
        map.SaveToStream(stream);
        Packet p = new Packet((int)stream.Length, Opcode.SMSG_SEND_MAP);
        p.Write(stream.ToArray());
        return p;
    }
    public static Packet BuildInstantiateObjPacket(byte[] values)
    {
        Packet p = new Packet(values.Length, Opcode.SMSG_INSTANTIATE_OBJ);
        p.Write(values);
        return p;
    }

    public static Packet BuildSpawnBomb(int guid,Vector3 pos)
    {
        Packet p = new Packet(4 * 4, Opcode.CMSG_PLAYER_DROP_BOMB);
        p.Write(guid);
        p.Write(pos); // need to by only 2 float
        return p;
    }

    public static Packet BuildBombExplode(int guid, IntVector2 pos, int radius = 2)
    {
        Packet p = new Packet(13, Opcode.SMSG_BOMB_EXPLODE);
        p.Write(guid);
        p.Write(pos.x);
        p.Write(pos.y);
        p.Write((byte)radius);
        return p;
    }

    public static Packet BuildStartGame()
    {
        Packet p = new Packet(0,Opcode.SMSG_START_GAME);
        return p;
    }

    public static Packet BuildSendMessage(string name, string message)
    {
        Packet p = new Packet((name.Length + message.Length + 2) * 2, Opcode.MSG_SEND_MESSAGE);
        p.Write(name);
        p.Write(message);
        return p;
    }

    public static Packet BuildJumpPacket(int guid, Vector3 pos)
    {
        Packet p = new Packet(4 + 4 * 3, Opcode.MSG_JUMP);
        p.Write(guid);
        p.Write(pos);
        return p;
    }

    public static Packet BuildChangePhasePacket(WorldState state, WorldStateExtra extra)
    {
        Packet p = new Packet(2, Opcode.SMSG_CHANGE_PHASE);
        p.Write((byte)state);
        p.Write((byte)extra);
        return p;
    }

    public static Packet BuildBindOffensiveItem(int guid, Config.PowerType powertype)
    {
        Packet p = new Packet(4+4, Opcode.SMSG_OFF_POWER_PICK_UP);
        p.Write(guid);
        p.Write((int)powertype);
        return p;
    }

    public static Packet BuildUnbindOffensiveItem(int guid, Config.PowerType powertype)
    {
        Packet p = new Packet(4, Opcode.SMSG_OFF_POWER_USED);
        p.Write(guid);
        p.Write((int)powertype);
        return p;
    }


    public static Packet BuildUseOffensiveItem(int guid)
    {
        Packet p = new Packet(4, Opcode.CMSG_OFF_POWER_USE);
        p.Write(guid);
        return p;
    }

    public static Packet BuildDespawn(int guid)
    {
        Packet p = new Packet(4, Opcode.SMSG_DESPAWN);
        p.Write(guid);
        return p;
    }


    public static Packet BuildPlayAnnouncePacket(Announce announce, byte variant, params string[] values)
    {
        //Calculate size of packet
        int size = 0;
        foreach (string str in values)
            size += (str.Length + 1) * 2;
        Packet p = new Packet(size+3,Opcode.SMSG_PLAY_ANNOUNCEMENT);
        p.Write((short)announce);
        p.Write(variant);
        foreach (string str in values)
            p.Write(str);
        return p;
    }

    public static Packet BuildSpeedUpPacket(int guid, int speedmult)
    {
        Packet p = new Packet(4 + 4, Opcode.SMSG_SPEED_UP);
        p.Write(guid);
        p.Write(speedmult);
        return p;
    }


    public static Packet BuildUpdateScoresPacket(int[] scores)
    {
        Packet p = new Packet(4 *scores.Length, Opcode.SMSG_UPDATE_SCORES);
        for (int i = 0, len = scores.Length; i < len; i++)
            p.Write(scores[i]);
        return p;
    }


    public static Packet BuildPlayerDespawn(int guid)
    {
        Packet p = new Packet(4 + 4, Opcode.SMSG_PLAYER_DESPAWN);
        p.Write(guid);
        return p;
    }

    public static Packet BuildSendEndOfGame(int gametype)
    {
        Packet p = new Packet(4 + 4, Opcode.SMSG_GAME_ENDED);
        p.Write(gametype);
        return p;
    }


    public static Packet BuildRespawnPacket(int guid, Vector3 respawn_pos)
    {
        Packet p = new Packet(16, Opcode.SMSG_PLAYER_RESPAWN);
        p.Write(guid);
        p.Write(respawn_pos);
        return p;
    }
}
