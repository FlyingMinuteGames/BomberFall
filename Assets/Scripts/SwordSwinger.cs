using UnityEngine;
using System.Collections;

public class SwordSwinger : MonoBehaviour {

    private AudioClip m_audioclip;
    private AudioSource m_audiosource;
    private Animation m_animation;
    private GameObject m_sword;

	// Use this for initialization
	void Start () {
        m_audiosource = gameObject.GetComponent<AudioSource>();
        m_animation = gameObject.GetComponent<Animation>();
        m_audioclip = m_audiosource.clip;
        m_animation.Rewind();
        m_sword = gameObject.transform.FindChild("sword").gameObject;

        m_sword.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    IEnumerator SwingRoutine()
    {
        Debug.Log("Start swinging");
        m_audiosource.PlayOneShot(m_audioclip, PlayerPrefs.GetFloat("SoundVolume"));
        m_animation.Play();

        yield return new WaitForSeconds(m_animation.clip.length);
        Debug.Log("end of swing :(");
        m_animation.Rewind();
        m_sword.SetActive(false);
    }

    public void Swing()
    {
        m_sword.SetActive(true);
        StartCoroutine(SwingRoutine());
    }
}
