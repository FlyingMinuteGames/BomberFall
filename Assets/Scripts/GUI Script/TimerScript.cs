using UnityEngine;
using System.Collections;
using System.Globalization;
using System;

public class TimerScript : MonoBehaviour {

	// Use this for initialization
    private float startValue = 0f;
    private TextMesh textmesh;
    string result = "00.00";
    bool ended = false;
    NumberFormatInfo twodecimals;

	void Start () {
        twodecimals = new NumberFormatInfo();
        twodecimals.NumberDecimalDigits = 2;
	}

    public void Init(){
        startValue = GameMgr.Instance.gameIntel.game_duration;
        textmesh = gameObject.GetComponent<TextMesh>();
        textmesh.text = startValue.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if (startValue <= 0f)
        {
            if(!ended)
                EndOfTimer();
            return;
        }
       startValue -= Time.smoothDeltaTime;
       textmesh.text = startValue.ToString("N", twodecimals);//Math.Round(startValue, 2).ToString();
	}

    void EndOfTimer()
    {
        ended = true;
        Debug.Log("Kill everyone, this game is over !!!!!!!");
    }
}
