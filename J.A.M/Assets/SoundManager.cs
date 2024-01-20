using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioListener listener;
    public AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if(Instance != null) return;
        Instance = this;
    }

    public void PlaySound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }
    
    
    
}
