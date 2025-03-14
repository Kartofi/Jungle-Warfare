using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;

public class UiSettingsSet : MonoBehaviour
{
    public Resolution[] commonResolutions;

    public static UiSettingsSet main;

    private void Awake()
    {
        main = this;
        commonResolutions = Screen.resolutions;

       for(int i = 0; i < commonResolutions.Length; i++)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData() { text = commonResolutions[i].width + "x" + commonResolutions[i].height });
        }
    }
    public GameObject settings;

    [Header("Screen")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullScreen;
    public TMP_Dropdown qualityDropdown;
    public Toggle vsyncTogle;
    [Header("FPS")]
    public Slider fpsSlider;
    public TMP_InputField fpsCounter;
    [Header("Mouse Sensitivity")]
    public Slider mouseSensSlider;
    public TMP_Text mouseSensCounter;
    [Header("FOV")]
    public Slider fovSlider;
    public TMP_InputField fovCounter;
    [Header("Master")]
    public Slider masterAudio;
    public TMP_Text masterAudioCounter;
    [Header("Music")]
    public Slider musicAudio;
    public TMP_Text musicCounter;

    public void ToggleSettings()
    {
        settings.SetActive(!settings.activeSelf);
    }

    public void OnResolutionChange(int value)
    {
        SettingsSet.main.settings.resolution = value;
        SettingsSet.main.Apply();
    }
    bool firstReadFullScreenMode = false;
    public void OnFullScreenModeChange(int value)
    {
        if (firstReadFullScreenMode == false)
        {
            firstReadFullScreenMode = true;
            return;
        }
        SettingsSet.main.settings.screenMode = value;
        SettingsSet.main.Apply();
    }
    public void OnFovChange(float value)
    {
        SettingsSet.main.settings.fov = (int)value;
        fovCounter.text = value.ToString();
        SettingsSet.main.Apply();
    }
    public void OnFovChange(string value)
    {
        int result = 0;
        try
        {
            result = int.Parse(value);
        }
        catch (Exception e)
        {
            fovCounter.text = SettingsSet.main.settings.fov.ToString();
            Debug.Log(e.Message);
            return;
        }
        result = (int)Mathf.Clamp(result, fovSlider.minValue, fovSlider.maxValue);
        fovCounter.text = result.ToString();
        fovSlider.value = result;
        SettingsSet.main.settings.fov = result;
        SettingsSet.main.Apply();
    }
    public void OnFpsChange(float value)
    {
        SettingsSet.main.settings.fps = (int)value;
        fpsCounter.text = value.ToString();
        SettingsSet.main.Apply();
    }
    public void OnFpsChange(string value)
    {
        int result = 0;
        try
        {
            result = int.Parse(value);
        }catch(Exception e)
        {
            fpsCounter.text = SettingsSet.main.settings.fps.ToString();
            Debug.Log(e.Message);
            return;
        }
        result = (int)Mathf.Clamp(result, fpsSlider.minValue, fpsSlider.maxValue);
        fpsCounter.text = result.ToString();
        fpsSlider.value = result;
        SettingsSet.main.settings.fps = result;
        SettingsSet.main.Apply();
    }
    public void OnMouseSensChange(float value)
    {
        SettingsSet.main.settings.mouseSensitivity = value;
        mouseSensCounter.text = value.ToString("0.00");
        SettingsSet.main.Apply();
    }
    public void OnMusicAudioChange(float value)
    {
        SettingsSet.main.settings.musicVolume = (int)value;
        musicCounter.text = value.ToString() + "%";
        SettingsSet.main.Apply();
    }
    public void OnMasterAudioChange(float value)
    {
        SettingsSet.main.settings.masterVolume = (int)value;
        masterAudioCounter.text = value.ToString() + "%";
        SettingsSet.main.Apply();
    }
    public void OnQualityChange(int value)
    {
        SettingsSet.main.settings.quality = value;
        SettingsSet.main.Apply();
    }
    public void OnVSyncChange(bool value)
    {
        SettingsSet.main.settings.VSyncCount = value == true ? 1 : 0;
        SettingsSet.main.Apply();
    }
}
