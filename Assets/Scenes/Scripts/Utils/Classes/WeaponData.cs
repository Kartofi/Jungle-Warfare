using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public GameObject GunTip;
    public GameObject bulletsContainer;
    public GameObject partsRoot;

    public Vector3 bulletsContainerStartLocalPos;

    public ParticleSystem[] particles;
    public string Name;
    public static WeaponData local;
    public bool IsLocal = false;
    private void Awake()
    {
        bulletsContainerStartLocalPos = bulletsContainer.transform.localPosition;
        if (IsLocal == false)
        {
            return;
        }
        WeaponData.local = this;
    }
    public void SetLocalThis()
    {
        WeaponData.local = this;
    }

}
