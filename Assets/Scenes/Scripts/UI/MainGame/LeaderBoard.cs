using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public Gradient pingColor;

    public GameObject playerUi;
    public GameObject parentOfAll;
    public GameObject parent;

    public LoadImageFromUrl[] LeaderBoardCompact;

    public long[] topThree = new long[3] { 0,0,0 };

    private void Start()
    {
        InvokeRepeating("UpdatePanels", 0f, 0.1f);
    }
    void UpdatePanels()
    {
        PlayerData[] players = Udp.client.ServerData.players;
        players = players.OrderByDescending((element) => element.kills).ToArray();

        long[] playersTopThree = players.Take(3).Select(item => item.id).ToArray();
        
        if (topThree.Equals(playersTopThree) == false)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i > playersTopThree.Length - 1)
                {
                    LeaderBoardCompact[i].UpdateImageUsingId();
                    break;
                }
                try
                {
                    LeaderBoardCompact[i].UpdateImageUsingId(playersTopThree[i]);
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log(e.Message);
                    LeaderBoardCompact[i].UpdateImageUsingId();
                }

            }
            topThree = playersTopThree;
        }
        if (parentOfAll.activeSelf == false)
        {
            return;
        }
        UpdatePanel(players);
    }

    void UpdatePanel(PlayerData[] players = null)
    {
        if (players == null)
        {
            players = Udp.client.ServerData.players;
            players = players.OrderByDescending((element) => element.kills).ToArray();
        }
        string[] names = players.Select(player => player.name).ToArray();

        if (players.Length > 0)
        {
            for (int i = parent.transform.childCount - 1; i >= 0; i--)
            {
                GameObject obj = parent.transform.GetChild(i).gameObject;
                if (!names.Contains(obj.name))
                {
                    Destroy(obj);
                    continue;
                }
            }
            
            for (int i =0;i < players.Length;i++)
            {
                PlayerData player = players[i];


                Transform obj = parent.transform.Find(player.name);
                if (obj == null)
                {
                    GameObject clone = Instantiate(playerUi, parent.transform);
                    clone.name = player.name;

                    clone.transform.GetChild(0).GetComponent<TMP_Text>().text = player.name.Equals(Udp.client.name) ? "<color=green>" + player.name + " (YOU)" : player.name;
                    clone.transform.GetChild(1).GetComponent<TMP_Text>().text = player.kills.ToString();
                    clone.transform.GetChild(3).GetComponent<TMP_Text>().text = player.ping.ToString();
                    clone.transform.GetChild(3).GetComponent<TMP_Text>().color = pingColor.Evaluate(player.ping / 999f);
                    clone.transform.GetChild(4).GetComponent<Image>().color = pingColor.Evaluate(player.ping / 999f);

                    clone.transform.GetChild(5).GetComponent<LoadImageFromUrl>().UpdateImageUsingId(player.id);

                    if (player.id == Udp.client.ServerData.creatorId)
                    {
                        clone.transform.GetChild(6).GetComponent<Image>().enabled = true;
                    }
                    else
                    {
                        clone.transform.GetChild(6).GetComponent<Image>().enabled = false;
                    }
                    

                    clone.transform.SetSiblingIndex(i);
                }
                else
                {
                    obj.transform.GetChild(0).GetComponent<TMP_Text>().text = player.name.Equals(Udp.client.name) ? "<color=green>" + player.name + " (YOU)" : player.name;
                    obj.transform.GetChild(1).GetComponent<TMP_Text>().text = player.kills.ToString();
                    obj.transform.GetChild(3).GetComponent<TMP_Text>().text = player.ping.ToString();
                    obj.transform.GetChild(3).GetComponent<TMP_Text>().color = pingColor.Evaluate(player.ping / 999f);
                    obj.transform.GetChild(4).GetComponent<Image>().color = pingColor.Evaluate(player.ping / 999f);
                   

                    obj.SetSiblingIndex(i);
                }

            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && EventSystem.current.currentSelectedGameObject == null)
        {
            parentOfAll.SetActive(true);
            Player.instance.canCameraRotate = false;
            Player.instance.canShoot = false;
            Cursor.lockState = CursorLockMode.None;

            PlayerData[] players = Udp.client.ServerData.players;
            players = players.OrderByDescending((element) => element.kills).ToArray();

            UpdatePanel(players);

        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (parentOfAll.activeSelf == true)
            {
                parentOfAll.SetActive(false);
                Player.instance.canCameraRotate = true;
                Cursor.lockState = CursorLockMode.Locked;
                Player.instance.canShoot = true;
            }
        }
    }
}
