using UnityEngine;
using System.Collections;

public class ServerHandler  {

    public static Server current;
    public static OpcodeMgr.HandlePacketStruct[] handlers = new OpcodeMgr.HandlePacketStruct[]{ 
            new OpcodeMgr.HandlePacketStruct(Opcode.MSG_PLAYER_MOVE, HandleMovePlayer),
            new OpcodeMgr.HandlePacketStruct(Opcode.CMSG_PLAYER_DROP_BOMB, HandleDropBomb),
            new OpcodeMgr.HandlePacketStruct(Opcode.CMSG_CONNECT,HandleConnect),
            new OpcodeMgr.HandlePacketStruct(Opcode.MSG_SEND_MESSAGE,HandleSendMessage)
    };

    public static void HandleMovePlayer(Packet p)
    {
        int guid, moveflag;
        Vector3 start_pos;
        GameObject obj;
        guid = p.ReadInt();
        moveflag = p.ReadInt();
        start_pos = p.ReadVector3();
        if (null == (obj = ObjectMgr.Instance.get(guid)))
            return;
        obj.SendMessage("OnRecvMove", new object[] { moveflag, start_pos });
        current.SendPacketBroadCast(p, p.Sender);
    }

    public static void HandleDropBomb(Packet p)
    {
        Vector3 pos = p.ReadVector3();
        current.SpawnBomb(pos);
    }

    public static void HandleConnect(Packet p)
    {
        int flag, session;
        flag = p.ReadInt();
        session = p.ReadInt();
        if ((flag & 1) != 0 && current.Session.ContainsKey(session))
            ;//ok reconnect player;
        else 
            current.RegisterPlayer(p.Sender);
        
    }

    public static void HandleSendMessage(Packet p)
    {
        string name, message;
        name = p.ReadString();
        message = p.ReadString();
        if(!GameMgr.Instance.game_started)
        {
            MainMenuScript menu = GameObject.Find("MenuCam").GetComponent<MainMenuScript>();
            menu.AddMessage(name, message);
            current.SendPacketBroadCast(p);
        }
    }
}
