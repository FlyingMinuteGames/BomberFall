﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpcodeMgr  {
    public delegate void _HandlePacket(Packet p);
    public struct HandlePacketStruct
    {
        public HandlePacketStruct(Opcode _o,_HandlePacket h)
        {
            o = _o;
            handler = h;
        }
        public Opcode o;
        public _HandlePacket handler;
    }
    private Dictionary<Opcode,_HandlePacket> m_handler = new Dictionary<Opcode,_HandlePacket>();
    private HandlePacketStruct[] test = new HandlePacketStruct[]{ new HandlePacketStruct(Opcode.MSG_PLAYER_MOVE,(p)=>{Debug.Log("recv packet MSG_PLAYER_MOVE");})};

    public void SetHandler(HandlePacketStruct[] handlers)
    {
        foreach(HandlePacketStruct handler in handlers)
            m_handler[handler.o] = handler.handler;
    }
    public void HandlePacket(Packet p)
    {
        if(!m_handler.ContainsKey(p.GetOpcode()))
            return;

        m_handler[p.GetOpcode()](p);
    }
    public OpcodeMgr()
    {
        SetHandler(test);
    }
}
