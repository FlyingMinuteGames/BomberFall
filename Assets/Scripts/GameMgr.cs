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
    public bool game_started = false;
    private GameMgrType type;
    private WorldState m_state = WorldState.CENTER;
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
        game_started = true;
        s.SendPacketBroadCast(PacketBuilder.BuildStartGame());
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
        }
        return -1;
    }

    public void Despawn(GOType type, int guid)
    {
        GameObject go = ObjectMgr.Instance.get(guid);
        if (go != null)
            Despawn(type, go);

    }
    public void Despawn(GOType type, GameObject go)
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
        c.SetHandler(ClientHandler.handlers);
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

    public void SpawnBomb(Vector3 pos)
    {
        Packet p = PacketBuilder.BuildSpawnBomb(pos);
        c.SendPacket(p);
    }

    IEnumerator ChangePhaseTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(30);
            ChangePhase();
        }
    }

    public void ChangePhase(WorldState state = WorldState.UNKNOWN)
    {
        if(state != WorldState.UNKNOWN)
            m_state = state;
        else m_state = m_state == WorldState.CENTER ? (WorldState)((int)(WorldState.CENTER)+Mathf.Ceil(Random.Range(1,4))) : WorldState.CENTER;

        IList<GameObject> l = ObjectMgr.Instance.Get(GOType.GO_PLAYER);

    }
}
