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

    public static Packet BuildPlayAnnouncementPacket(int announceId)
    { return null; }

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

    public static Packet BuildPlayerConnectPacket(int sessionId,int guid, int player_index)
    {
        Packet p = new Packet(12,(int)Opcode.SMSG_PLAYER_CONNECTED);
        p.Write(sessionId);
        p.Write(player_index);
        p.Write(guid);
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

    public static Packet BuildSpawnBomb(Vector3 pos)
    {
        Packet p = new Packet(3 * 4, Opcode.CMSG_PLAYER_DROP_BOMB);
        p.Write(pos); // need to by only 2 float
        return p;
    }

    public static Packet BuildBombExplode(IntVector2 pos, int radius = 2)
    {
        Packet p = new Packet(8, Opcode.SMSG_BOMB_EXPLODE);
        p.Write(pos.x);
        p.Write(pos.y);
        p.Write(radius);
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

    public static Packet BuildChangePhasePacket(WorldState state)
    {
        Packet p = new Packet(4, Opcode.SMSG_CHANGE_PHASE);
        p.Write((int)state);
        return p;
    }

    public static Packet BuildBindOffensiveItem(int guid, Config.PowerType powertype)
    {
        Packet p = new Packet(4+4, Opcode.SMSG_OFF_POWER_PICK_UP);
        p.Write(guid);
        p.Write((int)powertype);
        return p;
    }

    public static Packet BuildUseOffensiveItem(int guid, Vector3 pos)
    {
        Packet p = new Packet(4+4*3, Opcode.CMSG_OFF_POWER_USE);
        p.Write(guid);
        p.Write(pos);
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

}
