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

    public WorldState State
    {
        get { return m_state; }
        
    }

    private GameObject m_MainCamera;
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
        baseRotation = m_MainCamera.transform.rotation;
	}
	
    public void StartServer()
    {
        s = new Server();
        ServerHandler.current = s;
        s.SetHandler(ServerHandler.handlers);
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
        StartCoroutine(ChangePhaseTimer());
        //ChangePhase();
    }

    public int Spawn(GOType type,Vector3 pos,int guid = -1)
    {
        GameObject go;
        switch (type)
        {
            case GOType.GO_PLAYER:
                go = player_pool.Pop(pos, Quaternion.identity);
                return  ObjectMgr.Instance.Register(go, type, guid);
            case GOType.GO_BOMB:
                go = bomb_pool.Pop(pos, Quaternion.identity);
                return ObjectMgr.Instance.Register(go, type, guid);
            case GOType.GO_PWRUP:
                go = pwr_up_pool.Pop(pos, Quaternion.identity);
                go.GetComponent<PowerUpGOScript>().Init();
                return ObjectMgr.Instance.Register(go, type, guid);

        }
        return -1;
    }

    public void Despawn(GOType type, int guid)
    {
        GameObject go = ObjectMgr.Instance.get(guid);
        if (go != null)
        {
            ObjectMgr.Instance.UnRegister(guid);
            Despawn(type, go);
        }

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
        }
    }

    public void StartClient(string address)
    {
       
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

    IEnumerator ChangePhaseTimer()
    {
        while (game_started)
        {
            yield return new WaitForSeconds(5);
            //ChangePhase();
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
}
