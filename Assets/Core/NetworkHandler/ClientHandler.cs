using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            {Opcode.SMSG_CHANGE_PHASE,HandleChangePhase},
            {Opcode.SMSG_OFF_POWER_PICK_UP, HandleOffensivePowerPickUp},
            {Opcode.SMSG_DESPAWN,HandleDespawn},
            {Opcode.SMSG_PLAY_ANNOUNCEMENT,HandlePlayAnnouncement}, 
            {Opcode.SMSG_SPEED_UP, HandlePlayerSpeedUp},
            {Opcode.SMSG_OFF_POWER_USED, HandleOffensivePowerDrop},
            {Opcode.SMSG_UPDATE_SCORES, HandleUpdateScores},
            {Opcode.SMSG_PLAYER_DESPAWN, HandlePlayerDespawn},
            {Opcode.SMSG_GAME_ENDED, HandleEndGame},
            {Opcode.SMSG_PLAYER_RESPAWN,HandleRespawn}
    };

    public static void HandleMovePlayer(Packet p)
    {
        if (current.Both)
            return;
        int guid, moveflag;
        Vector3 start_pos;
        GameObject obj;
        guid = p.ReadInt();
        moveflag = p.ReadInt();
        start_pos = p.ReadVector3();
        if (null == (obj = ObjectMgr.Instance.Get(guid)))
            return;
        obj.SendMessage("OnRecvMove", new object[] { moveflag, start_pos });
    }

    public static void HandleCreatePlayer(Packet p)
    {
        if (current.Both)
            return;
        int guid; byte flags;
        guid = p.ReadInt();
        flags = p.ReadByte();

    }

    public static void HandleBombExplode(Packet p)
    {
        if (current.Both)
            return;
        int x, y, guid, radius;
        guid = p.ReadInt();
        x = p.ReadInt();
        y = p.ReadInt();
        radius = p.ReadByte();
        GameObject obj;
        if ((obj = ObjectMgr.Instance.Get(guid)) != null)
            obj.SendMessage("ForceExplode");

        GameMgr.Instance.maps.ExplodeAt(guid, new IntVector2(x, y), radius);
    }

    public static void HandlePlayerConnected(Packet p)
    {
        if (current.Both)
            return;
        int session = p.ReadInt();
        p.ReadInt(); // unused
        int guid = p.ReadInt();

        int game_mode = p.ReadByte();
        float game_duration = p.ReadFloat();
        int nb_player = p.ReadByte();
        int powerup_enable = p.ReadInt();

        bool[] powerup = new bool[12];
        for (int i = 0, len = powerup.Length; i < len; i++)
            powerup[i] = (powerup_enable & 1 << i) != 0;

        GameIntel gintel = new GameIntel(game_duration, game_mode, powerup, nb_player, 0, false, false, "");
        GameMgr.Instance.gameIntel = gintel;
        if (current.Session < 0)
        {
            current.Session = session;
            current.Guid = guid;
        }
    }

    public static void HandleSendMap(Packet p)
    {
        if (current.Both)
            return;
        Debug.Log("handle maps " + p.Size);
        byte[] buffer = new byte[p.Size];
        p.ReadBuffer(buffer);
        string str = "";
        foreach (byte b in buffer)
            str += "." + b;
        Debug.Log(str);
        current.LoadMap(buffer);
    }

    public static void HandleInstantiateObject(Packet p)
    {
        if (current.Both)
            return;
        int count = p.Size / 17, guid, type, extra;
        float x, y, z = 0;

        GameMgr gmgr = GameMgr.Instance;
        for (var i = 0; i < count; i++)
        {
            guid = p.ReadInt();
            type = p.ReadByte();
            extra = p.ReadByte();
            x = p.ReadFloat();
            y = p.ReadFloat();
            Debug.Log("recv guid:" + guid + " type:" + (GOType)type + " extra:" + extra + " x:" + x + " y:" + y);
            if (type == (int)(GOType.GO_PLAYER))
                z = 0.5150594f;
            gmgr.Spawn((GOType)type, new Vector3(x, z, y), guid, extra);
            if (type == (int)(GOType.GO_BOMB))
            {
                GameObject go = ObjectMgr.Instance.Get(guid);
                go.GetComponent<BombScript>().StartScript(0);

            }
        }
    }

    public static void HandleStartGame(Packet p)
    {
        if (current.Both)
            return;
        Debug.Log("START GAME");
        GameMgr.Instance.game_started = true;
        GameObject.Find("OrthoCamera").GetComponent<MainMenuScript>().active = false;
        HUD hud = GameObject.Find("HUD").GetComponent<HUD>();
        hud.Init();

    }

    public static void HandleSendMessage(Packet p)
    {
        if (current.Both)
            return;
        string name, message;
        name = p.ReadString();
        message = p.ReadString();
        if (!GameMgr.Instance.game_started)
        {
            MainMenuScript menu = GameObject.Find("OrthoCamera").GetComponent<MainMenuScript>();
            menu.AddMessage(name, message);
        }
    }



    public static void HandleChangePhase(Packet p)
    {
        if (current.Both)
            return;
        WorldState state;
        WorldStateExtra extra;
        state = (WorldState)p.ReadByte();
        extra = (WorldStateExtra)p.ReadByte();
        GameMgr.Instance.ChangePhase(state, extra);
    }

    public static void HandleJump(Packet p)
    {
        if (current.Both)
            return;
        int guid;
        Vector3 start_pos;
        guid = p.ReadInt();
        start_pos = p.ReadVector3();
        GameObject obj;
        if ((obj = ObjectMgr.Instance.Get(guid)) != null)
        {
            obj.SendMessage("RecvJump", start_pos);
        }
    }

    public static void HandleOffensivePowerPickUp(Packet p)
    {
        int guid;
        Config.PowerType powertype;
        guid = p.ReadInt();
        powertype = (Config.PowerType)p.ReadInt();
        HUD hud = GameObject.Find("HUD").GetComponent<HUD>();
        hud.BindOffensivePower(powertype);
    }

    public static void HandleOffensivePowerDrop(Packet p)
    {
        int guid;
        guid = p.ReadInt();
        Config.PowerType powertype = (Config.PowerType)p.ReadInt();
       
        if(current.Guid == guid)
        {
            HUD hud = GameObject.Find("HUD").GetComponent<HUD>();
            hud.unBindOffensivePower();
        }

        GameObject go = ObjectMgr.Instance.Get(guid);
        go.GetComponent<BomberController>().Swing();
       
    }


    public static void HandleDespawn(Packet p)
    {
        if (current.Both)
            return;
        int guid;
        guid = p.ReadInt();
        GameMgr.Instance.Despawn(guid);
    }

    public static void HandlePlayAnnouncement(Packet p)
    {
        List<string> strs = new List<string>();
        int announce = p.ReadShort(), variant = p.ReadByte();
        string str = null;
        Debug.Log("read string");
        while ((str = p.ReadString()) != null)
            strs.Add(str);
        Announcer.Instance.PlayAnnounce((Announce)announce, variant, strs.ToArray());
    }

    public static void HandlePlayerSpeedUp(Packet p)
    {
        if (current.Both)
            return;
        int guid, speedMult;

        guid = p.ReadInt();
        speedMult = p.ReadInt();

        GameObject obj = ObjectMgr.Instance.Get(guid);
        obj.SendMessage("RecvIncSpeedMult");


    }

    public static void HandleUpdateScores(Packet p)
    {
        if (current.Both)
            return;
        int[] scores = new int[p.Size/4];
        for (int i = 0, len = scores.Length; i < len; i++)
            scores[i] = p.ReadInt();

        GameMgr.Instance.hud.setScores(scores);

    }


    public static void HandlePlayerDespawn(Packet p)
    {
        if (current.Both)
            return;
        int guid = p.ReadInt();
        GameMgr.Instance.Despawn(guid);
    }

    public static void HandleEndGame(Packet p)
    {
        if (current.Both)
            return;
        Config.GameMode gamemode = (Config.GameMode)p.ReadInt();
        GameMgr.Instance.EndGame(gamemode);
    }

    public static void HandleRespawn(Packet p)
    {
        int guid = p.ReadInt();
        Vector3 v = p.ReadVector3();


        GameObject player = null;
        if ((player = ObjectMgr.Instance.Get(guid)) != null)
            player.transform.position = v;

    }

}
