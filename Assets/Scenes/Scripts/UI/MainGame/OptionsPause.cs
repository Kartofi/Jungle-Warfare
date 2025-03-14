using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class OptionsPause : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject LobbyId;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenu.activeSelf == true)
            {
                if (UiSettingsSet.main.settings.activeSelf == true)
                {
                    UiSettingsSet.main.ToggleSettings();
                }
                PauseMenu.SetActive(false);
                Player.instance.canCameraRotate = true;
                Player.instance.canShoot = true;
                Player.instance.canMove = true;
                Cursor.lockState = CursorLockMode.Locked;
            }else
            {
                LobbyId.GetComponent<TMP_Text>().text = "Lobby:" + Udp.client.lobbyId;
                PauseMenu.SetActive(true);
                Player.instance.canCameraRotate = false;
                Player.instance.canShoot = false;
                Player.instance.canMove = false;
                Cursor.lockState = CursorLockMode.None;
            }
            
        }
    }
    public void CloseMenu()
    {
        if (PauseMenu.activeSelf == true)
        {
            PauseMenu.SetActive(false);
            Player.instance.canCameraRotate = true;
            Player.instance.canShoot = true;
            Player.instance.canMove = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Quit()
    {
        if (PauseMenu.activeSelf == true)
        {
            
            TCP.main.CloseConnection();
            Udp.client.ExitGame();
            Udp.client.CloseConnection();
            Player.instance.gameObject.SetActive(false);
            DG.Tweening.DOTween.KillAll();
            SceneManager.LoadScene(0);
        }
    }
}
