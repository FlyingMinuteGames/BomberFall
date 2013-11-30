﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
public class Client
{

    const int BUFFER_SIZE = 4096;

    TcpClient tcp_client;
    Thread tcp_thread;
    string address;
    int port;
    int sessionId = -1;
    bool m_isRunning = false;
    private bool m_isBoth = false;
    public bool Both
    {
        get { return m_isBoth; }
        set { m_isBoth = value; }
    }
    public int Session
    {
        get { return sessionId; }
        set { sessionId = value; }
    }
    OpcodeMgr m_opcodeMgr = new OpcodeMgr();
    public Client(string address, int port = Config.DEFAULT_PORT)
    {
        tcp_client = new TcpClient();
        this.address = address;
        this.port = port;
    }

    public void SetHandler(Dictionary<Opcode,OpcodeMgr._HandlePacket> handler)
    {
        m_opcodeMgr.SetHandler(handler);
    }


    public void Connect()
    {
        tcp_client.Connect(address, port);
        m_isRunning = true;

        tcp_thread = new Thread(new ParameterizedThreadStart(HandleClient));

        tcp_thread.Start(tcp_client);
    }

    public void SendPacket(Packet packet)
    {
        byte[] data = packet.ToByte();
        tcp_client.Client.Send(data);
    }

    private void HandleClient(object client)
    {
        TcpClient tcpClient = (TcpClient)client;

        NetworkStream clientStream = tcpClient.GetStream();
        Debug.Log("begin to read network stream");
        int size, opcode;
        byte[] buffer = new byte[4096];
        int bytesRead;
        Socket sock = tcpClient.Client;
        while (m_isRunning)
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
                Debug.Log("Recv packet size : " + size + ", opcode : " + (Opcode)opcode);
                if (size == 0)
                {
                    Packet p = new Packet(size, opcode, null);
                    HandlePacket(tcpClient, p);
                    continue;
                }
                if (size > BUFFER_SIZE)
                {

                    Debug.Log("unhandled packet, buffer execced ! " + size + " bytes");
                    int max = size / BUFFER_SIZE;
                    int last = size % BUFFER_SIZE;
                    for (int i = 0; i < max; i++)
                        bytesRead = sock.Receive(buffer, BUFFER_SIZE, SocketFlags.None); //clientStream.Read(buffer, 0, BUFFER_SIZE);
                    bytesRead = sock.Receive(buffer, last, SocketFlags.None); //clientStream.Read(buffer, 0, last);
                    continue;
                }
                
                bytesRead = sock.Receive(buffer, size, SocketFlags.None);
                if (bytesRead == size)
                {
                    Debug.Log("Recv packet real size : " + bytesRead + ", opcode : " + (Opcode)opcode);
                    Packet p = new Packet(size, opcode, buffer);
                    HandlePacket(tcpClient, p);
                }
            }
            catch
            {
                //a socket error has occured
                Debug.Log("socket error has occured !");
                break;
            }
            //Thread.Sleep(1000);

            if (bytesRead == 0)
            {
                Debug.Log("error socket close !");
                //the client has disconnected from the server
                break;
            }
        }
        Debug.Log("socket close !");
        tcpClient.Close();
    }

    private void HandlePacket(TcpClient client, Packet packet)
    {
        if (m_isBoth)
            return;
        Debug.Log("handle !");
        packet.Sender = client;
        Async.Instance.DelayedAction(() =>
        {
            m_opcodeMgr.HandlePacket(packet);
        });
    }

    public void LoadMap(byte[] buffer)
    {
        System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);

        GameMgr.Instance.maps = Maps.LoadMapsFromStream(stream);
    }

    public void Destroy()
    {
        Debug.Log("destroy client");
        m_isRunning = false;
        tcp_client.Close();
    }

    private GameObject player = null;
    private int guid = -1;
    public int Guid
    {
        get { return guid; }
        set{
            guid = value;
            player = ObjectMgr.Instance.get(guid);
            Debug.Log("player : " + (player == null)+ " guid : " + guid);
            player.GetComponent<BomberController>().m_IsPlayer = true;
        } 
    }
}
