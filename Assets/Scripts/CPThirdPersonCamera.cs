using UnityEngine;
using System.Collections;


//SCRIPT FROM UNITY TUTORIALS
public class CPThirdPersonCamera : MonoBehaviour
{
    public float smooth = 3f;		// a public variable to adjust smoothing of camera motion
    Transform standardPos;			// the usual position for the camera, specified by a transform in the game
    Transform lookAtPos;			// the position to move the camera to when using head look
    Vector3 newPosition;
    void Start()
    {
        // initialising references
        standardPos = GameObject.Find("robot_mesh_and_rig").transform;
        transform.Rotate(transform.rotation.x + 25f, transform.rotation.y, transform.rotation.z);

    }

    void FixedUpdate()
    {
        // if we hold Alt
      
            // return the camera to standard position and direction

        //transform.RotateAround(standardPos.position, Vector3.forward, 20 * Time.deltaTime);        
            newPosition = standardPos.position + new Vector3(0f, 2f, -3f);

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * smooth);
        //    transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.deltaTime * smooth);

    }
}
