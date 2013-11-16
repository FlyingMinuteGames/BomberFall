using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class ObjectMgr {

    public struct GOWrapper
    {
        public GOWrapper(GOType t, GameObject g, int guid)
        {
            go = g;
            type = t;
            this.guid = guid;
        }
        public GameObject go;
        public GOType type;
        public int guid;
    }
    private int _guid = 1;
    private static ObjectMgr s_instance = null;
    private Dictionary<int,GOWrapper> m_objects;
    
    public static ObjectMgr Instance
    {
        get {  return null == s_instance ?(s_instance = new ObjectMgr()) : s_instance; }
    }
    
    private ObjectMgr()
    {
        m_objects = new Dictionary<int, GOWrapper>();
    }

    public int Register(GameObject o,GOType type, int guid = -1)
    {
        if (guid < 0)
            guid = _guid++;
        Debug.Log("register go, type : " + type + ", guid :" + guid);
        m_objects[guid] = new GOWrapper(type,o,guid) ;
        return guid;
    }
    public GameObject get(int guid)
    {
        if (!m_objects.ContainsKey(guid))
            return null;
        return m_objects[guid].go;
    }
    public byte[] DumpData()
    {
        byte[] data = new byte[m_objects.Count * 16];
        int i = 0;
        foreach (var j in m_objects) 
        {
            GOWrapper go = j.Value;
            Vector3 pos  = go.go.transform.position;
            Array.Copy(BitConverter.GetBytes(go.guid), 0, data, i++ * 4,4);
            Array.Copy(BitConverter.GetBytes((int)go.type), 0, data, i++ * 4, 4);
            Array.Copy(BitConverter.GetBytes(pos.x), 0, data, i++ * 4, 4);
            Array.Copy(BitConverter.GetBytes(pos.z), 0, data, i++ * 4, 4);
        }
        return data;
    }
    public byte[] DumpData(int guid)
    {
        if (!m_objects.ContainsKey(guid))
            return null;
        byte[] data = new byte[16];
        int i = 0;
        GOWrapper go = m_objects[guid];
        Vector3 pos = go.go.transform.position;
        Array.Copy(BitConverter.GetBytes(go.guid), 0, data, i++ * 4, 4);
        Array.Copy(BitConverter.GetBytes((int)go.type), 0, data, i++ * 4, 4);
        Array.Copy(BitConverter.GetBytes(pos.x), 0, data, i++ * 4, 4);
        Array.Copy(BitConverter.GetBytes(pos.z), 0, data, i++ * 4, 4);
        return data;
    }

}
