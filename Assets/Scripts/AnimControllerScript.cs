/**
 * AnimControllerScript
 *  --> Sets parameters to play animations 
 * 
 * Author: Jean-Vincent Lamberti
 * **/

using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public class AnimControllerScript : MonoBehaviour
{

    public float m_MovementAnimMult = 1.0f;
    public bool useCurves;
    private CapsuleCollider m_PlayerCollider;
    private Animator m_animator;

   
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");			
    static int jumpState = Animator.StringToHash("Base Layer.Jump");				
    static int jumpBackState = Animator.StringToHash("Base Layer.JumpBack");
    static int jumpLeftState = Animator.StringToHash("Base Layer.JumpLeft");
    static int jumpRightState = Animator.StringToHash("Base Layer.JumpRight");	
    static int jumpDownState = Animator.StringToHash("Base Layer.JumpDown");		
    static int rollState = Animator.StringToHash("Base Layer.Roll");
    static int fallState = Animator.StringToHash("Base Layer.Fall");

    private AnimatorStateInfo currentBaseState;			// Current State of the animator




    void Start()
    {
        //gameObject.transform.FindChild("RobotMesh").gameObject.renderer.material.color = Color.red + Color.yellow;
        m_PlayerCollider = gameObject.GetComponent<CapsuleCollider>();
        m_animator = gameObject.GetComponent<Animator>();

        if (m_animator.layerCount == 2)
            m_animator.SetLayerWeight(1, 1);

        m_animator.speed = m_MovementAnimMult;

    }

    /*
     *  FixedUpdate()
     *      To use instead of Update() when adding a force continuously
     * */
    void FixedUpdate()
    {
        m_animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        m_animator.SetFloat("Direction", Input.GetAxis("Horizontal"));

        currentBaseState = m_animator.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation

        m_animator.speed = m_MovementAnimMult; //1.5f;

        AnimationInfo[] currentClips = m_animator.GetCurrentAnimationClipState(0);
        bool isIdle = false;

        for (int i = 0, len = currentClips.Length; i < len; i++ )
        {

            if (currentClips[i].clip.name == "Idle2")
                isIdle = true;
        }

        if (currentBaseState.nameHash == locoState)
        {
            if (Input.GetButtonDown("Jump") && !isIdle)
            {

                m_animator.SetBool("Jump", true);
            }
            //else
            //    m_animator.speed = m_MovementAnimMult;

        }
        else if (currentBaseState.nameHash == jumpState || currentBaseState.nameHash == jumpBackState || currentBaseState.nameHash == jumpLeftState || currentBaseState.nameHash == jumpRightState)
        {
            if (!m_animator.IsInTransition(0))
            {
                m_animator.SetBool("Jump", false);
            }

            // Raycast to check distance from ground
            Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Raycast(ray, out hitInfo))
            {
                //If farther than 1.75f use match target in order to make the player translate towards the ground smoothly and end jump once on ground
                if (hitInfo.distance > 1.75f)
                {
                    m_animator.MatchTarget(hitInfo.point, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), 0.35f, 0.5f);
                }
            }
        }

    }

    //TODO DEFINES METHOD THE CHECK IF THE PLAYER IS FALLING OR NOT
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Ground") && !(currentBaseState.nameHash == jumpState || currentBaseState.nameHash == jumpBackState || currentBaseState.nameHash == jumpLeftState || currentBaseState.nameHash == jumpRightState))
        {
            Debug.Log("Falling like a fat fuck");
            m_animator.SetBool("JumpDown", true);
        }

    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.CompareTag("Ground"))
            m_animator.SetBool("JumpDown", false);

    }

}
