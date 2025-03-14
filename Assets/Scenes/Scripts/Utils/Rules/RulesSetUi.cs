using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.ComponentModel;
using System.Reflection;

public class RulesSetUi : MonoBehaviour
{
    [Header("Windows")]
    public GameObject mainLobbyWindow;
    public GameObject lobbySettingsWindow;
    [Header("Instances")]
    public Slider slider;
    public TMP_InputField counter;

    public string property;
    public bool switchWindows = false;

    private static List<RulesSetUi> resetInputs = new(); 
    // Start is called before the first frame update

    private void Start()
    {
        if (switchWindows == true)
        {
            return;
        }
        if (property != null && property.Length > 1)
        {
            resetInputs.Add(this);
        }
        else
        {
            RulesSet.main.lobbyRules = new LobbyRules();
        }
        if (resetInputs.Count >= RulesSet.main.lobbyRules.GetType().GetFields().Length)
        {
            ResetRules();
        }
    }
    public void SwitchLobbiesWindows()
    {
        mainLobbyWindow.SetActive(!mainLobbyWindow.activeSelf);
        lobbySettingsWindow.SetActive(!lobbySettingsWindow.activeSelf);
    }
    public void ResetRules()
    {
        RulesSet.main.lobbyRules = new LobbyRules();
       
        foreach(RulesSetUi element in resetInputs)
        {
            FieldInfo field = RulesSet.main.lobbyRules.GetType().GetField(element.property);
            object value = field.GetValue(RulesSet.main.lobbyRules);
            if (element.slider.wholeNumbers == true)
            {
                if (element.property == "respawnTime")
                {
                    element.slider.value = ((int)value / 1000);
                }
                else
                {
                    element.slider.value = (int)value;
                }
            }
            else
            {
                element.slider.value = ((float)value);
            }
        }
    }
    public void HandeSliderChange(float value)
    {
        if(slider.wholeNumbers == true)
        {
            if (property == "respawnTime")
            {
                counter.text = (value).ToString();
                SetValue((int)value * 1000);
            }
            else
            {
                counter.text = value.ToString();
                SetValue((int)value);
            }
            
        }
        else
        {
            counter.text = ((float)value).ToString("0.00");
            SetValue((float)value);
        }
        
        RulesSet.main.Save();
    }
    public void HandleCounterChange(string value)
    {
        object propertyInstance = GetValue();

        float floatValue = 0;
        int intValue = 0;

        try
        {
            if (slider.wholeNumbers == true)
            {
                intValue = int.Parse(value);
            }
            else
            {
                floatValue = float.Parse(value);
            }
        }
        catch (Exception e)
        {
            counter.text = propertyInstance.ToString();
            Debug.Log(e.Message);
            return;
        }
        if(slider.wholeNumbers == true)
        {
            int resultValue = (int)Mathf.Clamp(intValue, slider.minValue, slider.maxValue);
            
            slider.value = resultValue;
            counter.text = resultValue.ToString();
            if (property == "respawnTime")
            {
                SetValue(resultValue * 1000);
            }
            else
            {
                SetValue(resultValue);
            }
        }
        else
        {
            float resultValue = (float)Mathf.Clamp(floatValue, slider.minValue, slider.maxValue);

            slider.value = resultValue;
            SetValue(resultValue);

            counter.text = resultValue.ToString("0.00");
        }
       
        RulesSet.main.Save();
    }
    public object GetValue()
    {
        var fieldInfo = RulesSet.main.lobbyRules.GetType().GetField(property);
        if (fieldInfo.IsStatic)
            return fieldInfo.GetValue(null);
        else
            return fieldInfo.GetValue(RulesSet.main.lobbyRules);
    }
    public void SetValue(object value)
    {
        var fieldInfo = RulesSet.main.lobbyRules.GetType().GetField(property);
        if (fieldInfo.IsStatic)
            fieldInfo.SetValue(null, value);
        else
            fieldInfo.SetValue(RulesSet.main.lobbyRules, value);
    }
}
