using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BomberController : MonoBehaviour
{

    int m_MoveFlags = 0;
    private static Color[] s_color = new Color[] {Color.white, Color.red,Color.blue,Color.yellow+Color.red};
    private int m_colorIndex = 0;

    public int ColorIndex
    {
        get { return m_colorIndex; }
        set {
            m_colorIndex = value;
            this.transform.FindChild("RobotMesh").renderer.material.color = s_color[m_colorIndex];
            }
    }
    public float m_GravityAcceleration = 0.0f;
    public float m_JumpSpeed = 1.0f;
    public bool m_IsNetworkControlled = false;
    public bool m_IsPlayer = false;
    Animator m_Animator;
    private SwordSwinger m_swordSwinger;
    public WorldState m_State = WorldState.CENTER;
    private static Quaternion[] s_BaseRotation = new Quaternion[] { Quaternion.identity, Quaternion.AngleAxis(90, Vector3.right), Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right), Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right), Quaternion.AngleAxis(-90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right) };
    private static Quaternion[] s_ExtraRotation = new Quaternion[] { Quaternion.identity, Quaternion.AngleAxis(180, Vector3.up), Quaternion.AngleAxis(90, Vector3.up), Quaternion.AngleAxis(-90, Vector3.up) };
    private static Vector3[] s_BaseGravity = new Vector3[] { Vector3.zero, Vector3.up };
    private static int[][] StateIndex = new int[][] { new int[] { 2, 1 }, new int[] { 2, -1 }, new int[] { 0, 1 }, new int[] { 0, -1 } };

    public int hasOffensiveItem = -1;

    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        m_guid = gameObject.GetComponent<Guid>();
        m_swordSwinger = gameObject.transform.FindChild("SwordSwinger").GetComponent<SwordSwinger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPlayer && GameMgr.Instance && GameMgr.Instance.game_started)
            UpdateInput();

    }
    private bool m_IsOnGround = true;

    public bool IsOnGround
    {
        get { return m_IsOnGround; }
    }
    private Dictionary<int, bool> m_contact = new Dictionary<int, bool>();
    private int stack = 0;
    Vector3 move = Vector3.zero;
    public float speed = 1.0f;
    private float fall_velocity = 0;
    private Vector3 gravity = Vector3.back;
    private Guid m_guid;
    delegate int Callback(BomberController me, bool enable);

    private Dictionary<KeyCode, Callback> m_actions = new Dictionary<KeyCode, Callback>()
    {
        {(KeyCode)PlayerPrefs.GetInt("ForwardKey"),Jump},
        {(KeyCode)PlayerPrefs.GetInt("BackwardKey"),(Callback)((me,enable) => { if(me.m_State != WorldState.CENTER) return 0; me.m_MoveFlags = enable ? me.m_MoveFlags | (int)MoveState.MOVE_BACKWARD : me.m_MoveFlags & ~(int)MoveState.MOVE_BACKWARD; return 1;})},
        {(KeyCode)PlayerPrefs.GetInt("LeftKey"),(Callback)((me,enable) => { me.m_MoveFlags = enable ? me.m_MoveFlags | (int)MoveState.MOVE_LEFT : me.m_MoveFlags & ~(int)MoveState.MOVE_LEFT; return 1;})},
        {(KeyCode)PlayerPrefs.GetInt("RightKey"), (Callback)((me,enable) => { me.m_MoveFlags = enable ? me.m_MoveFlags | (int)MoveState.MOVE_RIGHT : me.m_MoveFlags & ~(int)MoveState.MOVE_RIGHT; return 1;})},
        {KeyCode.Space,(Callback)((me,enable) => { if(enable) me.SpawnBomb(); return 0;})},
        {(KeyCode)PlayerPrefs.GetInt("OffensiveItemKey"),(Callback)((me,enable)=>{if (enable)me.UseOffensiveItem();return 0;})},
        {(KeyCode)PlayerPrefs.GetInt("DefensiveItemKey"),(Callback)((me,enable)=>{/*me.UseDefensiveItem();*/return 0;})}
        /*{KeyCode.Alpha8,(Callback)((me,enable)=>{ if(enable)GameMgr.Instance._switch(); return 0;})}*/
    };

    private int bombCount = 1;

    public int BombCount
    {
        get { return bombCount; }
    }
    private int bombSpawn = 0;

    public int BombSpawn
    {
        get { return bombSpawn; }
        set { bombSpawn = value; }
    }
    private int speedMult = 1;
    private int bombRadius = 1;

    public int BombRadius
    {
        get { return bombRadius; }
    }

    public void Swing()
    {
        m_swordSwinger.Swing();
    }

    void UpdateInput()
    {
        int flag = 0;
        foreach (var o in m_actions)
        {
            if (Input.GetKeyDown(o.Key))
                flag |= o.Value(this, true);
            else if (Input.GetKeyUp(o.Key))
                flag |= o.Value(this, false);

            if ((flag & 1) != 0 && GameMgr.Instance != null)
                GameMgr.Instance.PlayerMove(m_MoveFlags, transform.position);
            if ((flag & 2) != 0 && GameMgr.Instance != null)
                GameMgr.Instance.PlayerJump(transform.position);
            
        }
    }

    public void SpawnBomb()
    {
        if (!m_IsOnGround)
            return;
        GameMgr.Instance.SpawnBomb(m_guid.GetGUID(), transform.position);
    }

    public void UseOffensiveItem()
    {
        GameMgr.Instance.UseOffensiveItem(m_guid.GetGUID());
    }


    void FixedUpdate()
    {
        move = Vector3.zero;
        if ((m_MoveFlags & (int)MoveState.MOVE_FORWARD) != 0)
            move += Vector3.forward;
        if ((m_MoveFlags & (int)MoveState.MOVE_BACKWARD) != 0)
            move -= Vector3.forward;
        if ((m_MoveFlags & (int)MoveState.MOVE_LEFT) != 0)
            move += Vector3.left;
        if ((m_MoveFlags & (int)MoveState.MOVE_RIGHT) != 0)
            move -= Vector3.left;

        if (m_State != WorldState.CENTER)
        {
            if (!m_IsOnGround)
                fall_velocity += Time.deltaTime * m_GravityAcceleration;
            if (fall_velocity > 20)
                fall_velocity = 20;
        }
        m_Animator.SetFloat("Speed", move.z);
        m_Animator.SetFloat("Direction", move.x);
        m_Animator.SetBool("IsOnGround", m_IsOnGround);
        m_Animator.SetFloat("FallVelocity", fall_velocity);
        m_Animator.speed = 3f;
        transform.Translate(move.normalized * speed * ((1+speedMult)*0.5f) * Time.deltaTime - s_BaseGravity[(int)m_State != 0 ? 1 : 0] * fall_velocity);

    }

    void OnCollisionEnter(Collision collision)
    {
        //register collider
        if(m_State == WorldState.CENTER)
            return;
        m_contact[collision.gameObject.GetInstanceID()] = false;

        foreach (ContactPoint contact in collision.contacts)
        {
            //Debug.DrawRay(contact.point, contact.normal, Color.red);
            // z < -0.5
            // -z < -0.5
            if (StateIndex[(int)m_State - 1][1] * contact.normal[StateIndex[(int)m_State - 1][0]] < -0.5)
                fall_velocity = fall_velocity < 0 ? fall_velocity / 2 : fall_velocity;
            if (StateIndex[(int)m_State - 1][1] * contact.normal[StateIndex[(int)m_State-1][0]] > 0.7)
            {
                m_contact[collision.gameObject.GetInstanceID()] = true;
                m_IsOnGround = true;
                break;
            }
        }

        if (m_IsOnGround)
            fall_velocity = 0;
    }

    public void ResetOnGround()
    {
        m_IsOnGround = false;
        //WakeUp physics
        //transform.Translate(0, 0, 0);
    }

    void OnCollisionExit(Collision collision)
    {
        
        m_contact.Remove(collision.gameObject.GetInstanceID());
        
        if (m_State == WorldState.CENTER || !m_IsOnGround)
            return;
        UpdateOnGround();
    }

    void OnCollisionStay(Collision collision)
    {
        if(m_State == WorldState.CENTER)
            return;
        stack = 0;


        foreach (ContactPoint contact in collision.contacts)
        {
            if (StateIndex[(int)m_State - 1][1] * contact.normal[StateIndex[(int)m_State - 1][0]] > 0.5)
            {
                stack = 1;
                break;
            }
        }
        if (stack > 0 && !m_IsOnGround)
            m_IsOnGround = true;
        else if (stack == 0 && m_IsOnGround)
            UpdateOnGround();

        if (m_IsOnGround)
            fall_velocity = 0;
    }

    private void UpdateOnGround()
    {
        foreach (var kv in m_contact)
        {
            if (kv.Value)
            {
                m_IsOnGround = true;
                return;
            }
        }
        m_IsOnGround = false;
    }

    void OnChangePhase(short[] value)
    {
        m_State = (WorldState)value[0];
        m_MoveFlags = 0;
        m_IsOnGround = m_State == WorldState.CENTER ? true : false;
        if (m_State == WorldState.CENTER)
            transform.rotation = s_ExtraRotation[value[1]];
        else
            transform.rotation = s_BaseRotation[(int)m_State];
    }


    void OnRecvMove(object[] o)
    {
        if (m_IsPlayer)
            return;
        this.m_MoveFlags = (int)o[0];
        this.transform.position = (Vector3)o[1];
    }

    private static int Jump(BomberController me, bool enable)
    {
        me.m_Animator.SetBool("Jump", false);

        if (me.m_IsOnGround && me.m_State != WorldState.CENTER && enable)
        {
            me.m_Animator.SetBool("Jump", true);

            me.fall_velocity = -0.15f * me.m_JumpSpeed;
            return 2;
        }

        if (me.m_State != WorldState.CENTER)
            return 0;

        me.m_MoveFlags = enable ? me.m_MoveFlags | (int)MoveState.MOVE_FORWARD : me.m_MoveFlags & ~(int)MoveState.MOVE_FORWARD;
        return 1;

    }

    public void RecvJump(Vector3 start)
    {
        if (m_IsPlayer)
            return;
        transform.position = start;
        fall_velocity = -0.15f * m_JumpSpeed;
    }

    public void RecvIncBombCount()
    {
        bombCount++;
    }

    public void RecvIncRadius()
    {
        bombRadius++;
    }


    public int GetSpeedMult()
    {
        return speedMult;
    }

    public int GetBombRadius(){
        return bombRadius;
    }

    public void RecvIncSpeedMult()
    {
        if (speedMult >= 6)
            return;
        speedMult++;
        m_Animator.speed *= (3 * 0.5f);

    }

}
