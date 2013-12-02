using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GOType
{
    GO_PLAYER,
    GO_BOMB,
    GO_PWRUP
}
public enum GameMgrType
{
    CLIENT = 1,
    SERVER = 2
}


public enum WorldState
{
    CENTER,
    LATERAL_X,
    LATERAL_X2,
    LATERAL_Z,
    LATERAL_Z2,
    UNKNOWN
}

public enum WorldStateExtra
{
    WORLDSTATE_1,
    WORLDSTATE_2,
    WORLDSTATE_3,
    WORLDSTATE_4,
    NONE
}


public class GameMgr : MonoBehaviour
{

    // Use this for initialization

    private static Quaternion[] s_CameraRotation = new Quaternion[] { Quaternion.identity, Quaternion.identity, Quaternion.AngleAxis(180, Vector3.forward), Quaternion.AngleAxis(-90, Vector3.forward), Quaternion.AngleAxis(90, Vector3.forward) };
    private Quaternion baseRotation;
    private static GameMgr s_instance = null;
    public static GameMgr Instance
    {
        get { return s_instance; }
    }
    public PoolSystem<GameObject> player_pool;
    public PoolSystem<GameObject> bomb_pool;
    public PoolSystem<GameObject> pwr_up_pool;

    public Client c = null;
    public Server s = null;
    public Maps maps;
    public GameIntel gameIntel;
    public HUD hud;
    public bool game_started = false;
    private GameMgrType type;
    private WorldState m_state = WorldState.CENTER;
    private WorldStateExtra m_state_extra = WorldStateExtra.WORLDSTATE_1;
    private static float const_gravity = -20.0f;
    private Vector3[] gravityStates;
    public MusicPlayer mp;
    private EndMenu endmenu;
    private int nbDeads = 0;

    public WorldState State
    {
        get { return m_state; }

    }

    private GameObject m_MainCamera;
    private MainMenuScript mainMenu;
    public GameMgrType Type
    {
        get { return type; }
    }

