using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletsDisplay : MonoBehaviour
{
    public static BulletsDisplay main;
    // Start is called before the first frame update
    public Image bulletsBar;
    public TMP_Text bullets;

    int lastBullets;
    void Start()
    {
        main = this;
        lastBullets = Player.instance.bullets;
        UpdateBulletsBar();
    }
    bool updateHealthBar = false;
    public void UpdateBulletsBar()
    {
        updateHealthBar = true;
    }
    // Update is called once per frame
    
    void Update()
    {
        if (updateHealthBar == true || lastBullets != Player.instance.bullets)
        {
            if (Player.instance.weaponRules.bulletsMax == 0)
            {
                return;
            }
            updateHealthBar = false;
            bulletsBar.DOFillAmount((float)Player.instance.bullets / (float)Player.instance.weaponRules.bulletsMax, 0.05f);
            bullets.text = Player.instance.bullets.ToString();
            lastBullets = Player.instance.bullets;
        }
    }
}
