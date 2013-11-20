using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PoolSystem<T> where T : Object{
    private Queue<T> m_data;
    private T template;
    bool m_IsGo = false;
    public PoolSystem(T _template, int allocate)
    {
        template = _template;
        m_data = new Queue<T>(allocate);
        m_IsGo = template.GetType() == typeof(GameObject);
        for (int i = 0; i < allocate; i++)
        {
            Object obj = Object.Instantiate(template);
            T o = (T)obj;
            if (m_IsGo)
            {
                GameObject go = (GameObject)obj;
                UnityUtils.SetLayerRecursivlyOn(go.transform, Config.POOL_LAYER);
                go.SetActive(false);
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



	
}
