using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{

    public AudioClip[] MusicTracks;
    private int current = 0;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audio.clip = MusicTracks[current];
        audio.Play();
    }

    void Update()
    {
        audio.volume = PlayerPrefs.GetFloat("MusicVolume") / 5f;
    }

    public void PlayNextTrack()
    {
        audio.clip = MusicTracks[++current % 2];
        audio.Play();
    }
}
