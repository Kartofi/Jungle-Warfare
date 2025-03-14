using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BulletLine : MonoBehaviour
{
    private void RemoveLine()
    {
        Destroy(gameObject);
    }
    float TimeElapsed = 0f;
    private float deltaTime = 0.008f;

    private void FixedUpdate()
    {
        TimeElapsed += deltaTime;
        if (TimeElapsed >= 0.1f)
        {
            gameObject.GetComponent<LineRenderer>().startWidth -= 0.0015f;
            gameObject.GetComponent<LineRenderer>().endWidth -= 0.0015f;
            if (gameObject.GetComponent<LineRenderer>().startWidth <= 0)
            {
                RemoveLine();
            }
        }
        
    }
}
