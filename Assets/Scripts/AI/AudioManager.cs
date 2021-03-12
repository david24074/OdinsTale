using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager audioManager;
    [SerializeField] private AudioSource fxSource, musicSource;
    [SerializeField] private AudioClip[] musicClips;

    private void Start()
    {
        //We want this object to be with us in every scene. When a scene starts we check if its there, if it is destroy it and keep this one
        //If its not there then set this audioManager to be the global audioManager
        if (audioManager)
        {
            Destroy(gameObject);
        }
        else
        {
            audioManager = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(startNewMusicClip());
        }
    }

    //We call this for when it doesnt matter from which 3D space the audio comes from
    public static void PlayAudioClipGlobal(AudioClip clip)
    {
        audioManager.fxSource.PlayOneShot(clip);
    }

    private IEnumerator startNewMusicClip()
    {
        AudioClip musicClip = musicClips[Random.Range(0, musicClips.Length)];
        musicSource.PlayOneShot(musicClip);
        yield return new WaitForSeconds(musicClip.length);
        StartCoroutine(startNewMusicClip());
    }
}
