using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ErrorUi : MonoBehaviour
{
    public static ErrorUi main;
    public GameObject ErrorUiPanel;

    public GameObject[] panelsToDisable;

    string reason;
    bool needToShow = false;
    bool fatal = false;

    bool activeError = false;

    private void Awake()
    {
        main = this;
    }
    public void NewError(string reason, bool fatal)
    {
        if (activeError == true)
        {
            return;
        }
        activeError = true;
        this.reason = reason;
        needToShow = true;
        this.fatal = fatal;
        if (fatal == true)
        {
            Udp.client.ExitGame();
            Udp.client.CloseConnection();
            TCP.main.CloseConnection();
            DOTween.KillAll(true);
            Udp.client.enabled = false;
        }
    }
    public void GoBackToTitle()
    {
        SceneManager.LoadScene(0);
    }
    // Update is called once per frame
    private void Update()
    {
       if (needToShow == true)
        {
            needToShow = false;
            ErrorUiPanel.SetActive(true);
            ErrorUiPanel.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = reason;
            reason = null;
            if (fatal == true)
            {
                foreach(GameObject panel in panelsToDisable)
                {
                    panel.SetActive(false);
                }
                Player.instance.gameObject.SetActive(false);
            }
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
