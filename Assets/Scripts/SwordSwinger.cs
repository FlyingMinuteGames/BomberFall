using UnityEngine;
using System.Collections;

public class SwordSwinger : MonoBehaviour {

    private AudioClip m_audioclip;
    private AudioSource m_audiosource;
    private Animation m_animation;

	// Use this for initialization
	void Start () {
        m_audiosource = gameObject.GetComponent<AudioSource>();
        m_animation = gameObject.GetComponent<Animation>();
        m_audioclip = m_audiosource.clip;
        m_animation.Rewind();

        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    IEnumerator SwingRoutine()
    {
        m_audiosource.PlayOneShot(m_audioclip, PlayerPrefs.GetFloat("SoundVolume"));
        m_animation.Play();

        yield return new WaitForSeconds(m_animation.clip.length);
        m_animation.Rewind();
        gameObject.SetActive(false);
    }

    public void Swing()
    {
        gameObject.SetActive(true);
        StartCoroutine(SwingRoutine());
    }
}
