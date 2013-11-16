﻿using UnityEngine;
using System.Collections;

public class BomberControler2 : MonoBehaviour {

    int moveFlags = 0;
    Rigidbody body;
    PoolSystem<GameObject> pool;
    public float gravity_acceleration = 1.0f;
    public float jump_speed = 1.0f;
    public bool IsNetworkControlled = false;
    public bool IsPlayer = false;
	void Start () {
	    body = GetComponent<Rigidbody>();
        GameObject Bomb = ResourcesLoader.LoadResources<GameObject>("Prefabs/Bomb");
        pool = new PoolSystem<GameObject>(Bomb,10);
	}
	
	// Update is called once per frame
	void Update () {
        if (IsPlayer)
            UpdateInput();
	}
    private int stack = 0;
    Vector3 move = Vector3.zero;
    public float speed = 1.0f;
    private float fall_velocity = 0;
    private Vector3 gravity = Vector3.back;
    delegate int Callback(BomberControler2 me, bool enable);
    private KeyCode[] key_binding = { KeyCode.Z, KeyCode.S, KeyCode.Q, KeyCode.D, KeyCode.Space };
    private Callback[] action_callback = {

                                            /*(me,enable) => { me.m_force += enable ? Vector3.forward : Vector3.back; return 1;},
                                            (me,enable) => { me.m_force += !enable ? Vector3.forward : Vector3.back; return 1;},
                                            (me,enable) => { me.m_force += !enable ? Vector3.right : Vector3.left; return 1;},
                                            (me,enable) => { me.m_force += enable ? Vector3.right : Vector3.left; return 1;},*/
                                            (me,enable) => { /*if(me.stack > 0) me.fall_velocity =-0.15f*me.jump_speed; return 0;},*/me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_FORWARD : me.moveFlags & ~(int)MoveState.MOVE_FORWARD; return 1;},
                                            (me,enable) => {  /*return 0;},*/me.moveFlags = enable ? me.moveFlags | (int)MoveState.MOVE_BACKWARD : me.moveFlags & ~(int)MoveState.MOVE_BACKWARD; return 1;},
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

            if ((flag & 1 )!= 0)
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
*/    }



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


        /*if(stack == 0)
            fall_velocity += Time.deltaTime * gravity_acceleration;
        
        //  Debug.Log("fall velocity " + fall_velocity);
        if (fall_velocity > 20)
            fall_velocity = 20;*/
        transform.Translate(move * speed * Time.deltaTime + gravity*fall_velocity);
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
