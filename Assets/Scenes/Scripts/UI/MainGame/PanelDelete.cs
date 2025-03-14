using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class PanelDelete : MonoBehaviour
{
    // Start is called before the first frame update
    public bool DeleteNovisible = false;
    public int maxY;
    Image img;
    void Start()
    {
        if (DeleteNovisible == false)
        {
            gameObject.transform.DOScaleX(0, 0.5f).SetDelay(5.5f).onComplete = DestroyAfter;
        }else
        {
            img = gameObject.GetComponent<Image>();
        }
    }
    
    private void FixedUpdate()
    {
        if (gameObject.transform.localPosition.y > maxY)
        {
           Destroy(gameObject);
        }
    }
   
    void DestroyAfter()
    {
        transform.DOComplete();
        transform.DOKill();
        Destroy(gameObject);
    }
}
