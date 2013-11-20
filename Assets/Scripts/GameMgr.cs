using UnityEngine;
using System.Collections;


public enum GOType
{
    GO_PLAYER,
    GO_BOMB,
    GO_BONUS
}
public enum GameMgrType
{
    CLIENT,
    SERVER
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
    public Client c = null;
    public Server s = null;
    public Maps maps;
    public bool game_started = false;
    private GameMgrType type;
    public GameMgrType Type
    {
        get { return type; }
    }
    void Start () {
        Application.runInBackground = true;
        s_instance = this;
        player_pool = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("Prefabs/Player_model"), 4);
        bomb_pool = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("Prefabs/Bomb"), 100);
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
                return  ObjectMgr.Instance.Register(go, type, guid);;
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
        c.SetHandler(ClientHandler.handlers);
        c.Connect();
        c.SendPacket(PacketBuilder.BuildConnectPacket(0, 0));
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
}
