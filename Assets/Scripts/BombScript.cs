using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour {

    public delegate void /*IEnumerator*/ Callback();
    public Callback callback;
    private float[] timer = {4,2};
    private int progress = 0;
    private GameObject bomb_object;
    private Animation anim;
    private ParticleSystem panim;
    // Use this for initialization
	private Transform[] child;
    private bool m_IsInit = false;
    void Start () {
        Init();

    }
    private void Init()
    {
        if (m_IsInit)
            return;
        m_IsInit = true;
        
        var i = 0;
        bomb_object = transform.GetChild(0).gameObject;
        child = new Transform[bomb_object.transform.childCount];
        anim = bomb_object.GetComponent<Animation>();
        panim = bomb_object.GetComponent<ParticleSystem>();

        foreach (Transform t in bomb_object.transform)
            child[i++] = t;
    }

    private void SetActiveChild(bool enable=true)
    {
        foreach(Transform t in child)
            t.gameObject.SetActive(enable);
    }


    public void StartScript(Callback onExplode = null,Callback onEnd = null)
    {
        if (!m_IsInit)
            Init();
        StartCoroutine(WaitAndExplode(onExplode,onEnd));
    }

    IEnumerator WaitAndExplode(Callback c1,Callback c2)
    {
        progress = 0;
        Debug.Log("begin explode");
        bomb_object.SetActive(true);
        SetActiveChild(true);
        anim.Rewind();
        anim.Play();
        panim.time = 0;
        panim.Clear(true);
        panim.Play();

        yield return new WaitForSeconds(timer[progress++]);

        Debug.Log("explode phase 2");
        //bomb_object.SetActive(false);
        SetActiveChild(false);
        if (c1 != null)
            c1();//StartCoroutine(c1());

        yield return new WaitForSeconds(timer[progress++]);
        if (c2 != null)
            c2();//StartCoroutine(c2());
        Debug.Log("explode!");
    }

    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        NetworkMgr.Instance.RegisterObj(this,NetworkMgr.ObjectType.OBJECT_BOMB);
    }
	
	// Update is called once per frame
	void Update () {
        /*if (timer <= 0)
        {
            Debug.Log("explode!");
            if (callback != null)
                callback();
        }
        else timer -= Time.deltaTime;*/
	}
}
