using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesSet : MonoBehaviour
{
    public static RulesSet main;

    public LobbyRules lobbyRules;
    // Start is called before the first frame update
    void Awake()
    {
        main = this;
        lobbyRules = new LobbyRules();
    }

    public void Save()
    {
        try
        {
            string json = JsonParser.ToJson(lobbyRules);
            PlayerPrefs.SetString("lobbyRules", json);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
