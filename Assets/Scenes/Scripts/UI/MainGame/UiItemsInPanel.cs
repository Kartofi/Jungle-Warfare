using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Message
{
    public long fromId;
    public long toId;

    public string message;
    public Message(long fromId, string message)
    {
        this.fromId = fromId;
        this.message = message;
    }
    public Message(long fromId, long toId)
    {
        this.fromId = fromId;
        this.toId = toId;
    }

}
public enum ParentType
{
    KillFeed = 0,
    Chat = 1
}
public class UiItemsInPanel : MonoBehaviour
{
    public static UiItemsInPanel killFeed;
    public static UiItemsInPanel chat;
    public ParentType ParentType = new ParentType();
    private void Awake()
    {
        if (ParentType == 0)
        {
            killFeed = this;
        }else
        {
            chat = this;
        }
        
    }
    

    public GameObject Parent;
    public GameObject Panel;

    public Queue<Message> messagesToAdd = new Queue<Message>();
    bool updateNeed = false;

    public void NewMessage(long fromId,string message)
    {
        messagesToAdd.Enqueue(new Message(fromId, message));
        updateNeed = true;
    }
    public void NewMessage(long fromId, long toId)
    {
        messagesToAdd.Enqueue(new Message(fromId, toId));
        updateNeed = true;
    }
    private void Update()
    {
        if (updateNeed == true)
        {
            updateNeed = false;
            for(int i =0;i< messagesToAdd.Count; i++)
            {
                Message message = messagesToAdd.Dequeue();
                if (message == null)
                {
                    continue;
                }
                GameObject clone = Instantiate(Panel, Parent.transform);
                if (ParentType == ParentType.KillFeed)
                {
                    string fromName = "";
                    string toName = "";

                    if (message.fromId == Udp.client.id)
                    {
                        fromName = Udp.client.name;
                    }
                    if (message.toId == Udp.client.id)
                    {
                        toName = Udp.client.name;
                    }
                    if (Udp.client.idsObjs.ContainsKey(message.fromId))
                    {
                        fromName = Udp.client.idsObjs.GetValueOrDefault(message.fromId).name;
                    }
                    if (Udp.client.idsObjs.ContainsKey(message.toId))
                    {
                        toName = Udp.client.idsObjs.GetValueOrDefault(message.toId).name;
                    }

                    clone.gameObject.GetComponent<KillFeedElement>().UpdateData(fromName,toName,message.fromId, message.toId);
                }
                else
                {
                    string fromName = "";

                    if (message.fromId == Udp.client.id)
                    {
                        fromName = Udp.client.name;
                    }
                    if (Udp.client.idsObjs.ContainsKey(message.fromId))
                    {
                        fromName = Udp.client.idsObjs.GetValueOrDefault(message.fromId).name;
                    }
                    if(fromName == "")
                    {
                        fromName = "Server";
                    }
                    clone.GetComponent<TMP_Text>().text = fromName.Trim() + " : " + message.message.Trim();
                    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Parent.transform);
                }
            }
        }

    }
}
