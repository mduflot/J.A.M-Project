using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource audioSource;
    public AudioClip notificationChime;
    public AudioSource musicSource;
    public AudioSource ambientSource;

    public bool musicStopped;

    public int musicDelay;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if(Instance != null) return;
        Instance = this;
    }

    private void Update()
    {
        if (!musicSource.isPlaying && !musicStopped)
        {
            musicStopped = true;
            StartCoroutine(DelayMusic());
        }
    }


    private IEnumerator DelayMusic()
    {
        yield return new WaitForSeconds(Random.Range(musicDelay - 40, musicDelay));
        PlayMusic();
    }

    public void PlaySound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }

    private void PlayMusic()
    {
        musicSource.Play();
        musicStopped = false;
    }
    
}
