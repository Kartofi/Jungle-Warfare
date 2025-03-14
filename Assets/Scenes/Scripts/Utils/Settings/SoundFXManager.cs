using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundFX
{
    shoot = 0,
    die = 1
}
public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager main;

    public AudioMixer mixer;

    public AudioClip shootShound;
    public AudioClip dieSound;

    public GameObject audioPlayer;

    [Header("Audio Settings")]
    public float volumeShoot = 0.85f;
    public float volumePitch = 0.5f;

    private void Awake()
    {
        main = this;
    }
    public void SpawnSoundAt(Vector3 pos, SoundFX index, int weaponIndex = 0)
    {
        GameObject clone = Instantiate(audioPlayer);
        clone.transform.position = pos;
        AudioSource audio = clone.GetComponent<AudioSource>();
        if (index == SoundFX.shoot)
        {
            audio.clip = shootShound;
            if (weaponIndex == 1)
            {
                audio.volume = volumeShoot;
                audio.pitch = volumePitch;
            }
            
            
            audio.PlayDelayed(0.1f);
        }
        else if (index == SoundFX.die)
        {
            audio.clip = dieSound;
            audio.PlayDelayed(0.1f);
        }
    }
}
