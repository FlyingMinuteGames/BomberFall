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
        CheckIfWithinPlayer();
    }

    void OnTriggerExit(Collider collision)
    {
        Debug.Log("Exit collider !");
        if (collision.gameObject.tag != "PlayerTrigger")
            return;

        Debug.Log("Enable collision with bomb");
        Collider col = collision.transform.parent.collider;
        Physics.IgnoreCollision(collider, col, false);
    }

    public void CheckIfWithinPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.0f);
        foreach (Collider col in hitColliders)
            if (col.gameObject.tag == "Player")
            {
                Debug.Log("Disable collision with bomb");
                Physics.IgnoreCollision(collider, col, true);
            }
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
        ((BoxCollider)collider).center = Vector3.zero;
        yield return new WaitForSeconds(timer[progress++]);

        Debug.Log("explode phase 2");
        //bomb_object.SetActive(false);
        SetActiveChild(false);
        ((BoxCollider)collider).center = Vector3.up * 100;
        //collider.enabled = false;
        if (c1 != null)
            c1();//StartCoroutine(c1());

        yield return new WaitForSeconds(timer[progress++]);
        if (c2 != null) 
            c2();
        Debug.Log("explode!");
    }
}