    void Start()
    {
        Application.runInBackground = true;
        s_instance = this;
        player_pool = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("Prefabs/Player_model"), 4);
        bomb_pool = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("Prefabs/Bomb"), 100);
        pwr_up_pool = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("Prefabs/PowerUp"), 100);
        hud = GameObject.Find("HUD").GetComponent<HUD>();
        m_MainCamera = GameObject.Find("MainCamera");
        mainMenu = GameObject.Find("OrthoCamera").GetComponent<MainMenuScript>();
        mp = GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>();

        endmenu = GameObject.Find("EndMenu").GetComponent<EndMenu>();


        baseRotation = m_MainCamera.transform.rotation;
        gravityStates = new Vector3[] { Vector3.up * const_gravity, Vector3.forward * const_gravity, Vector3.forward * -const_gravity, Vector3.right * const_gravity, Vector3.right * -const_gravity };

    }

    
    public void Reset()
    {

        if (null != c)
            c.Destroy();
        c = null;
        if (null != s)
            s.Destroy();
        s = null;
        type = (GameMgrType)0;
        ObjectMgr.Instance.Clear();
        player_pool.ClearAndRealloc();
        bomb_pool.ClearAndRealloc();
        pwr_up_pool.ClearAndRealloc();
        if (maps != null)
            maps.Clear();
        game_started = false;
        StopAllCoroutines();
        Async.Instance.Restart();
        
        
    }

    public void StartServer()
    {
        s = new Server(gameIntel.nb_players);
        ServerHandler.current = s;
        s.SetHandler(ServerHandler.handlers);
        type |= GameMgrType.SERVER;
    }

    public void StartGame()
    {
        hud.Init();
        game_started = true;
        s.SendPacketBroadCast(PacketBuilder.BuildStartGame());
        StartCoroutine(ChangePhaseTimer());
        //ChangePhase();
        mp.PlayNextTrack();

    }

    /*private int xtra = 0;
    public void _switch()
    {
        xtra++;
        if (xtra > 3)
            xtra = 0;
        ChangePhase(WorldState.CENTER, (WorldStateExtra)xtra);
    }*/

    public int Spawn(GOType type, Vector3 pos, int guid = -1, int extra = 0)
    {
        GameObject go;
        int _guid = -1;
        switch (type)
        {
            case GOType.GO_PLAYER:
                go = player_pool.Pop(pos, Quaternion.identity);
                _guid = ObjectMgr.Instance.Register(go, type, guid, extra);
                BomberController controller = go.GetComponent<BomberController>();
                controller.ColorIndex = extra;
                break;
            case GOType.GO_BOMB:
                go = bomb_pool.Pop(pos, Quaternion.identity);
                _guid = ObjectMgr.Instance.Register(go, type, guid, extra);
                break;
            case GOType.GO_PWRUP:
                go = pwr_up_pool.Pop(pos, Quaternion.identity);
                PowerUpGOScript sc = go.GetComponent<PowerUpGOScript>();
                sc.type = (Config.PowerType)extra;
                sc.Init();
                _guid = ObjectMgr.Instance.Register(go, type, guid, extra);
                break;
        }
        if (_guid > 0 && ((GameMgr.Instance.Type & GameMgrType.SERVER) != 0))
            GameMgr.Instance.s.SendPacketBroadCast(PacketBuilder.BuildInstantiateObjPacket(ObjectMgr.Instance.DumpData(_guid)));
        return _guid;
    }

    public void Despawn(int guid)
    {

        ObjectMgr.GOWrapper go = ObjectMgr.Instance.GetWrapper(guid);
        if (go.go != null)
        {
            ObjectMgr.Instance.UnRegister(guid);
            Despawn(go.type, go.go);
        }
        if (((GameMgr.Instance.Type & GameMgrType.SERVER) != 0))
            GameMgr.Instance.s.SendPacketBroadCast(PacketBuilder.BuildDespawn(guid));

    }

    public void Despawn(GOType type, int guid)
    {
        GameObject go = ObjectMgr.Instance.Get(guid);

        if (go != null)
        {
            ObjectMgr.Instance.UnRegister(guid);
            Despawn(type, go);
        }
        if (((GameMgr.Instance.Type & GameMgrType.SERVER) != 0))
            GameMgr.Instance.s.SendPacketBroadCast(PacketBuilder.BuildDespawn(guid));
    }
    private void Despawn(GOType type, GameObject go)
    {
        switch (type)
        {
            case GOType.GO_PLAYER:
                player_pool.Free(go);
                break;
            case GOType.GO_BOMB:
                bomb_pool.Free(go);
                break;
            case GOType.GO_PWRUP:
                pwr_up_pool.Free(go);
                break;
        }
    }

    public bool StartClient(string address)
    {
        type |= GameMgrType.CLIENT;
        c = new Client(address);
        ClientHandler.current = c;
        if ((type & GameMgrType.SERVER) != 0)
            c.Both = true;
        c.SetHandler(ClientHandler._handlers);
        if (c.Connect())
        {
            c.SendPacket(PacketBuilder.BuildConnectPacket(c.Both ? 4 : 0, 0));
            return true;
        }
        else return false;

    }

    void OnDestroy()
    {
       
        Reset();
    }

    void OnApplicationQuit()
    {
        Reset();
    }

    public void PlayerMove(int flag, Vector3 pos)
    {
        Packet p = PacketBuilder.BuildMovePlayerPacket(c.Guid, flag, pos);
        c.SendPacket(p);
    }
    public void PlayerJump(Vector3 pos)
    {
        Packet p = PacketBuilder.BuildJumpPacket(c.Guid, pos);
        c.SendPacket(p);
    }

    public void SpawnBomb(int guid, Vector3 pos)
    {
        Packet p = PacketBuilder.BuildSpawnBomb(guid, pos);
        c.SendPacket(p);
    }

    public void UseOffensiveItem(int clientguid)
    {
        Debug.Log("Trying to use offensive item");
        if (!hud.hasOffensivePower)
            return;
        Packet p = PacketBuilder.BuildUseOffensiveItem(clientguid);
        c.SendPacket(p);
    }

    public void PowerUpPickUp(GameObject powerGo, int player_guid, APowerUp power)
    {
        Debug.Log("Power picked by player " + player_guid + " power id is " + powerGo.GetComponent<Guid>().GetGUID() + " power is " + power);
        power.OnPickUp(powerGo, player_guid);
    }

    public void PlayAnnounce(Announce annnouce, byte variant, params string[] var)
    {
        if ((type & GameMgrType.SERVER) == 0)
            return;
        Announcer.Instance.PlayAnnounce(annnouce, variant, var);
        s.SendPacketBroadCast(PacketBuilder.BuildPlayAnnouncePacket(annnouce, variant, var));
    }

    IEnumerator ChangePhaseTimer()
    {
        while (game_started)
        {
            PlayAnnounce(Announce.ANNOUNCE_CHANGE_PHASE, 0, "30");
            yield return new WaitForSeconds(30);
            PlayAnnounce(Announce.ANNOUNCE_CHANGE_PHASE, 0, "5");
            yield return new WaitForSeconds(5);
            PlayAnnounce(Announce.ANNOUNCE_CHANGE_NOW, 0);
            ChangePhase();
        }
    }

    public void ChangePhase(WorldState state = WorldState.UNKNOWN, WorldStateExtra extra = WorldStateExtra.NONE)
    {
        Debug.Log("Change from " + m_state);
        if (state != WorldState.UNKNOWN)
        {
            m_state = state;
            m_state_extra = extra;
        }
        else
        {
            m_state = m_state == WorldState.CENTER ? (WorldState)((int)(WorldState.CENTER) + Random.Range(1, 3)) : WorldState.CENTER;
            if (m_state == WorldState.CENTER)
            {
                int rand = Random.Range(0, 3);
                extra = (WorldStateExtra)rand;
                if (extra == m_state_extra)
                    extra++;
                m_state_extra = extra;
            }
            else
            {
                extra = (WorldStateExtra)((int)m_state - 1);
                if (extra == m_state_extra)
                {
                    extra++;
                    m_state = (WorldState)((int)extra + 1);
                }
                m_state_extra = extra;
            }
        }
        Debug.Log("Change to " + m_state);

        IList<GameObject> l = ObjectMgr.Instance.Get(GOType.GO_PLAYER);
        foreach (var a in l)
            a.SendMessage("OnChangePhase", new short[] { (short)m_state, (short)m_state_extra });

        l = ObjectMgr.Instance.Get(GOType.GO_BOMB);
        Debug.Log("size : " + l.Count);
        foreach (var a in l)
            a.SendMessage("OnChangePhase", m_state);
        Debug.Log("change gravity from " + Physics.gravity + " to " + gravityStates[(int)m_state]);
        Physics.gravity = gravityStates[(int)m_state];

        TurnCamera();
        if (s != null)
            s.SendPacketBroadCast(PacketBuilder.BuildChangePhasePacket(m_state, m_state_extra));
    }

    public void TurnCamera()
    {

        AnimationCurve x = new AnimationCurve();
        AnimationCurve y = new AnimationCurve();
        AnimationCurve z = new AnimationCurve();
        AnimationCurve w = new AnimationCurve();

        x.AddKey(0, m_MainCamera.transform.rotation.x);
        y.AddKey(0, m_MainCamera.transform.rotation.y);
        z.AddKey(0, m_MainCamera.transform.rotation.z);
        w.AddKey(0, m_MainCamera.transform.rotation.w);
        int index = (int)m_state;

        Quaternion final = m_state == WorldState.CENTER ? baseRotation * s_CameraRotation[(int)m_state_extra + 1] : baseRotation * s_CameraRotation[index];
        x.AddKey(1, final.x);
        y.AddKey(1, final.y);
        z.AddKey(1, final.z);
        w.AddKey(1, final.w);
        AnimationClip clip = new AnimationClip();
        clip.SetCurve("", typeof(Transform), "localRotation.x", x);
        clip.SetCurve("", typeof(Transform), "localRotation.y", y);
        clip.SetCurve("", typeof(Transform), "localRotation.z", z);
        clip.SetCurve("", typeof(Transform), "localRotation.w", w);
        m_MainCamera.GetComponent<Animation>().AddClip(clip, "1->" + index);
        m_MainCamera.GetComponent<Animation>().Play("1->" + index);
    }

    public void KillPlayer(Cross cross, int bombGUID)
    {
        Debug.Log("In kill player by bomb");
        IList<GameObject> m_player = ObjectMgr.Instance.Get(GOType.GO_PLAYER);
        BombScript bomb = ObjectMgr.Instance.Get(bombGUID).GetComponent<BombScript>();
        int killerGUID = bomb.OwnerGuid;
        int[] scores = hud.getScores();

        bool suicide = false, hasvictim = false; ;
        for (int i = 0, len = m_player.Count; i < len; i++)
        {
            GameObject t = m_player[i];
            int curID = t.GetComponent<Guid>().GetGUID();

            if (t == null)
                continue;
            IntVector2 tpos = maps.GetTilePosition(t.transform.position.x, t.transform.position.z);
            Debug.Log(tpos);
            if (tpos == null)
                continue;
            if (cross.IsIn(tpos))
            {
                hasvictim = true;
                PlayAnnounce(Announce.ANNOUNCE_PLAYER_KILL, 0, "" + (i + 1));
                if (this.gameIntel.game_mode == Config.GameMode.SURVIVAL)
                {
                    scores = hud.getScores();
                    scores[i] = -1;
                    hud.setScores(scores);
                    s.SendPacketBroadCast(PacketBuilder.BuildPlayerDespawn(curID));
                    this.Despawn(curID);
                    nbDeads++;

                    if (nbDeads == this.s.client_count - 1)
                    {
                        if ((this.type & GameMgrType.SERVER) != 0)
                        {
                           this.s.SendPacketBroadCast(PacketBuilder.BuildSendEndOfGame((int)GameMgr.Instance.gameIntel.game_mode));
                           EndGame(GameMgr.Instance.gameIntel.game_mode);
                        }
                    }
                }
                else
                {
                    scores = hud.getScores();
                    if (curID == killerGUID)
                    {
                        scores[i]--;
                        suicide = true;
                    }
                    hud.setScores(scores);


                    RespawnPlayer(curID);
                }
            }
            if (this.gameIntel.game_mode == Config.GameMode.SURVIVAL && curID == killerGUID && hasvictim && !suicide)
                scores[i]++;
        }

        if (hasvictim)
        {
            s.SendPacketBroadCast(PacketBuilder.BuildUpdateScoresPacket(scores));
        }
    }

    public void KillPlayer(int victim, int killer, Config.PowerType powertype)
    {
        Debug.Log("In kill player by sword");

        IList<GameObject> m_player = ObjectMgr.Instance.Get(GOType.GO_PLAYER);

        int[] scores = hud.getScores();

        for (int i = 0, len = m_player.Count; i < len; i++)
        {
            int curId = m_player[i].GetComponent<Guid>().GetGUID();
            if (curId == victim)
            {
                PlayAnnounce(Announce.ANNOUNCE_KILL_BY_SW, 0, "" + (i + 1));
                if (this.gameIntel.game_mode == Config.GameMode.SURVIVAL)
                {
                    scores = hud.getScores();
                    scores[i] = -1;
                    hud.setScores(scores);
                    s.SendPacketBroadCast(PacketBuilder.BuildPlayerDespawn(victim));
                    this.Despawn(victim);
                    nbDeads++;

                    if (nbDeads == this.s.client_count - 1)
                    {
                        if ((this.type & GameMgrType.SERVER) != 0)
                        {
                            this.s.SendPacketBroadCast(PacketBuilder.BuildSendEndOfGame((int)GameMgr.Instance.gameIntel.game_mode));
                            EndGame(GameMgr.Instance.gameIntel.game_mode);
                        }
                    }
                }
                else
                {
                    scores = hud.getScores();
                    if (victim == killer)
                    {
                        scores[i]--;
                    }
                    hud.setScores(scores);

                    RespawnPlayer(victim);
                }
            
            
            }
            else if (curId == killer)
            {

                s.SendPacketTo(GameMgr.Instance.s.GetTcpClient(killer), PacketBuilder.BuildPlayAnnouncePacket(Announce.ANNOUNCE_KILL_BY_SW, 0, "" + (i + 1)));

            }
            s.SendPacketBroadCast(PacketBuilder.BuildUpdateScoresPacket(scores));

        }
    }

    public void QuitGame()
    {
        mp.PlayNextTrack();
        hud.Deactivate();
        Reset();
        mainMenu.Reset();
        mainMenu.active = true;
    }

    public void RespawnPlayer(int guid)
    {
        List<IntVector2> respawn_position = new List<IntVector2>() { new IntVector2(0, 0), new IntVector2(maps.Size.x - 1, maps.Size.y - 1), new IntVector2(maps.Size.x - 1, 0), new IntVector2(0, maps.Size.y - 1) };
        Vector3 respawn_pos = Vector3.zero;
        Debug.Log("Respawn Player !!!!!!!!!!");
        for (int i = 0; i < 4; i++)
        {
            int index = Random.Range(0, 4 - i);
            IntVector2 pos = respawn_position[index];
            respawn_position.RemoveAt(index);
            Vector3 world_pos = maps.TilePosToWorldPos(pos);
            world_pos.y = 0.5f;
            var hit = Physics.OverlapSphere(world_pos, 0.25f);
            bool isOk = true;
            foreach( var o in hit)
            {
                if (o.gameObject.tag == "Player" || o.gameObject.tag == "Bomb")
                {
                    Debug.Log(o.gameObject.tag + " found at " + pos);
                    isOk = false;
                    break;
                }
            }
            if(isOk)
            {
                respawn_pos = world_pos;
                break;
            }
        }


        GameObject player = ObjectMgr.Instance.Get(guid);
        respawn_pos.y =  player.transform.position.y;
        player.transform.position = respawn_pos;
        GameMgr.Instance.s.SendPacketBroadCast(PacketBuilder.BuildRespawnPacket(guid, respawn_pos));
    }

    public void EndGame(Config.GameMode gamemode)
    {
        hud.Deactivate();

        endmenu.setMode(gamemode, hud.getScores());
        endmenu.m_active = true;
    }
}
