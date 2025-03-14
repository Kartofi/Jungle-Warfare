using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
   public static IEnumerator LoadMainGame(Image progressBar = null, TMP_Text progressText = null)
    {
        AsyncOperation op =  SceneManager.LoadSceneAsync(1);

        while (!op.isDone)
        {
            if (progressBar != null)
            {
                progressBar.fillAmount = op.progress / 1f;
            }
            if (progressText != null)
            {
                progressText.text = (op.progress * 100).ToString("0.0") + "%";
            }
            if (op.progress >= 0.9f)
            {
                break;
            }
            yield return null;
        }
    }
}
