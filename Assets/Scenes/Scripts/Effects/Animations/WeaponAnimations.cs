using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    
    public static WeaponAnimations local;

    Animator animator;
    public GameObject LeftArmTarget;
    public OtherPlayer playerData;

    float reloadAnimDuration = 0.217f;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip takeAmmo;
    public AudioClip putAmmo;

    public bool isLocal = false;

    private void Awake()
    {
        if (isLocal == true)
        {
            local = this;
        }
        
    }
    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }
    public void Reload()
    {
        animator.StopPlayback();
        if (isLocal == true)
        {
            animator.speed = reloadAnimDuration / Player.instance.weaponRules.reloadTime ;
            animator.SetTrigger("Reload");
        }
        else
        {
            
            animator.speed = reloadAnimDuration / playerData.weaponRules.reloadTime;
            animator.SetTrigger("Reload");
        }
    }
    public void GrabAmmo()
    {
        source.clip = takeAmmo;
        
        if (isLocal == true)
        {
            if (Player.instance.weapon == "Revolver")
            {
                source.pitch = 2;
            }
            else
            {
                source.pitch = 1;
            }
            WeaponData.local.bulletsContainer.transform.parent = LeftArmTarget.transform;
            WeaponData.local.bulletsContainer.transform.localPosition = new Vector3();
        }
        else
        {
            if (playerData.weaponName == "Revolver")
            {
                source.pitch = 2;
            }
            else
            {
                source.pitch = 1;
            }
            playerData.weapon.bulletsContainer.transform.parent = LeftArmTarget.transform;
            playerData.weapon.bulletsContainer.transform.localPosition = new Vector3();
        }
        source.Play();
    }
    public void PutBackAmmo()
    {
        source.clip = putAmmo;
        source.Play();
        if (isLocal == true)
        {
            WeaponData.local.bulletsContainer.transform.parent = WeaponData.local.partsRoot.transform;
            WeaponData.local.bulletsContainer.transform.localPosition = WeaponData.local.bulletsContainerStartLocalPos;
        }
        else
        {
            playerData.weapon.bulletsContainer.transform.parent = playerData.weapon.partsRoot.transform;
            playerData.weapon.bulletsContainer.transform.localPosition = playerData.weapon.bulletsContainerStartLocalPos;
        }
        
    }
}
