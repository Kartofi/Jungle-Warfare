using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager main;

    public AudioMixer masterMixer;
    public AudioMixer musicMixer;

    private void Awake()
    {
        main = this;
    }
    public void SetVolume(int value)
    {
        masterMixer.SetFloat("MasterVolume", value - 80);
    }
    public void SetMusicVolume(int value)
    {
        musicMixer.SetFloat("MusicVolume", value - 80);
    }
}
