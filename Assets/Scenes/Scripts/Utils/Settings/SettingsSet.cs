using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    //Video
    public int resolution = 0;
    public int screenMode = 0;
    public int fps = 60;
    //Quality
    public int quality = 2;
    public int VSyncCount = 0;
    //GamePlay
    public float mouseSensitivity = 1f;
    public int fov = 70;
    //Audio
    public int masterVolume = 75;
    public int musicVolume = 75;

    public Settings()
    {

    }
}
public enum Qualities
{
    Performance = 0,
    Balanced = 1,
    Ultra = 2
}
public class SettingsSet : MonoBehaviour
{
    public bool loadFromPrefs;
    public bool mainGame;


    public Settings settings = new Settings();

    public static SettingsSet main;

    private void Awake()
    {
        main = this;
        if (loadFromPrefs == true)
        {
            bool result = Load();
            if (result == false)
            {
                settings.resolution = Screen.resolutions.Length-1;
            }
        }

        if (Screen.resolutions.Length <= settings.resolution)
        {
            settings.resolution = Screen.resolutions.Length - 1;
        }
        Resolution res = Screen.resolutions[settings.resolution];

        QualitySettings.vSyncCount = settings.VSyncCount;
        Screen.SetResolution(res.width, res.height, (FullScreenMode)settings.screenMode);

        
        QualitySettings.SetQualityLevel(settings.quality);
        Application.targetFrameRate = settings.fps;

        AudioManager.main.SetVolume(settings.masterVolume);
        AudioManager.main.SetMusicVolume(settings.musicVolume);

        if (mainGame == true)
        {
            Player.instance.sensitivity = settings.mouseSensitivity;
            BasicCamera.main.StartFOV = settings.fov;
            Camera.main.fieldOfView = settings.fov;
        }
        LoadUi();
    }

    bool loaded = false;
    private void Update()
    {

        if (loaded == false)
        {
            loaded = true;
            Apply();
        }

    }
    public void Save()
    {
        try
        {
            string json = JsonParser.ToJson(settings);
            PlayerPrefs.SetString("Settings", json);
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public bool Load()
    {
        if (!PlayerPrefs.HasKey("Settings"))
        {
            return false;
        }
        try
        {
            string json = PlayerPrefs.GetString("Settings");
            Settings data = JsonUtility.FromJson<Settings>(json);
            settings = data;
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }
    public void LoadUi()
    {
        UiSettingsSet.main.fpsCounter.text = settings.fps.ToString();
        UiSettingsSet.main.fpsSlider.value = settings.fps;

        UiSettingsSet.main.fovCounter.text = settings.fov.ToString();
        UiSettingsSet.main.fovSlider.value = settings.fov;

        UiSettingsSet.main.mouseSensCounter.text = settings.mouseSensitivity.ToString("0.00");
        UiSettingsSet.main.mouseSensSlider.value = settings.mouseSensitivity;


        UiSettingsSet.main.qualityDropdown.value = settings.quality;

        UiSettingsSet.main.fullScreen.value = settings.screenMode;
       

        UiSettingsSet.main.vsyncTogle.isOn = settings.VSyncCount == 1;
      

        UiSettingsSet.main.resolutionDropdown.value = settings.resolution; 

        UiSettingsSet.main.masterAudio.value = settings.masterVolume;
        UiSettingsSet.main.masterAudioCounter.text = settings.masterVolume.ToString() + "%";

        UiSettingsSet.main.musicAudio.value = settings.musicVolume;
        UiSettingsSet.main.musicCounter.text = settings.musicVolume.ToString() + "%";

        if (mainGame == true)
        {
            Player.instance.sensitivity = settings.mouseSensitivity;
            BasicCamera.main.StartFOV = settings.fov;
            Camera.main.fieldOfView = settings.fov;
        }

    }
    public void Apply()
    {
        if (Screen.resolutions.Length <= settings.resolution)
        {
            settings.resolution = Screen.resolutions.Length - 1;
        }
        Resolution res = Screen.resolutions[settings.resolution];
        QualitySettings.vSyncCount = settings.VSyncCount;
        Screen.SetResolution(res.width, res.height, (FullScreenMode)settings.screenMode);

        QualitySettings.SetQualityLevel(settings.quality);

        Application.targetFrameRate = settings.fps;

        AudioManager.main.SetVolume(settings.masterVolume);
        AudioManager.main.SetMusicVolume(settings.musicVolume);

        if (mainGame == true)
        {
            Player.instance.sensitivity = settings.mouseSensitivity;
            BasicCamera.main.StartFOV = settings.fov;
            Camera.main.fieldOfView = settings.fov;
        }
        Save();
    }
}
