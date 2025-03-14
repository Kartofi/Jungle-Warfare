using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class HitHudEffects : MonoBehaviour
{
    public float time = 1;
    public float height = 2;
    
    
    void Start()
    {
        transform.DOScale(1.2f, time/10f).SetEase(Ease.InOutQuint);
        transform.DOScale(1f, time / 10f).SetEase(Ease.InOutQuint).SetDelay(time / 10f);

        transform.DOMoveY(transform.position.y + height,time).onComplete = () => Destroy(gameObject);
        transform.GetChild(0).GetComponent<TMP_Text>().DOFade(0, time).SetEase(Ease.InOutQuint);
    }
}
