using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDIeEffect : MonoBehaviour
{
    public ParticleSystem ps;

  

    // Update is called once per frame
    void Update()
    {
        if (ps.particleCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
