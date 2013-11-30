using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OpcodeMgr
{
    public delegate void _HandlePacket(Packet p);
    public struct HandlePacketStruct
    {
        public HandlePacketStruct(Opcode _o, _HandlePacket h)
        {
            o = _o;
            handler = h;
        }
        public Opcode o;
        public _HandlePacket handler;
    }
    private Dictionary<Opcode, _HandlePacket> m_handler = new Dictionary<Opcode, _HandlePacket>();

    public void SetHandler(HandlePacketStruct[] handlers)
    {
        foreach (HandlePacketStruct handler in handlers)
            m_handler[handler.o] = handler.handler;
    }
    public void SetHandler(Dictionary<Opcode, _HandlePacket> handlers)
    {
        m_handler = handlers;
    }
    public void HandlePacket(Packet p)
    {
        if (!m_handler.ContainsKey(p.GetOpcode()))
            return;

        m_handler[p.GetOpcode()](p);
    }
    public OpcodeMgr()
    {
        //SetHandler(test);
    }
}
