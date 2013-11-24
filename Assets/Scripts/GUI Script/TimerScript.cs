using UnityEngine;
using System.Collections;

public class TimerScript : MonoBehaviour {

	// Use this for initialization
    private float startValue;
    private TextMesh textmesh;

	void Start () {

	}

    void Init(){
        startValue = GameMgr.Instance.gameIntel.game_duration;
        textmesh = gameObject.GetComponent<TextMesh>();

        Debug.Log(textmesh);

        textmesh.text = startValue.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        //startValue -= Time.smoothDeltaTime;
        //textmesh.text = "yeah";

        //Debug.Log("Delta"+ Time.deltaTime);
	}
}
