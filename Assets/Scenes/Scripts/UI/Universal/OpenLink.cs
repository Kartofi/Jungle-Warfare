using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLink : MonoBehaviour
{

    public void OpenUrl()
    {
        Application.OpenURL(NetworkingCredentials.createAccUrl);
    }
}
