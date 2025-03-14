using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class FullScreenStuff : MonoBehaviour
{
    public static FullScreenStuff instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject KilledByUi;
    private TMP_Text killedby;


    public GameObject KilledUi;
    private TMP_Text killed;


    public GameObject Damaged;
    public GameObject Healed;

    public Image DamagedImg;
    public Image HealedImg;

    private void Start()
    {
        killedby = KilledByUi.transform.GetChild(0).GetComponent<TMP_Text>();
        killed = KilledUi.transform.GetChild(0).GetComponent<TMP_Text>();

        HealedImg = Healed.GetComponent<Image>();
        DamagedImg = Damaged.GetComponent<Image>();
    }
    //Killed BY text
    long killedById = 0;
    bool changedKilledBy = true;
    //Killed Text
    long killedId = 0;
    bool changedKilled = true;
    //Damaged screen
    float leftDurationDamaged = -1;
    //Healed screen
    float leftDurationHealed = -1;

    public void KilledByText(long id)
    {
        if (!Udp.client.idsObjs.ContainsKey(id))
        {
            return;
        }
        killedById = id;
        changedKilledBy = false;
    }

    public void KilledText(long id)
    {
        if (!Udp.client.idsObjs.ContainsKey(id))
        {
            return;
        }
        killedId = id;
        changedKilled = false;
    }

    public void DamagedScreen(float duration)
    {
        leftDurationDamaged = duration;
    }
    public void HealedScreen(float duration)
    {
        leftDurationHealed = duration;
    }

    void Update()
    {
        if (changedKilledBy == false)
        {
            string NameKilled = Udp.client.idsObjs.GetValueOrDefault(killedById).name;
            KilledByUi.SetActive(true);
            killedby.color = new Color(1,1,1,1);
            killedby.text = "Killed By<br> <color=red> " + NameKilled + " </color> ";
            changedKilledBy = true;
            killedby.DOFade(0, 0.5f).SetDelay(Udp.client.Rules.respawnTime / 1000).onComplete = KilledByDisable;
        }
        if (changedKilled == false)
        {
            string NameKilled = Udp.client.idsObjs.GetValueOrDefault(killedId).name;
            KilledUi.SetActive(true);
            killed.color = new Color(1,1,1,1);
            killed.text = "Killed<br><color=red>" + NameKilled + "</color>";
            changedKilled = true;
            killed.DOFade(0, 0.5f).SetDelay(2).onComplete = KilledDisable;
        }
    
        if (leftDurationDamaged > 0)
        {
            leftDurationDamaged -= Time.deltaTime;
            if (Damaged.activeSelf == false)
            {
                DamagedImg.color = new Color(0,0,0,0.16f) + DamagedImg.color;
                Damaged.SetActive(true);
            }
        }else if (leftDurationDamaged != -1 && leftDurationDamaged < 0)
        {
            DamagedImg.DOFade(0, 0.25f).onComplete = () => { Damaged.SetActive(false); };
            leftDurationDamaged = -1;
        }

        if (leftDurationHealed > 0)
        {
            leftDurationHealed -= Time.deltaTime;
            if (Healed.activeSelf == false)
            {
                HealedImg.color = new Color(0, 0, 0, 0.16f) + HealedImg.color;
                Healed.SetActive(true);
            }
        }
        else if (leftDurationHealed != -1 && leftDurationHealed < 0)
        {
            HealedImg.DOFade(0, 0.25f).onComplete = () => { Healed.SetActive(false); };
            leftDurationHealed = -1;
        }
    }
    void KilledByDisable()
    {
        KilledByUi.SetActive(false);
    }
     
    void KilledDisable()
    {
        KilledUi.SetActive(false);
    }
}
