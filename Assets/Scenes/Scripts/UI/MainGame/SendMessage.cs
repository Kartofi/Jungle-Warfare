using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SendMessage : MonoBehaviour
{
    public TMP_InputField inputText;
    public void SendText()
    {
       inputText.transform.DOLocalRotate(new Vector3(0,0,1), 0.1f);
       inputText.transform.DOLocalRotate(new Vector3(0, 0, -1), 0.1f).SetDelay(0.1f); ;
       inputText.transform.DOLocalRotate(new Vector3(0,0,0), 0.1f).SetDelay(0.2f);
         
       TCP.main.SendChatMessage(inputText.text);
       inputText.text = "";
    }
    public void OnDeselect()
    {
        Player.instance.canMove = !inputText.isFocused;
        Player.instance.canCameraRotate = true;
        Player.instance.canShoot = true;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            inputText.Select();
            Player.instance.canMove = false;
            Player.instance.canCameraRotate = false;
            Player.instance.canShoot = false;
        }
        if (Input.GetKeyDown(KeyCode.Return) && inputText.text != "")
        {
            SendText();
            inputText.DeactivateInputField(true);
            EventSystem.current.SetSelectedGameObject(null);
            Player.instance.canMove = true;
            Player.instance.canCameraRotate = true;
            Player.instance.canShoot = true;
        }
      
    }
}
