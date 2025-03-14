using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager startMenu;
    private void Awake()
    {
        startMenu = this;
    }

    public GameObject lobbiesUi;
    public GameObject startMenuUi;
    public GameObject loadingUi;

    public void ToggleLobbiesUI()
    {
        lobbiesUi.SetActive(!lobbiesUi.activeSelf);
        startMenuUi.SetActive(!startMenuUi.activeSelf);
    }
    public void TogleLoadingUI()
    {
        lobbiesUi.SetActive(!lobbiesUi.activeSelf);
        loadingUi.SetActive(!loadingUi.activeSelf);
    }
}
