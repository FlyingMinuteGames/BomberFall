using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientHandler
{
    public static Client current;
    public static Dictionary<Opcode, OpcodeMgr._HandlePacket> _handlers = new Dictionary<Opcode, OpcodeMgr._HandlePacket>()
    {
            {Opcode.MSG_PLAYER_MOVE, HandleMovePlayer},
            {Opcode.SMSG_CREATE_PLAYER,HandleCreatePlayer},
            {Opcode.SMSG_BOMB_EXPLODE, HandleBombExplode},
            {Opcode.SMSG_SEND_MAP,HandleSendMap},
            {Opcode.SMSG_INSTANTIATE_OBJ,HandleInstantiateObject},
            {Opcode.SMSG_PLAYER_CONNECTED,HandlePlayerConnected},
            {Opcode.SMSG_START_GAME,HandleStartGame},
            {Opcode.MSG_SEND_MESSAGE,HandleSendMessage},
            {Opcode.MSG_JUMP,HandleJump},
            {Opcode.SMSG_CHANGE_PHASE,HandleChangePhase}
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
    }

    public static void HandleCreatePlayer(Packet p)
    {
        int guid; byte flags;
        guid = p.ReadInt();
        flags = p.ReadByte();

    }

    public static void HandleBombExplode(Packet p)
    {
        int x, y;
        x = p.ReadInt();
        y = p.ReadInt();
        GameMgr.Instance.maps.ExplodeAt(new IntVector2(x, y), 2);
    }

    public static void HandlePlayerConnected(Packet p)
    {
        int session = p.ReadInt();
        p.ReadInt(); // unused
        int guid = p.ReadInt();
        if (current.Session < 0)
        {
            current.Session = session;
            current.Guid = guid;
        }
    }

    public static void HandleSendMap(Packet p)
    {
        Debug.Log("handle maps "+p.Size);
        byte[] buffer = new byte[p.Size];
        p.ReadBuffer(buffer);
        string str = "";
        foreach (byte b in buffer)
            str += "."+b;
        Debug.Log(str);
        current.LoadMap(buffer);
    }

    public static void HandleInstantiateObject(Packet p)
    {
        int count = p.Size / 16, guid,type;
        float x,y,z = 0;

        GameMgr gmgr = GameMgr.Instance;
        for (var i = 0; i < count; i++)
        {
            guid = p.ReadInt();
            type = p.ReadInt();
            x =p.ReadFloat();
            y = p.ReadFloat();
            if (type == (int)(GOType.GO_PLAYER))
                z = 0.5150594f;
            gmgr.Spawn((GOType)type, new Vector3(x, z, y), guid);
            if (type == (int)(GOType.GO_BOMB))
            {
                GameObject go = ObjectMgr.Instance.get(guid);
                go.GetComponent<BombScript>().StartScript(
                    () => { ;}
                    , () =>
                    {
                        GameMgr.Instance.Despawn(GOType.GO_BOMB, guid);
                    });

            }
        }
    }

    public static void HandleStartGame(Packet p)
    {
        Debug.Log("START GAME");
        GameMgr.Instance.game_started = true;
        GameObject.Find("OrthoCamera").GetComponent<MainMenuScript>().active = false;
        HUD hud = GameObject.Find("HUD").GetComponent<HUD>();
        hud.Init();

    }

    public static void HandleSendMessage(Packet p)
    {
        string name, message;
        name = p.ReadString();
        message = p.ReadString();
        if (!GameMgr.Instance.game_started)
        {
            MainMenuScript menu = GameObject.Find("MenuCam").GetComponent<MainMenuScript>();
            menu.AddMessage(name, message);
        }
    }

    

    public static void HandleChangePhase(Packet p)
    {
        WorldState state;
        state = (WorldState)p.ReadInt();
        GameMgr.Instance.ChangePhase(state);
    }

    public static void HandleJump(Packet p)
    {
        int guid;
        Vector3 start_pos;
        guid = p.ReadInt();
        start_pos = p.ReadVector3();
        GameObject obj;
        if ((obj = ObjectMgr.Instance.get(guid)) != null)
        {
            obj.SendMessage("RecvJump",start_pos);
        }
    }
}
