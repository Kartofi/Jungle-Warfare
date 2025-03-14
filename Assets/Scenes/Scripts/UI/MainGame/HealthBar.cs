using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class HealthBar : MonoBehaviour
{
    public static HealthBar main;
    // Start is called before the first frame update
    public Image healthBar;
    public TMP_Text health;
    public Gradient colorTransition;
    void Start()
    {
        main = this;
        UpdateHealthBar();
    }
    bool updateHealthBar = false;
    public void UpdateHealthBar()
    {
        updateHealthBar = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (updateHealthBar == true)
        {
            updateHealthBar = false;
            healthBar.DOColor(colorTransition.Evaluate(Player.instance.health / Udp.client.Rules.maxHealth), 0.1f);
            healthBar.DOFillAmount(Player.instance.health / Udp.client.Rules.maxHealth, 0.05f);
            health.text = Player.instance.health.ToString();
        }
    }
}
