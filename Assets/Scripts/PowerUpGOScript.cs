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
        Vector3 originalTransform = goContainer.transform.position;
        goContainer = (GameObject)Instantiate(powerGOs[index], originalTransform, Quaternion.identity);

    }
}
