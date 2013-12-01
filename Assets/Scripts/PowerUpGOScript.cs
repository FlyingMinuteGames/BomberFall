using UnityEngine;
using System.Collections;

public class PowerUpGOScript : MonoBehaviour {


    public GameObject[] powerGOs;
    public static APowerUp[] powers = {
                                          new AsNeilTaughtMe(), 
                                          new BackToSchool(), 
                                          new BombSquad(), 
                                          new BombUp(), 
                                          new BringASwordToAGF(), 
                                          new FireUp(), 
                                          new ImpenetrableTrinket(), 
                                          new KickItLikeYouMeanIt(), 
                                          new RandomizatronTeleporter(), 
                                          new SpeedUp(), 
                                          new TheHomeRunner(), 
                                          new TheVengefulPhenix()
                                      };

    private GameObject goContainer;
    public Config.PowerType type;
    private Collider m_collider;

    // Use this for initialization
	void Start () {
        m_collider = gameObject.GetComponent<Collider>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init()
    {
        Debug.Log("Type is "+ type);
        Vector3 originalTransform = gameObject.transform.position;
        GameObject go = (GameObject)Instantiate(powerGOs[(int)type], powerGOs[(int)type].transform.position/*+ new Vector3(0,-0.5f,0f)*/, Quaternion.identity);
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = powerGOs[(int)type].transform.position;
        go.transform.rotation = powerGOs[(int)type].transform.rotation;
        gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
    }

    void OnTriggerEnter(Collider col){

        if ((GameMgr.Instance.Type & GameMgrType.SERVER) != 0 && col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has entered" + col.gameObject.GetComponent<Guid>().GetGUID());
            GameMgr.Instance.PowerUpPickUp(gameObject, col.gameObject.GetComponent<Guid>().GetGUID(), powers[(int)type]);
        }
    }
}
