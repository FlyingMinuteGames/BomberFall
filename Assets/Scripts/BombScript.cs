using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour {

    public delegate void /*IEnumerator*/ Callback();
    public Callback callback;
    public AudioClip explosion;
    private float[] timer = {4,2};
    private int progress = 0;
    private GameObject bomb_object;
    private Animation anim;
    private ParticleSystem panim;
    // Use this for initialization
	private Transform[] child;
    private bool m_IsInit = false;
    private Rigidbody m_RigidBody;
    private bool m_Explode = false;
    private static Quaternion[] s_BaseRotation = new Quaternion[] { Quaternion.identity, Quaternion.AngleAxis(90, Vector3.right), Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right), Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right), Quaternion.AngleAxis(-90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right) };
    private static RigidbodyConstraints[] s_constraint = new RigidbodyConstraints[] { ~RigidbodyConstraints.FreezePositionY, ~RigidbodyConstraints.FreezePositionZ, ~RigidbodyConstraints.FreezePositionZ, ~RigidbodyConstraints.FreezePositionX, ~RigidbodyConstraints.FreezePositionX};
    private Callback m_onExplode = null;
    private Callback m_onEnd = null;
    private int m_OwnerGuid = -1;

    public int OwnerGuid
    {
        get { return m_OwnerGuid; }
    }
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
        m_RigidBody = GetComponent<Rigidbody>();
        foreach (Transform t in bomb_object.transform)
            child[i++] = t;

    }

    private void SetActiveChild(bool enable=true)
    {
        foreach(Transform t in child)
            t.gameObject.SetActive(enable);
    }

    void OnChangePhase(WorldState state)
    {
        transform.rotation = s_BaseRotation[(int)state];
        m_RigidBody.constraints = s_constraint[(int)state];
        if (state == WorldState.CENTER)
            m_RigidBody.useGravity = false;
        else m_RigidBody.useGravity = true;
    }


    public void StartScript(int ownerGuid, Callback onExplode = null,Callback onEnd = null)
    {
        if (!m_IsInit)
            Init();
        m_OwnerGuid = ownerGuid;
        m_onExplode = onExplode;
        m_onEnd = onEnd;
        transform.rotation = s_BaseRotation[(int)GameMgr.Instance.State];
        m_RigidBody.constraints = s_constraint[(int)GameMgr.Instance.State];
        if (GameMgr.Instance.State == WorldState.CENTER)
            m_RigidBody.useGravity = false;
        else m_RigidBody.useGravity = true;
        m_Explode = false;
        StartCoroutine(WaitAndExplode());
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


    public void ForceExplode()
    {
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        Debug.Log("EXPLODE");
        if (m_Explode)
            yield break;
        m_Explode = true;
        audio.PlayOneShot(explosion, PlayerPrefs.GetFloat("SoundVolume") * 20f);
        Debug.Log("explode phase 2");
        SetActiveChild(false);
        ((BoxCollider)collider).center = Vector3.up * 100;
        if (m_onExplode != null)
            m_onExplode();//StartCoroutine(c1());

        yield return new WaitForSeconds(timer[1]);
        if (m_onEnd != null)
            m_onEnd();
        Debug.Log("explode!");
    }

    IEnumerator WaitAndExplode()
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
        yield return new WaitForSeconds(timer[0]);
        yield return StartCoroutine(Explode());
    }
}
