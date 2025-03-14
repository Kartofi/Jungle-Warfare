using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChatMessage
{
    public string from;
    public string message;
    public ChatMessage(string from, string message)
    {
        this.from = from;
        this.message = message;
    }
}
public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject Chat;
    public GameObject Panel;

    public List<ChatMessage> messagesToAdd = new List<ChatMessage>();
    bool updateNeed = false;
    public void NewMessage(string from, string message)
    {
        messagesToAdd.Add(new ChatMessage(from, message));
        updateNeed = true;
    }
    private void Update()
    {
        if (updateNeed == true)
        {
            updateNeed = false;
            for (int i =0;i<messagesToAdd.Count;i++)
            {
                ChatMessage message = messagesToAdd[i];
                if (message == null)
                {
                    continue;
                }
                GameObject clone = Instantiate(Panel, Chat.transform);
                clone.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = message.from + " :";
                clone.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = message.message;
            }
            messagesToAdd.Clear();
        }

    }
}
