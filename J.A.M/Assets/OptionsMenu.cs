using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private SoundSlider[] soundSliders;

    [Serializable]
    public struct SoundSlider
    {
        public Slider slider;
        public AudioSource AudioSource;
        public float maxValue;
    }

    private void Start()
    {
        soundSliders[0].AudioSource = SoundManager.Instance.musicSource;
        soundSliders[1].AudioSource = SoundManager.Instance.audioSource;
        soundSliders[2].AudioSource = SoundManager.Instance.ambientSource;
        
        foreach (var slider in soundSliders)
        {
            slider.slider.value = slider.AudioSource.volume / slider.maxValue;
        }
    }

    private void Update()
    {
        foreach (var slider in soundSliders)
        {
            slider.AudioSource.volume = slider.slider.value * slider.maxValue;
        }
    }
}
