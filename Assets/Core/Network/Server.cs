using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Net;
public class Server //: INetwork 
{
    public struct ClientSession
    {
        public ClientSession(int sessionId, int guid, TcpClient client)
        {
            this.sessionId = sessionId;
            playerGUID = guid;
            tcpClient = client;
        }
        public int sessionId, playerGUID;
        public TcpClient tcpClient;
    }
    const int BUFFER_SIZE = 4096;
    TcpListener tcp_server;
    List<TcpClient> m_clients;
    Dictionary<int, ClientSession> m_sessions;
    public Dictionary<int, ClientSession> Session
    {
        get { return m_sessions; }
    }
    Thread listener_thread;
    bool m_isRunning = false;
    OpcodeMgr m_opcodeMgr = new OpcodeMgr();
    public  delegate void _OnClientConnected(TcpClient cl);
    public _OnClientConnected OnClientConnected;
    private int session = 0;

    public int client_count
    {
        get { return m_clients.Count; }
    }
    private int max_client;
    public Server(int max = 4)
    {
        max_client = max;
        m_sessions = new Dictionary<int, ClientSession>();
        tcp_server = new TcpListener(new IPEndPoint(IPAddress.Any, Config.DEFAULT_PORT));
        listener_thread = new Thread(new ThreadStart(ListenForClients));
        listener_thread.Start();
    }


    public void SetHandler(OpcodeMgr.HandlePacketStruct[] handler)
    {
        m_opcodeMgr.SetHandler(handler);
    }
    public void SetHandler(Dictionary<Opcode,OpcodeMgr._HandlePacket> handler)
    {
        m_opcodeMgr.SetHandler(handler);
    }

    private void ListenForClients()
    {
        m_isRunning = true;
        tcp_server.Start();
        m_clients = new List<TcpClient>();
        while (m_isRunning)
        {
            //Debug.Log("(SERVER) Listen...");
            if (!tcp_server.Pending())
            {
                Thread.Sleep(100);
            }
            else
            {

                TcpClient client = tcp_server.AcceptTcpClient();
                if (GameMgr.Instance.game_started || m_clients.Count >= max_client)
                {
                    client.Close();
                    break;
                }
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
                m_clients.Add(client);
                if (null != OnClientConnected)
                    OnClientConnected(client);
            }
        
        }
        tcp_server.Stop();
        tcp_server = null;
    }

    public int GetSessionId(int playerGuid)
    {
        foreach(var v in m_sessions)
        {
            if (v.Value.playerGUID == playerGuid)
                return v.Key;
        }
        return -1;
    }

    public TcpClient GetTcpClient(int playerGuid)
    {
        foreach (var v in m_sessions)
        {
            if(v.Value.playerGUID == playerGuid)
                return v.Value.tcpClient;
        }
        return null;
    }

    private void HandleClient(object client)
    {
        TcpClient tcpClient = (TcpClient)client;
        
        NetworkStream clientStream = tcpClient.GetStream();

        int size, opcode;
        byte[] buffer =  new byte[4096];
        int bytesRead;
        Socket sock = tcpClient.Client;

        while (true)
        {
            bytesRead = 0;
            try
            {
                //read packet header
                bytesRead = sock.Receive(buffer, 8, SocketFlags.None);//clientStream.Read(buffer, 0, 8);
                if (bytesRead != 8)
                    break;
                size = Packet.ToInt(buffer, 0);
                opcode = Packet.ToInt(buffer, 4);
                if (size == 0)
                {
                    Packet p = new Packet(size, opcode, null);
                    HandlePacket(tcpClient, p);
                    continue;
                }
                if(size > BUFFER_SIZE)
                {

                    Debug.Log("(SERVER) unhandled packet, buffer execced ! " + size + " bytes");
                    int max = size/BUFFER_SIZE;
                    int last = size%BUFFER_SIZE;
                    for(int i = 0; i < max; i++)
                        bytesRead = sock.Receive(buffer, BUFFER_SIZE, SocketFlags.None); //clientStream.Read(buffer, 0, BUFFER_SIZE);
                    bytesRead = sock.Receive(buffer, last, SocketFlags.None); //clientStream.Read(buffer, 0, last);
                    continue;
                }
                bytesRead = sock.Receive(buffer, size, SocketFlags.None);
                if (bytesRead == size)
                {
                    Packet p = new Packet(size, opcode, buffer);
                    HandlePacket(tcpClient, p);
                }
            }
            catch(Exception e)
            {
                //a socket error has occured
                Debug.Log("(SERVER) catch an error"+e.Message);
                break;
            }

            if (bytesRead == 0)
            {
                //the client has disconnected from the server
                break;
            }
        }
        Debug.Log("(SERVER) socket close !");
        tcpClient.Close();
        //remove

        lock (m_sessions)
        {
            foreach(var k in m_sessions)
            {
                if (k.Value.tcpClient == tcpClient)
                {
                    m_sessions.Remove(k.Key);
                    break;
                }
            }
        }
        lock (m_clients)
        {
            m_clients.Remove(tcpClient);
        }

    }
    private void HandlePacket(TcpClient client, Packet packet)
    {
        packet.Sender = client;
        Async.Instance.DelayedAction(() =>
        {
            m_opcodeMgr.HandlePacket(packet);
        }); 
    }

