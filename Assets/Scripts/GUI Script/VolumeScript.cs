using UnityEngine;
using System.Collections;

public class VolumeScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<AudioSource>().audio.volume = PlayerPrefs.GetFloat("SoundVolume");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
