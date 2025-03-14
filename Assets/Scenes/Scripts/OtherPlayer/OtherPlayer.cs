using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OtherPlayer : MonoBehaviour
{
    public new string name;
    public long id;

    public TMP_Text nameTag;
    public LoadImageFromUrl pfpTag;

    public float health;
    public int state = (int)PlayerStates.idle;

    public int bullets;
    public string weaponName;
    public bool reloading;
    public WeaponData weapon;

    public WeaponRules weaponRules;

    public WeaponData[] weapons;


    public int kills;
    public bool isDead;

    public Vector3 position;
    public Vector3 rotation;
    public CameraData cameraData;

    public GameObject headInvisible;
    public GameObject torsoInvisible;


    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    bool playedDead = false;
    private void FixedUpdate()
    {
        if (isDead == true)
        {
            if (playedDead == false)
            {
                SetActiveAllChilds(false);
                SoundFXManager.main.SpawnSoundAt(transform.position, SoundFX.die);
                playedDead = true;
            }
           
        }
        else
        {
            if (playedDead == true)
            {
                SetActiveAllChilds(true);
                playedDead = false;
            }
            if (nameTag.text != name && isDead == false)
            {
                nameTag.text = name;
                pfpTag.UpdateImageUsingId(id);
            }
            if (state == (int)PlayerStates.walking)
            {
                if (audioSource.isPlaying == false)
                {
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.isPlaying == true)
                {
                    audioSource.Stop();
                }
            }
          
        }
    }
    void SetActiveAllChilds(bool active)
    {
        int childs = gameObject.transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            GameObject obj = gameObject.transform.GetChild(i).gameObject;
            obj.SetActive(active);
        }
    }
    public void UpdateWeapon(string weapon)
    {
        foreach(WeaponData weaponInstance in weapons)
        {
            if (weaponInstance.Name != weapon)
            {
                weaponInstance.gameObject.SetActive(false);
                torsoInvisible.GetComponent<Animator>().SetBool(weaponInstance.Name, false);
            }
            else
            {
                torsoInvisible.GetComponent<Animator>().SetBool(weaponInstance.Name, true);
                this.weapon = weaponInstance;
                weaponInstance.gameObject.SetActive(true);
            }
        }
    }
}
