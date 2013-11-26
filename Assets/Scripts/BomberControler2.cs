using UnityEngine;
using System.Collections;

public class BomberControler2 : MonoBehaviour {

    int moveFlags = 0;
    Rigidbody body;
    public float gravity_acceleration = 1.0f;
    public float jump_speed = 1.0f;
    public bool IsNetworkControlled = false;
    public bool IsPlayer = false;
    Animator m_animator;
    public WorldState state = WorldState.CENTER;
    Quaternion[] baseRotation = new Quaternion[] { Quaternion.identity, Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.forward) };
	void Start () {
	    body = GetComponent<Rigidbody>();
        m_animator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (IsPlayer && GameMgr.Instance && GameMgr.Instance.game_started)
            UpdateInput();
	}
    private int stack = 0;
    Vector3 move = Vector3.zero;
    public float speed = 1.0f;
    private float fall_velocity = 0;
    private Vector3 gravity = Vector3.back;
    delegate int Callback(BomberControler2 me, bool enable);
    private KeyCode[] key_binding = { (KeyCode)PlayerPrefs.GetInt("ForwardKey"), (KeyCode)PlayerPrefs.GetInt("BackwardKey"), (KeyCode)PlayerPrefs.GetInt("LeftKey"), (KeyCode)PlayerPrefs.GetInt("RightKey"), KeyCode.Space, (KeyCode)PlayerPrefs.GetInt("OffensiveItemKey"), (KeyCode)PlayerPrefs.GetInt("OffensiveItemKey")};
    private Callback[] action_callback = {
                                            /*(me,enable) => { me.m_force += enable ? Vector3.forward : Vector3.back; return 1;},
                                            (me,enable) => { me.m_force += !enable ? Vector3.forward : Vector3.back; return 1;},
                                            (me,enable) => { me.m_force += !enable ? Vector3.right : Vector3.left; return 1;},
                                            (me,enable) => { me.m_force += enable ? Vector3.right : Vector3.left; return 1;},*/
                                            (me,enable) => { if(me.stack > 0 && me.state != WorldState.CENTER) {me.fall_velocity =-0.15f*me.jump_speed; return 1;} if(me.state != WorldState.CENTER)return 0;me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_FORWARD : me.moveFlags & ~(int)MoveState.MOVE_FORWARD; return 1;},
                                            (me,enable) => { if(me.state != WorldState.CENTER) return 0; me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_BACKWARD : me.moveFlags & ~(int)MoveState.MOVE_BACKWARD; return 1;},
                                            (me,enable) => { me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_LEFT : me.moveFlags & ~(int)MoveState.MOVE_LEFT; return 1;},
                                            (me,enable) => { me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_RIGHT : me.moveFlags & ~(int)MoveState.MOVE_RIGHT; return 1;},
                                            (me,enable) => { if(enable) me.SpawnBomb(); return 1;}
                                           };
    void UpdateInput()
    {
        int flag = 0;
        for (int i = 0, len = key_binding.Length; i < len; i++)
        {
            if (Input.GetKeyDown(key_binding[i]))
                flag |= action_callback[i](this, true);
            else if (Input.GetKeyUp(key_binding[i]))
                flag |= action_callback[i](this, false);

            if ((flag & 1 )!= 0 && GameMgr.Instance != null)
                GameMgr.Instance.PlayerMove(moveFlags, transform.position);
        }
    }

    public void SpawnBomb()
    { 
        
        GameMgr.Instance.SpawnBomb(transform.position);
        /*GameObject o = pool.Pop(transform.position,new Quaternion());
        if (o == null)
        {
            Debug.Log("warning no bomb in pool !");
            return;
        }
        o.GetComponent<BombScript>().StartScript(() => {/* handle damage  return;}, () => { pool.Free(o); return; });
        */    
    }

    void FixedUpdate()
    {
        move = Vector3.zero;
        if ((moveFlags & (int)MoveState.MOVE_FORWARD) != 0)
            move += Vector3.forward;
        if ((moveFlags & (int)MoveState.MOVE_BACKWARD) != 0)
            move -= Vector3.forward;
        if ((moveFlags & (int)MoveState.MOVE_LEFT) != 0)
            move += Vector3.left;
        if ((moveFlags & (int)MoveState.MOVE_RIGHT) != 0)
            move -= Vector3.left;


        /*if(stack == 0 && state != WorldState.CENTER)
            fall_velocity += Time.deltaTime * gravity_acceleration;
        
        //  Debug.Log("fall velocity " + fall_velocity);
        if (fall_velocity > 20)
            fall_velocity = 20;*/
        m_animator.SetFloat("Speed", move.z);
        m_animator.SetFloat("Direction", move.x);
        transform.Translate(move * speed * Time.deltaTime - gravity*fall_velocity);
        transform.rotation = baseRotation[(int)state];
    }

    void OnCollisionEnter(Collision collision)
    {
        
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.z > 0.5)
                stack++;
        }
        if (stack > 0)
        {
            fall_velocity = 0;
            //action_callback[0](this, true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.z > 0.5)
                stack--;
            
        }
    }

    void OnRecvMove(object[] o)
    {
        this.moveFlags = (int)o[0];
        this.transform.position = (Vector3)o[1];
    }

}
