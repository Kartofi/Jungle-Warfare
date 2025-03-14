using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UiButtonEffect : MonoBehaviour
{
    public static UiButtonEffect main;
    private void Awake()
    {
        main = this;
    }
    public AudioSource audioSource;
    // Start is called before the first frame update
    public void ButtonClickSound()
    {
       audioSource.Play();
    }
}
