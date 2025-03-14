using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PlayerPrefManager : MonoBehaviour
{
    public static PlayerPrefManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        if (PlayerPrefs.HasKey("playerId") && PlayerPrefs.HasKey("playerName") && PlayerPrefs.HasKey("password"))
        {
            string loginSessionId = PlayerPrefs.GetString("loginSessionId");
            gameObject.GetComponent<Udp>().loginSessionId = loginSessionId;

            long playerId = -1;
            try
            {
                playerId = long.Parse(PlayerPrefs.GetString("playerId"));
            }catch (Exception e)
            {
                Debug.Log(e.Message);
                PlayerPrefs.DeleteAll();
                Application.Quit();
            }
            gameObject.GetComponent<Udp>().id = playerId;
        }
        else
        {
            SceneManager.LoadScene(0);
        }
        if (PlayerPrefs.HasKey("lobbyId"))
        {
            string lobbyId = PlayerPrefs.GetString("lobbyId");
            gameObject.GetComponent<Udp>().lobbyId = lobbyId;
        }
        else
        {
            PlayerPrefs.SetString("lobbyId", "");
            gameObject.GetComponent<Udp>().lobbyId = "";
        }
        
        if (PlayerPrefs.HasKey("lobbyRules"))
        {
            LobbyRules lobbyRules = JsonParser.FromJson<LobbyRules>(PlayerPrefs.GetString("lobbyRules"));
            gameObject.GetComponent<Udp>().lobbyRules = lobbyRules;
        }
        else
        {
            gameObject.GetComponent<Udp>().lobbyRules = null;
        }
        PlayerPrefs.DeleteKey("lobbyRules");
        PlayerPrefs.Save();
    }
}
