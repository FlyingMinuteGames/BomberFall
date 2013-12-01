using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Async : MonoBehaviour {

	// Use this for initialization
    private static Async instance = null;
    public delegate void Callback();
    private Queue<Callback> m_queue = new Queue<Callback>();
    private bool stop = false;
    private int max_action_frame = 10;
    public static Async Instance
    {
        get {
            /*if (null == instance)
            {
                GameObject async = ResourcesLoader.LoadResources<GameObject>("Prefab/Async");
                async = (GameObject)GameObject.Instantiate(async);
                return (instance = async.GetComponent<Async>());
            }
            else */return instance;
        }
    }
	
	// Update is called once per frame
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Restart();
    }

    public void Restart()
    {
        m_queue.Clear();
        StartCoroutine(HandleAction());
    }

    IEnumerator  HandleAction()
    {
        while (!stop)
        {
            //Debug.Log("try to do action");
            if (m_queue.Count == 0)
            {
                
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            lock (m_queue)
            {
                for (int i = 0, len = max_action_frame > m_queue.Count ? m_queue.Count : max_action_frame; i < len; i++)
                {
                    var action = m_queue.Dequeue();
                    action();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    /*public void Update()
    {
        HandleAction();
    }*/

    public void DelayedAction(Callback cb)
    {
        m_queue.Enqueue(cb);
    }
}
