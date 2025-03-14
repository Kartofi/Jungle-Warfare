using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

[Serializable]
public class Lobby
{
    public string name;

    public string creator;
    public int creatorId;

    public int players;
    public int lobbySize;
    public Lobby(string name, int players,int lobbySize)
    {
        this.name = name;
        this.players = players;
        this.lobbySize = lobbySize;
    }
}
public class LobbyManage : MonoBehaviour
{
    public static LobbyManage main;

    private Lobby[] lobbies;

    public Lobby[] Lobbies { 
        set { 
            lobbies = value;
            UpdatePanel();
        } 
        get {
            return lobbies;
        } 
    }

    public GameObject parent;
    public GameObject lobbyPrefab;

    public Image progressBar;
    public TMP_Text progressText;
    // Start is called before the first frame update
    private void Awake()
    {
        main = this;
    }


    public bool isJoining = false;

    public void JoinRandomLobby()
    {
        if (lobbies.Length <= 0 || lobbies.FirstOrDefault(lobby => lobby.lobbySize != lobby.players) == null)
        {
            ErrorUiStartMenu.main.NewError("There are no joinable lobbies, please create one.");
            return;
        }
        if (isJoining == true)
        {
            return;
        }
        isJoining = true;
        PlayerPrefs.SetString("lobbyId", "");
        CanvasManager.startMenu.TogleLoadingUI();
        StartCoroutine(LoadingScreen.LoadMainGame(progressBar, progressText));
    }
    public void JoinLobby(Lobby lobby)
    {
        if (lobby.players == lobby.lobbySize)
        {
            ErrorUiStartMenu.main.NewError("Lobby is full.");
            return;
        }
        if (isJoining == true)
        {
            return;
        }
        isJoining = true;
        PlayerPrefs.SetString("lobbyId", lobby.name);
        CanvasManager.startMenu.TogleLoadingUI();
        StartCoroutine(LoadingScreen.LoadMainGame(progressBar, progressText));
    }
    string filter = "";
    bool showFull = true;

    public void UpdateShowFull(bool value)
    {
        showFull = value;
        UpdatePanel();
    }

    public void ChangeFilterLobby(string value)
    {
        filter = value;
        UpdatePanel();
    }
    public void UpdatePanel()
    {
        if (parent == null)
        {
            return;
        }
        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
        foreach (Lobby lobby in this.lobbies)
        {
            if (lobby.name.Contains(filter) == false || (showFull == false && lobby.lobbySize <= lobby.players)) {
                continue;
            }
            GameObject lobbyobj = Instantiate(lobbyPrefab, parent.transform);
            lobbyobj.transform.GetChild(0).GetComponent<TMP_Text>().text = lobby.name;
            lobbyobj.transform.GetChild(1).GetComponent<TMP_Text>().text = lobby.players.ToString() + "/" + lobby.lobbySize.ToString();
            lobbyobj.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => JoinLobby(lobby));
            lobbyobj.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => UiButtonEffect.main.ButtonClickSound());
        }
    }
}
