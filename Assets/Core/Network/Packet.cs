using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System;
public class Packet  {


    /* byte utils*/
    public static void ToByte(byte[] data,int value,int offset = 0)
    {
        data[0 + offset] = (byte)(value >> 24);
        data[1 + offset] = (byte)(value >> 16);
        data[2 + offset] = (byte)(value >> 8);
        data[3 + offset] = (byte)value;
    }

    public static int ToInt(byte[] data, int offset = 0)
    {
        return (((int)data[0 + offset]) << 24) + (((int)data[1 + offset]) << 16) + (((int)data[2 + offset]) << 8) + (((int)data[3 + offset]));
    }

    private int m_size;
    private byte[] m_data;
    private int m_cursor = 0;
    private int m_opcode;
    private TcpClient m_sender;
    private byte[] m_buffer = new byte[8];
    
    public int Cursor
    {
        get { return m_cursor; }
        set { m_cursor = m_cursor > m_size ? m_size : (m_cursor < 0 ? 0 : value); }
    }

    public int Size
    {
        get { return m_size; }
    }
    
    public TcpClient Sender
    {
        get { return m_sender; }
        set { m_sender = value; }
    }

    public Opcode GetOpcode()
    {
        return (Opcode)m_opcode;
    }

    public Packet(int size, int opcode)
    {
        m_size = size;
        m_opcode = opcode;
        if (size == 0)
            return;
        m_data = new byte[size];
    }

    public Packet(int size, int opcode,byte[] data)
    {

        m_size = size;
        m_opcode = opcode;
        if (size == 0 || data == null)
            return;
        m_data = new byte[size];
        Array.Copy(data, m_data, m_size);
    }

    public Packet(int size, Opcode opcode)
    {
        
        m_size = size;
        m_opcode = (int)opcode;
        if (size == 0)
            return;
        m_data = new byte[size];
    }

    public Packet(int size, Opcode opcode, byte[] data)
    {

        m_size = size;
        m_opcode = (int)opcode;
        if (size == 0 || data == null)
            return;
        m_data = new byte[size];
        Array.Copy(data, m_data, m_size);
    }

    public Packet(byte[] data)
    {

        m_size = ToInt(data);
        m_opcode = ToInt(data,4);
        if (m_size <= 8)
            return;
        m_data = new byte[data.Length-8];
        Array.Copy(data, 8, m_data, 0, m_size);
    }

    public byte[] ToByte()
    {
        byte[] data = new byte[4 + 4 + m_size];
        Packet.ToByte(data,m_size,0);
        Packet.ToByte(data,m_opcode,4);
        if(m_data != null)
            System.Array.Copy(m_data, 0, data, 8, m_size);
        return data;
    }
    public void Write (byte value)
    {
        if (m_cursor >= m_size)
            return;
        m_data[m_cursor++] = value;
    }
    public void Write (short value)
    {
        if (m_cursor+2 > m_size)
            return;
        Array.Copy(BitConverter.GetBytes(value),0,m_data,m_cursor,2);
        m_cursor += 2;
    }
    public void Write(char value)
    {
        if (m_cursor+2 > m_size)
            return;
        Array.Copy(BitConverter.GetBytes(value), 0, m_data, m_cursor, 2);
        m_cursor += 2;
    }
    public void Write (int value)
    {
        if (m_cursor + 4 > m_size)
            return;
        Array.Copy(BitConverter.GetBytes(value), 0, m_data, m_cursor, 4);
        m_cursor += 4;
    }
    public void Write (float value)
    {
        if (m_cursor + 4 > m_size)
            return;
        Array.Copy(BitConverter.GetBytes(value), 0, m_data, m_cursor, 4);
        m_cursor += 4;
    }

    public void Write(string value)
    {
        if (m_cursor + (value.Length+1)*2 > m_size)
            return;
        foreach (char c in value)
        {
            Write(c);
        }
        Write((char)0);
    }

    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    public void Write(byte[] _buffer)
    {
        if (m_cursor + _buffer.Length > m_size)
            return;
        Array.Copy(_buffer, 0, m_data, m_cursor, _buffer.Length);
        m_cursor += _buffer.Length;
    }


    public int ReadInt()
    {
        if (m_cursor + 4 > m_size)
            return 0;
        var value = BitConverter.ToInt32(m_data, m_cursor);
        m_cursor += 4;
        return value;
    }

    public float ReadFloat()
    {
        if (m_cursor + 4 > m_size)
            return 0;
        var value = BitConverter.ToSingle(m_data, m_cursor);
        m_cursor += 4;
        return value;
    }

    public short ReadShort()
    {
        if (m_cursor + 2 > m_size)
            return 0;
        var value = BitConverter.ToInt16(m_data, m_cursor);
        m_cursor += 2;
        return value;
    }

    public char ReadChar()
    {
        if (m_cursor + 2 > m_size)
            return (char)0;
        var value = BitConverter.ToChar(m_data, m_cursor);
        m_cursor += 2;
        return value;
    }

    public string ReadString()
    {
        IList<char> _string = new List<char>();

        char tmp;
        int c = 0;
        while ((tmp = ReadChar()) != 0)
            _string.Add(tmp);
        if (_string.Count == 0)
            return null;
        char[] _str = new char[_string.Count];
        for (int i = 0, len = _string.Count; i < len; i++)
            _str[i] = _string[i];
        return new string(_str);
    }

    public byte ReadByte()
    {
        if (m_cursor >= m_size)
            return 0;
        return m_data[m_cursor++];
    }

    public Vector3 ReadVector3()
    {
        float x, y, z;
        x = ReadFloat();
        y = ReadFloat();
        z = ReadFloat();
        return new Vector3(x, y, z);
    }

    public void ReadBuffer(byte[] _buffer)
    {
        if (m_cursor + _buffer.Length > m_size)
            return;
        Array.Copy(m_data, m_cursor, _buffer, 0, _buffer.Length);
        m_cursor += _buffer.Length;
    }

    /*
     * this.Cursor = 0;
     */
    public void Rewind()
    { 
        this.m_cursor = 0;
    }
}