    public void SendPacketTo(TcpClient client, Packet packet)
    {
        byte[] data = packet.ToByte();
        if (!client.Connected)
            return;
        //Debug.Log("send packet to client, opcode : " + packet.GetOpcode() + " ,size : " + packet.Size + " bytes");
        client.Client.Send(data);//client.GetStream().Write(data,0,data.Length);
    }

    public void SendPacketBroadCast(Packet packet, TcpClient except = null)
    {
        byte[] data = packet.ToByte();
        //Debug.Log("send packet to all client, opcode : " + packet.GetOpcode() + " ,size : " + packet.Size + " bytes");
        foreach(TcpClient client in m_clients)
        {
            if (!client.Connected)
                return;
            if(client != except)
                client.Client.Send(data);//client.Send(data,0,data.Length);
        }
    }

    public void RegisterPlayer(TcpClient cl,int flags)
    {

        
        int _session = ++session;
       
        Maps maps = GameMgr.Instance.maps;
        SendPacketTo(cl, PacketBuilder.BuildSendMapPacket(maps));
        byte[] data = ObjectMgr.Instance.DumpData();
        if(data.Length > 0)
            SendPacketTo(cl,PacketBuilder.BuildInstantiateObjPacket(data));

        int guid = GameMgr.Instance.Spawn(GOType.GO_PLAYER, GetInitPos(session - 1), -1, _session-1);
        m_sessions[_session] = new ClientSession(_session,guid,cl);

        if ((flags & 4) != 0)
        {// hack lol
            ObjectMgr.Instance.Get(guid).GetComponent<BomberController>().m_IsPlayer = true;
            GameMgr.Instance.c.Guid = guid;
        }
        SendPacketTo(cl,PacketBuilder.BuildPlayerConnectPacket(_session,guid, 0,GameMgr.Instance.gameIntel));
    }
    public void Destroy()
    {
        Debug.Log("abord thread");
        m_isRunning = false;
        m_clients.ForEach((TcpClient a) => { a.Close(); });
    }
    Vector3 GetInitPos(int index)
    {
        if (index < 0 || index > 3)
            return Vector3.zero;
        Maps maps = GameMgr.Instance.maps;
        Vector3 pos = maps.TilePosToWorldPos(new IntVector2(index % 2 != 0 ? maps.Size.x - 1 : 0, index > 0 && index < 3 ? maps.Size.y - 1 : 0));
        pos.y = 0.5150594f;
        return pos;
    }

    public void SpawnBomb(BomberController controller, Vector3 pos)
    {
        if (!controller.IsOnGround || controller.BombSpawn >= controller.BombCount)
            return;
        controller.BombSpawn++;
        int owner = controller.GetComponent<Guid>().GetGUID();
        Maps maps = GameMgr.Instance.maps;
        IntVector2 tpos = maps.GetTilePosition(pos.x,pos.z);
        pos =   maps.TilePosToWorldPos(tpos);
        pos.y = 0.5f;
        var hit = Physics.OverlapSphere(pos, 0.25f);
        foreach (var c in hit)
        {
            if (c.gameObject.tag == "Bomb")
                return;
        }
        int guid = GameMgr.Instance.Spawn(GOType.GO_BOMB, pos);
        int radius = controller.BombRadius;
        GameObject go =  ObjectMgr.Instance.Get(guid);
        go.GetComponent<BombScript>().StartScript(owner, () =>
        {
            IntVector2 new_tpos = maps.GetTilePosition(go.transform.position.x, go.transform.position.z);
            maps.ExplodeAt(guid,new_tpos, radius);
            controller.BombSpawn--;
            if (new_tpos == null)
                return;
            SendPacketBroadCast(PacketBuilder.BuildBombExplode(guid,new_tpos, radius));
        },
        () => {
            GameMgr.Instance.Despawn(GOType.GO_BOMB, guid);
        });
    }
}
