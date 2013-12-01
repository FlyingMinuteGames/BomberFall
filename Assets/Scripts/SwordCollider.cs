using UnityEngine;
using System.Collections;

public class SwordCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnColliderEnter(Collider col)
    {
        if ((GameMgr.Instance.Type & GameMgrType.SERVER) != 0)
            return;
        if (col.CompareTag("Player"))
        {

        }
    }
}
