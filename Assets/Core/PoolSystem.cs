using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PoolSystem<T> where T : Object{
    private Queue<T> m_data;
    private T template;
    bool m_IsGo = false;
    int size;
    GameObject ctn_go;
    public PoolSystem(T _template, int allocate)
    {
        size = allocate;
        template = _template;
        Allocate();
    }

    public void Allocate()
    {
        m_data = new Queue<T>(size);
        m_IsGo = template.GetType() == typeof(GameObject);
        if (m_IsGo && ctn_go == null)
        {
            ctn_go = new GameObject();
            ctn_go.name = "Pool(" + template.name + ")";
        }

        for (int i = 0; i < size; i++)
        {
            Object obj = Object.Instantiate(template);
            T o = (T)obj;
            if (m_IsGo)
            {
                GameObject go = (GameObject)obj;
                UnityUtils.SetLayerRecursivlyOn(go.transform, Config.POOL_LAYER);
                go.SetActive(false);
                go.transform.parent = ctn_go.transform;
            }
            m_data.Enqueue(o);
        }
    }

    public T Pop(Vector3 pos, Quaternion rot)
    {
        if (m_data.Count == 0)
            return null;
        T o =  m_data.Dequeue();
        if (m_IsGo)
        {
            GameObject go = (GameObject)((Object)o);
            go.SetActive(true);
            UnityUtils.SetLayerRecursivlyOn(go.transform, 0);
            Debug.Log("postion : "+pos);
            pos.y = go.transform.position.y;
            go.transform.position = pos;
            go.transform.rotation = rot;
        }
        return o;
    }

    public void Free(T o)
    {
        if (template.GetType() != o.GetType())
            return;

        if (m_IsGo)
        {
            GameObject go = (GameObject)((Object)o);
            UnityUtils.SetLayerRecursivlyOn(go.transform, Config.POOL_LAYER);
            go.SetActive(false);
        }
        m_data.Enqueue(o);
    }


    public void Clear()
    {
        List<GameObject> go = new List<GameObject>();
        foreach (Transform t in ctn_go.transform)
            go.Add(t.gameObject);

        foreach (GameObject _go in go)
        {
            _go.transform.parent = null;
            GameObject.Destroy(_go);
        }              
    }


    public void ClearAndRealloc()
    {
        Clear();
        Allocate();
    }
}
