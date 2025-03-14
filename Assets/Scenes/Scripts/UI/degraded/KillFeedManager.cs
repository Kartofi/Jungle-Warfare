using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillMessage
{
    public string from;
    public string to;
    public KillMessage(string from,string to)
    {
        this.from = from;
        this.to = to;
    }
}
public class KillFeedManager : MonoBehaviour
{
    public static KillFeedManager instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject KillFeed;
    public GameObject Panel;

    public List<KillMessage> messagesToAdd = new List<KillMessage>();
    bool updateNeed = false;
    public void NewMessage(string from,string to)
    {
        messagesToAdd.Add(new KillMessage(from, to));
        updateNeed = true;
    }
    private void Update()
    {
        if (updateNeed == true)
        {
            updateNeed =false;
            foreach (KillMessage message in messagesToAdd)
            {
                if (message == null)
                {
                    continue;
                }
                GameObject clone = Instantiate(Panel, KillFeed.transform);
                clone.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = message.from;
                clone.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "<color=red>" + message.to;
            }
            messagesToAdd.Clear();
        }
        
    }

   
}
