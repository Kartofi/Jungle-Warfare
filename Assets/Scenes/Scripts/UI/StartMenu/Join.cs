using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Text;

public class Join : MonoBehaviour
{
    bool checkingLobbyName = false;
    public async void CreateLobby(TMP_InputField text)
    {
        if (LobbyManage.main.isJoining == true || checkingLobbyName == true)
        {
            return;
        }
        if (CheckIfExist(text.text) == true)
        {
            Debug.Log("Lobby already exists.");
            ErrorUiStartMenu.main.NewError("Lobby already exists.");
            return;
        }else if (text.text.Length <= 0 || text.text .Length > 20)
        {
            Debug.Log("Lobby name must be between 1-20 characters.");
            ErrorUiStartMenu.main.NewError("Lobby name must be between 1-20 characters.");
            return;
        }
        checkingLobbyName = true;
        string loginSessionId = PlayerPrefs.GetString("loginSessionId");
        long playerId = long.Parse(PlayerPrefs.GetString("playerId"));

        Httpdata httpData = await Http.GetAsync(NetworkingCredentials.rootUrl + "moderation/lobbyName/" + playerId + "/" + loginSessionId + "/" + text.text);
        checkingLobbyName = false;
        if (httpData.status == "unSuccessful") {
            ErrorUiStartMenu.main.NewError("Lobby name can't contain bad words.");
            return;
        }
        LobbyManage.main.JoinLobby(new Lobby(text.text, 0, 1));
    }
    public bool CheckIfExist(string match)
    {
        bool found = false;
        int length = LobbyManage.main.Lobbies.Length;
        for(int i =0;i< length; i++)
        {
           if(match == LobbyManage.main.Lobbies[i].name)
            {
                found = true;
                break;
            }
            
        }
        return found;
    }
}
