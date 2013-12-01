using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerHandler  {

    public static Server current;
    public static Dictionary<Opcode,OpcodeMgr._HandlePacket> handlers =new Dictionary<Opcode,OpcodeMgr._HandlePacket>()
    { 
            {Opcode.MSG_PLAYER_MOVE, HandleMovePlayer},
            {Opcode.CMSG_PLAYER_DROP_BOMB, HandleDropBomb},
            {Opcode.CMSG_CONNECT,HandleConnect},
            {Opcode.MSG_SEND_MESSAGE,HandleSendMessage},
            {Opcode.MSG_JUMP,HandleJump},
            {Opcode.CMSG_OFF_POWER_USE,HandleOffPowerUse}
    };

    public static void HandleMovePlayer(Packet p)
    {
       
        int guid, moveflag;
        Vector3 start_pos;
        GameObject obj;
        guid = p.ReadInt();
        moveflag = p.ReadInt();
        start_pos = p.ReadVector3();
        if (null == (obj = ObjectMgr.Instance.Get(guid)))
            return;
        obj.SendMessage("OnRecvMove", new object[] { moveflag, start_pos });

        current.SendPacketBroadCast(p, p.Sender);
    }

    public static void HandleDropBomb(Packet p)
    {
        int guid = p.ReadInt();
        Vector3 pos = p.ReadVector3();
        GameObject controller = null;
        Debug.Log("bomb by " + guid);
        if ((controller = ObjectMgr.Instance.Get(guid)) != null)
            current.SpawnBomb(controller.GetComponent<BomberController>(),pos);
        Debug.Log("controller : " + controller);
    }

    public static void HandleConnect(Packet p)
    {
        int flag, session;
        flag = p.ReadInt();
        session = p.ReadInt();
        if ((flag & 1) != 0 && current.Session.ContainsKey(session))
            ;//ok reconnect player;
        else 
            current.RegisterPlayer(p.Sender,flag);
        
    }

    public static void HandleSendMessage(Packet p)
    {
        string name, message;
        name = p.ReadString();
        message = p.ReadString();
        if(!GameMgr.Instance.game_started)
        {
            MainMenuScript menu = GameObject.Find("OrthoCamera").GetComponent<MainMenuScript>();
            menu.AddMessage(name, message);
            current.SendPacketBroadCast(p);
        }
    }

    public static void HandleJump(Packet p)
    {
        int guid;
        Vector3 start_pos;
        guid = p.ReadInt();
        start_pos = p.ReadVector3();
        GameObject obj;
        if ((obj = ObjectMgr.Instance.Get(guid)) != null)
        {
            obj.SendMessage("RecvJump",start_pos);
            current.SendPacketBroadCast(p, p.Sender);
        }
    }

    public static void HandleOffPowerUse(Packet p)
    {
        Debug.Log("IN HANDLER");
        int guid;
        guid = p.ReadInt();


        GameObject go = ObjectMgr.Instance.Get(guid);
        BomberController bc = go.GetComponent<BomberController>();

        if (bc.hasOffensiveItem > -1)
        {
            switch (bc.hasOffensiveItem)
            {
                case (int)Config.PowerType.BRING_A_SW_TO_A_GF :
                    bc.Swing();
                    bc.hasOffensiveItem = -1;
                    GameMgr.Instance.s.SendPacketBroadCast(PacketBuilder.BuildUnbindOffensiveItem(guid, Config.PowerType.BRING_A_SW_TO_A_GF));
                    break;
            }
        }

    }

}
