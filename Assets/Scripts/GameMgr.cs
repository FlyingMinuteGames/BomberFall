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
    CLIENT,
    SERVER
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


public class GameMgr : MonoBehaviour {

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
    private static float const_gravity = -20.0f;
    private Vector3[] gravityStates;
    public MusicPlayer mp;

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

    void Start () {
        Application.runInBackground = true;
        s_instance = this;
        player_pool = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("Prefabs/Player_model"), 4);
        bomb_pool = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("Prefabs/Bomb"), 100);
        pwr_up_pool = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("Prefabs/PowerUp"), 100);
        hud = GameObject.Find("HUD").GetComponent<HUD>();
        m_MainCamera = GameObject.Find("MainCamera");
        mainMenu = GameObject.Find("OrthoCamera").GetComponent<MainMenuScript>();
        mp = GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>();

        baseRotation = m_MainCamera.transform.rotation;
        gravityStates = new Vector3[] { Vector3.up * const_gravity, Vector3.forward * const_gravity, Vector3.forward * -const_gravity, Vector3.right * const_gravity, Vector3.right * -const_gravity };
	   
    }
	
    public void StartServer()
    {
        s = new Server();
        ServerHandler.current = s;
        s.SetHandler(ServerHandler.handlers);
        type = GameMgrType.SERVER;
        s.OnClientConnected = (client) => { 
            //s.SendPacketTo(client,PacketBuilder.BuildMovePlayerPacket()=;
        };
        //StartClient("127.0.0.1");
    }

    public void StartGame()
    {
        //maps = Maps.LoadMapsFromFile("map1.map");
        hud.Init();
        game_started = true;
        s.SendPacketBroadCast(PacketBuilder.BuildStartGame());
        //StartCoroutine(ChangePhaseTimer());
        //ChangePhase();
        mp.PlayNextTrack();

    }

    public int Spawn(GOType type, Vector3 pos, int guid = -1, int extra = 0)
    {
        GameObject go;
        int _guid = -1;
        switch (type)
        {
            case GOType.GO_PLAYER:
                go = player_pool.Pop(pos, Quaternion.identity);
                _guid =  ObjectMgr.Instance.Register(go, type, guid);
                break;
            case GOType.GO_BOMB:
                go = bomb_pool.Pop(pos, Quaternion.identity);
                _guid = ObjectMgr.Instance.Register(go, type, guid);
                break;
            case GOType.GO_PWRUP:
                go = pwr_up_pool.Pop(pos, Quaternion.identity);
                PowerUpGOScript sc = go.GetComponent<PowerUpGOScript>();
                sc.type = (Config.PowerType)extra;
                sc.Init();
                _guid =  ObjectMgr.Instance.Register(go, type, guid,extra);
                break;
        }
        if(_guid > 0 && GameMgr.Instance.Type ==  GameMgrType.SERVER)
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
        if (GameMgr.Instance.Type == GameMgrType.SERVER)
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
        if (GameMgr.Instance.Type == GameMgrType.SERVER)
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

    public void StartClient(string address)
    {
        type = type != GameMgrType.SERVER ? GameMgrType.CLIENT : type;
        c = new Client(address);
        ClientHandler.current = c;
        if (s != null)
            c.Both = true;
        c.SetHandler(ClientHandler._handlers);
        c.Connect();
        c.SendPacket(PacketBuilder.BuildConnectPacket(c.Both ? 4 : 0, 0));

    }

    void OnDestroy()
    {
        Debug.Log("TEST");
        if (null != c)
            c.Destroy();
        if (null != s)
            s.Destroy();
    }

    void OnApplicationQuit()
    {
        Debug.Log("TEST");
        if (null != c)
            c.Destroy();
        if (null != s)
            s.Destroy();
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

    public void SpawnBomb(Vector3 pos)
    {
        Packet p = PacketBuilder.BuildSpawnBomb(pos);
        c.SendPacket(p);
    }

    public void UseOffensiveItem(int clientguid, Vector3 pos)
    {
        if (!hud.hasOffensivePower)
            return;
        //Packet p = PacketBuilder.BuildUseOffensiveItem(clientguid, pos);
        //c.SendPacket(p);
    }

    public void PowerUpPickUp(GameObject powerGo, int player_guid, APowerUp power)
    {
        Debug.Log("Power picked by player " + player_guid + " power id is " + powerGo.GetComponent<Guid>().GetGUID() + " power is " + power);
        power.OnPickUp(powerGo, player_guid);
    }

    IEnumerator ChangePhaseTimer()
    {
        while (game_started)
        {
            yield return new WaitForSeconds(5);
            ChangePhase();
        }
    }

    public void ChangePhase(WorldState state = WorldState.UNKNOWN)
    {
        Debug.Log("Change from "+m_state);
        if(state != WorldState.UNKNOWN)
            m_state = state;
        else m_state = m_state == WorldState.CENTER ? (WorldState)((int)(WorldState.CENTER)+Mathf.Ceil(Random.Range(1,4))) : WorldState.CENTER;
        Debug.Log("Change to " + m_state);
        
        IList<GameObject> l = ObjectMgr.Instance.Get(GOType.GO_PLAYER);
        foreach (var a in l)
            a.SendMessage("OnChangePhase", m_state);

        l = ObjectMgr.Instance.Get(GOType.GO_BOMB);
        Debug.Log("size : " + l.Count);
        foreach (var a in l)
            a.SendMessage("OnChangePhase", m_state);
        Debug.Log("change gravity from " + Physics.gravity + " to " + gravityStates[(int)m_state]);
        Physics.gravity = gravityStates[(int)m_state];
        
        TurnCamera((int)m_state);
        if (s != null)
            s.SendPacketBroadCast(PacketBuilder.BuildChangePhasePacket(m_state));   
    }

    public void TurnCamera(int index)
    {
        AnimationCurve x = new AnimationCurve();
        AnimationCurve y = new AnimationCurve();
        AnimationCurve z = new AnimationCurve();
        AnimationCurve w = new AnimationCurve();

        x.AddKey(0, m_MainCamera.transform.rotation.x);
        y.AddKey(0, m_MainCamera.transform.rotation.y);
        z.AddKey(0, m_MainCamera.transform.rotation.z);
        w.AddKey(0, m_MainCamera.transform.rotation.w);


        Quaternion final = baseRotation * s_CameraRotation[index];
        x.AddKey(1, final.x);
        y.AddKey(1, final.y);
        z.AddKey(1, final.z);
        w.AddKey(1, final.w);
        AnimationClip clip = new AnimationClip();
        clip.SetCurve("", typeof(Transform), "localRotation.x", x);
        clip.SetCurve("", typeof(Transform), "localRotation.y", y);
        clip.SetCurve("", typeof(Transform), "localRotation.z", z);
        clip.SetCurve("", typeof(Transform), "localRotation.w", w);
        m_MainCamera.GetComponent<Animation>().AddClip(clip, "1->"+index);
        m_MainCamera.GetComponent<Animation>().Play("1->" + index);
    }

    public void KillPlayer(Cross cross)
    {
        IList<GameObject> m_player = ObjectMgr.Instance.Get(GOType.GO_BOMB);

        for (int i = 0, len = m_player.Count; i < len; i++)
        {
            GameObject t = m_player[i];
            if (t == null)
                continue;
            IntVector2 tpos = maps.GetTilePosition(t.transform.position.x, t.transform.position.z);
            Debug.Log(tpos);
            if (cross.IsIn(tpos))
                Announcer.Instance.PlayAnnounce(Announce.ANNOUNCE_PLAYER_KILL, 0, "" + i);
            s.SendPacketBroadCast(PacketBuilder.BuildPlayAnnouncePacket(Announce.ANNOUNCE_PLAYER_KILL, 0, "" + i));

        }
    }
    public void QuitGame()
    {
        mp.PlayNextTrack();
        hud.Deactivate();
        if (type == GameMgrType.SERVER)
        {
            c.Destroy();
            s.Destroy();
            mainMenu.active = true;
        }
        else
        {
            c.Destroy();
            s.Destroy();
            mainMenu.active = true;
            
        }
    }
}
