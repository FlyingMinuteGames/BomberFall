using UnityEngine;
using System.Collections;

public class CollideTrigger : MonoBehaviour {

    GameObject parent;
    void Start()
    {
        parent = this.transform.parent.gameObject;
    
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Bomb"))
        {
            Debug.Log("ignore collision with "+ col.name);
            Physics.IgnoreCollision(col, parent.collider);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Bomb"))
        {
            Debug.Log("don't ignore collision with " + col.name);
            Physics.IgnoreCollision(col, parent.collider, false);
        }
    }
}
