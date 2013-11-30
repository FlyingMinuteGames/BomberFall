using UnityEngine;
using System.Collections;

public class PowerUpGOScript : MonoBehaviour {


    public GameObject[] powerGOs;
    private GameObject goContainer;
    private Config.PowerType type;

    // Use this for initialization
	void Start () {
        goContainer = gameObject.transform.FindChild("Power").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init()
    {
        int index = UnityEngine.Random.Range(0, 12);
        type = (Config.PowerType)index;
        Debug.Log("Type is "+ index);
        Vector3 originalTransform = gameObject.transform.position;
        goContainer = (GameObject)Instantiate(powerGOs[index], originalTransform+ new Vector3(0,-0.5f,0f), Quaternion.identity);
        goContainer.transform.parent = gameObject.transform;
    }
}
