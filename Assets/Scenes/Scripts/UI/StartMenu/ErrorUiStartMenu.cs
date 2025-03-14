using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorUiStartMenu : MonoBehaviour
{
    public GameObject errorUi;

    public GameObject[] panelsToDisable;

    private TMP_Text reason;
    private TMP_Text button;

    public static ErrorUiStartMenu main;

    public bool critical = false;

    public void CloseError()
    {
        if (critical == true)
        {
            Application.Quit();
        }
        else
        {
            errorUi.SetActive(false);
        }
        
    }
    public void NewError(string reason, bool critical = false)
    {
        this.critical = critical;
        if (critical == true)
        {
            foreach (GameObject panel in panelsToDisable)
            {
                panel.SetActive(false);
            }

            button.text = "Exit";
        }
        else
        {
            button.text = "Close";
        }
        errorUi.SetActive(true);
        this.reason.text = reason;
    }
    // Start is called before the first frame update
    void Awake()
    {
        reason = errorUi.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        button = errorUi.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TMP_Text>();
        main = this;
    }
}
