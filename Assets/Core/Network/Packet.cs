using UnityEngine;
using System.Collections;
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
    public int Size
    {
        get { return m_size; }
    }
    private int m_opcode;
    private TcpClient sender;
    public TcpClient Sender
    {
        get { return sender; }
        set { sender = value; }
    }
    private byte[] m_data;
    private int cursor = 0;
    private byte[] buffer = new byte[8];
    public Opcode GetOpcode()
    {
        return (Opcode)m_opcode;
    }

    public Packet(int size, int opcode)
    {

        m_data = new byte[size];
        m_size = size;
        m_opcode = opcode;
    }

    public Packet(int size, int opcode,byte[] data)
    {

        m_size = size;
        m_opcode = opcode;
        m_data = new byte[size];
        Array.Copy(data, m_data, m_size);
    }

    public Packet(int size, Opcode opcode)
    {

        m_data = new byte[size];
        m_size = size;
        m_opcode = (int)opcode;
    }

    public Packet(int size, Opcode opcode, byte[] data)
    {

        m_size = size;
        m_opcode = (int)opcode;
        m_data = new byte[size];
        Array.Copy(data, m_data, m_size);
    }



    public Packet(byte[] data)
    {

        m_size = ToInt(data);
        m_opcode = ToInt(data,4);
        m_data = new byte[data.Length-8];
        Array.Copy(data, 8, m_data, 0, m_size);
    }

    public byte[] ToByte()
    {
        byte[] data = new byte[4 + 4 + m_size];
        Packet.ToByte(data,m_size,0);
        Packet.ToByte(data,m_opcode,4);
        System.Array.Copy(m_data, 0, data, 8, m_size);
        return data;
    }


    public void  WriteInt(int value)
    {
        m_data[cursor++] = (byte)(value >> 24);
        m_data[cursor++] = ((byte)(value >> 16));
        m_data[cursor++] = ((byte)(value >> 8));
        m_data[cursor++] = ((byte)value);
    }

    public void Write (byte value)
    {
        m_data[cursor++] = value;
        //m_data.WriteByte(value);
    }
    public void Write (short value)
    {
        Array.Copy(BitConverter.GetBytes(value),0,m_data,cursor,2);
        cursor += 2;
    }
    public void Write (int value)
    {
        Array.Copy(BitConverter.GetBytes(value), 0, m_data, cursor, 4);
        cursor += 4;
    }
    public void Write (float value)
    {
        Array.Copy(BitConverter.GetBytes(value), 0, m_data, cursor, 4);
        cursor += 4;
    }

    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    public void Write(byte[] _buffer)
    {
        Array.Copy(_buffer, 0, m_data, cursor, _buffer.Length);
        cursor += _buffer.Length;
    }

    public void WriteVector3(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    public int ReadInt()
    {
        Debug.Log("==>"+cursor);
        var value = BitConverter.ToInt32(m_data, cursor);
        cursor += 4;
        return value;
    }

    public float ReadFloat()
    {
        var value = BitConverter.ToSingle(m_data, cursor);
        cursor += 4;
        return value;
    }

    public short ReadShort()
    {
        var value = BitConverter.ToInt16(m_data, cursor);
        cursor += 1;
        return value;
    }
    public byte ReadByte()
    {
        return m_data[cursor++];
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

        Array.Copy(m_data, cursor, _buffer, 0, _buffer.Length);
        cursor += _buffer.Length;
    }
}
