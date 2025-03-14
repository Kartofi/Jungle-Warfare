using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[Serializable]
public class Lobbies
{
    public Lobby[] lobbies;
}
[Serializable]
public class Httpdata
{
    public string status;
    public string error;
    public string playerName;
    public long playerId;
    public string loginSessionId;
}
public class DataStartMenu : MonoBehaviour
{
    string urlLobby = NetworkingCredentials.rootUrl + "api/lobbies";

    string urlLogin = NetworkingCredentials.rootUrl + "api/login";

    string urlSessionLogin = NetworkingCredentials.rootUrl + "api/sessionLogin";

    string urlLogOut = NetworkingCredentials.rootUrl + "api/logout";

    public GameObject StartMenu;
    public GameObject LoginMenu;
    public new TMP_InputField name;
    public TMP_InputField password;


    public TMP_Text loggedInAs;
    public LoadImageFromUrl pfpPlayer;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ErrorUiStartMenu.main.NewError("Error. Please check your internet connection!", true);
                Debug.Log("Error. Check internet connection!");
                return;
            }
            StartLogin();
            UpdateLobby();
        }
    }
    public void Quit()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
    public async void Logout()
    {

        LoginHttp logoutHttp = new LoginHttp();
        logoutHttp.loginSessionId = PlayerPrefs.GetString("loginSessionId");
        
        try
        {
            logoutHttp.id = long.Parse(PlayerPrefs.GetString("playerId"));
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PlayerPrefs.DeleteKey("playerId");
            PlayerPrefs.DeleteKey("playerName");
            PlayerPrefs.DeleteKey("password");
            PlayerPrefs.DeleteKey("playerId");
            PlayerPrefs.Save();
            Application.Quit();
        }
        await Http.PostAsync(urlLogOut, logoutHttp, 10);

         PlayerPrefs.Save();
        StartMenu.SetActive(false);
        LoginMenu.SetActive(true);
    }
    public async void Login()
    {
        if (name.text.Length <= 0 || password.text.Length <= 0 )
        {
            ErrorUiStartMenu.main.NewError("Password and name length must be greater than 0");
            return;
        }
        try
        {
            LoginHttp loginHttp = new LoginHttp(
                name.text,
                password.text
            );

            Httpdata httpData = await Http.PostAsync(urlLogin, loginHttp, 10);

            if (httpData.status == "unSuccessful")
            {
                ErrorUiStartMenu.main.NewError(httpData.error);
            }
            else if (httpData.status == "Successful")
            {

                PlayerPrefs.SetString("playerName", name.text);
                PlayerPrefs.SetString("password", password.text);

                loggedInAs.text = "Logged in as:\n" + name.text;

                foreach (GameObject panel in ErrorUiStartMenu.main.panelsToDisable)
                {
                    panel.SetActive(false);
                }
                StartMenu.SetActive(true);
                pfpPlayer.UpdateImageUsingId(httpData.playerId);
               
                PlayerPrefs.SetString("playerId", httpData.playerId.ToString());
                PlayerPrefs.SetString("loginSessionId", httpData.loginSessionId);
                PlayerPrefs.Save();
                ErrorUiStartMenu.main.NewError("You logged in successfully " + PlayerPrefs.GetString("playerName"));
            }
            else if (httpData.status == "outdatedClient")
            {
                ErrorUiStartMenu.main.NewError(httpData.error, true);
            }
        }
        catch(Exception e)
        {
            Debug.Log("Cant connect to the server : " + e.Message);
            ErrorUiStartMenu.main.NewError("Cant connect to the server.", true);
        }
       
    }
    public async void StartLogin()
    {
        if (!PlayerPrefs.HasKey("playerId")|| !PlayerPrefs.HasKey("playerName") || !PlayerPrefs.HasKey("password") || !PlayerPrefs.HasKey("loginSessionId"))
        {
            StartMenu.SetActive(false);
            LoginMenu.SetActive(true);
            return;
        }
        try
        {

            string passwordSaved = PlayerPrefs.GetString("password");
            long playerId = -1;
            try
            {
                playerId = long.Parse(PlayerPrefs.GetString("playerId"));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                PlayerPrefs.DeleteAll();
                Application.Quit();
            }
            string playerName = PlayerPrefs.GetString("playerName");

            LoginHttp loginHttp = new LoginHttp();
            loginHttp.id = playerId;
            loginHttp.loginSessionId = PlayerPrefs.GetString("loginSessionId");

            Httpdata httpData = await Http.PostAsync(urlSessionLogin, loginHttp, 5);

            if (httpData.status == "unSuccessful")
            {
                StartMenu.SetActive(false);
                LoginMenu.SetActive(true);
                name.text = playerName;
                password.text = passwordSaved;
                PlayerPrefs.DeleteKey("loginSessionId");
                PlayerPrefs.DeleteKey("playerId");
                PlayerPrefs.Save();
                ErrorUiStartMenu.main.NewError(httpData.error);
            }
            else if (httpData.status == "Successful")
            {
                if (playerName != httpData.playerName)
                {
                    PlayerPrefs.SetString("playerName", httpData.playerName);
                }
                loggedInAs.text = "Logged in as:\n" + httpData.playerName;
                foreach (GameObject panel in ErrorUiStartMenu.main.panelsToDisable)
                {
                    panel.SetActive(false);
                }
                StartMenu.SetActive(true);
                pfpPlayer.UpdateImageUsingId(playerId);
                
            }
            else if (httpData.status == "outdatedClient")
            {
                ErrorUiStartMenu.main.NewError(httpData.error, true);
            }
        }
        catch(Exception e)
        {
           Debug.Log(e);
           StartMenu.SetActive(false);
           LoginMenu.SetActive(true);

           name.text = PlayerPrefs.GetString("playerName");
           password.text = PlayerPrefs.GetString("password");

           ErrorUiStartMenu.main.NewError("There was a problem logging you in.");
        }
        
    }
    public async void UpdateLobby()
    {
        if (CanvasManager.startMenu.lobbiesUi.activeSelf == false)
        {
            return;
        }
        HttpClient client = new HttpClient();
        string data = await client.GetStringAsync(urlLobby);
        if (data != null && data.Length > 2)
        {
            Lobbies lobbies = JsonParser.FromJson<Lobbies>(data);
            LobbyManage.main.Lobbies = lobbies.lobbies;
        }else
        {
            LobbyManage.main.Lobbies = new Lobby[0];
        }
    }
    int count = 0;
    private void FixedUpdate()
    {
        count++;
        if (count > 50)
        {
            UpdateLobby();
            count = 0;
        }
    }
}
