using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class PingDisplay : MonoBehaviour
{
    public static PingDisplay main;
    // Start is called before the first frame update
    public TMP_Text ping;
    public Gradient colorTransition;
    void Start()
    {
        main = this;
        UpdatePing();
    }
    bool updatePing = false;
    public void UpdatePing()
    {
        updatePing = true;
    }
    int pingValue = 999;
    int fps = 0;

    // Update is called once per frame
    float updateDisplayTime = 0;
    void Update()
    {
        if (updatePing == true || Udp.client.ping.ToString() != ping.text)
        {
            updatePing = false;
            int pingValue = Udp.client.ping;
            this.pingValue = pingValue;
        }
        updateDisplayTime += Time.deltaTime;
        if (updateDisplayTime >= 0.75f)
        {
            updateDisplayTime = 0;
            fps = (int)(1f / Time.deltaTime);
            string text = "<color=#" + ColorUtility.ToHtmlStringRGB(colorTransition.Evaluate(pingValue / 999f)) + ">" + pingValue + " ms</color>\n" + fps + " FPS";
            ping.text = text;
        }
        
    }
}
