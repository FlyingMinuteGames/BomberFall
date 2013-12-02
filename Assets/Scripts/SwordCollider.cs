using UnityEngine;
using System.Collections;

public class SwordCollider : MonoBehaviour {

	// Use this for initialization
    GameObject me;
    Guid m_guid;
	void Start () {
        me = transform.parent.parent.gameObject;
        m_guid = me.GetComponent<Guid>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if ((GameMgr.Instance.Type & GameMgrType.SERVER) == 0)
            return;
        if (col.CompareTag("Player"))
        {
            Guid guid = col.gameObject.GetComponent<Guid>();
            if (m_guid.GetGUID() == guid.GetGUID())
                return;//Avoid HaraKiri
            Debug.Log("I've touched a player");
            GameMgr.Instance.KillPlayer(guid.GetGUID(), m_guid.GetGUID(), Config.PowerType.BRING_A_SW_TO_A_GF);

        }
    }
}
