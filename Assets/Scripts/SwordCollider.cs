using UnityEngine;
using System.Collections;

public class SwordCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Colliding with stuff !!!!!!!!!!!!!!");
        if ((GameMgr.Instance.Type & GameMgrType.SERVER) == 0)
            return;
        Debug.Log("I AM the SERVER !!!, Walter White");
        if (col.CompareTag("Player"))
        {
            Debug.Log("I've touched a player");
            //GameMgr.Instance.KillPlayer(col.gameObject.GetComponent<Guid>().GetGUID(), gameObject.GetComponent<Guid>().GetGUID(), Config.PowerType.BRING_A_SW_TO_A_GF);

        }
    }
}
