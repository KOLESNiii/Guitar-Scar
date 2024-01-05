using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//SettingsManager class, used to manage settings
public class SettingsManager : MonoBehaviour
{
    //UI elements assigned in unity editor
    [SerializeField]
    private GameObject resolutionSlider;
    [SerializeField]
    private GameObject[] resolutionText;
    [SerializeField]
    private GameObject fullscreenSlider;
    [SerializeField]
    private GameObject[] fullscreenText;
    [SerializeField]
    private GameObject volumeSlider;
    [SerializeField]
    private GameObject[] volumeText;
    private Resolution[] resolutions;
    private FullScreenMode[] fullScreenModes = new FullScreenMode[]{FullScreenMode.Windowed, FullScreenMode.FullScreenWindow, FullScreenMode.ExclusiveFullScreen};
    private string[] FullScreenModesStrings = new string[]{"Windowed", "Borderless", "Fullscreen"};
    private int currentFullscreenModeIndex = 0; //0 = windowed, 1 = borderless, 2 = fullscreen
    private int currentVolume = 100;
    private int resolutionIndex = 0; //index of current resolution in resolutions array
    private int originalResolutionIndex; //used for reset logic
    private int originalVolume;
    private int originalFullscreenModeIndex;
    private Resolution currentResolution;
    private FullScreenMode currentFullscreenMode;
    //Initializes settings to default settings or saved settings
    void Start()
    {
        resolutions = Screen.resolutions; //gets all resolutions supported by monitor
        resolutionSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = resolutions.Length - 1; //set max value of resolution slider to number of resolutions
        if (PlayerPrefs.HasKey("fullscreenmode")) //if there is a saved fullscreen mode
        {
            currentFullscreenModeIndex = PlayerPrefs.GetInt("fullscreenmode");
        }
        else //if there is no saved fullscreen mode, default to fullscreen
        {
            currentFullscreenModeIndex = 2;
        }
        originalFullscreenModeIndex = currentFullscreenModeIndex;
        fullscreenSlider.GetComponent<UnityEngine.UI.Slider>().value = currentFullscreenModeIndex;
        currentFullscreenMode = fullScreenModes[currentFullscreenModeIndex];
        if (PlayerPrefs.HasKey("volume")) //if there is a saved volume
        {
            currentVolume = PlayerPrefs.GetInt("volume");
        }
        else //if there is no saved volume, default to 100
        {
            currentVolume = 100;
        }
        volumeSlider.GetComponent<UnityEngine.UI.Slider>().value = currentVolume;
        originalVolume = currentVolume;
        if (PlayerPrefs.HasKey("resolution")) //if there is a saved resolution
        {
            string res = PlayerPrefs.GetString("resolution");
            if (resolutions.Select(x => x.ToString()).Contains(res)) //if saved resolution is supported by monitor
            {
                resolutionIndex = Array.IndexOf(resolutions.Select(x => x.ToString()).ToArray(), res);
                currentResolution = resolutions[resolutionIndex];
            }
            else //if saved resolution is not supported by monitor, default to current resolution
            {
                currentResolution = Screen.currentResolution;
                resolutionIndex = Array.IndexOf(resolutions, Screen.currentResolution);
            }
        }
        else //if there is no saved resolution, default to current resolution, which is typically the highest supported resolution
        {
            currentResolution = Screen.currentResolution;
            resolutionIndex = Array.IndexOf(resolutions, Screen.currentResolution);
        }
        resolutionSlider.GetComponent<UnityEngine.UI.Slider>().value = resolutionIndex;
        originalResolutionIndex = resolutionIndex;
    }

    //Resets settings to values before changes were made
    public void Reset()
    {
        currentFullscreenModeIndex = originalFullscreenModeIndex;
        currentVolume = originalVolume;
        resolutionIndex = originalResolutionIndex;
        currentFullscreenMode = fullScreenModes[currentFullscreenModeIndex];
        currentResolution = resolutions[resolutionIndex];
        fullscreenSlider.GetComponent<UnityEngine.UI.Slider>().value = currentFullscreenModeIndex;
        volumeSlider.GetComponent<UnityEngine.UI.Slider>().value = currentVolume;
        resolutionSlider.GetComponent<UnityEngine.UI.Slider>().value = resolutionIndex;
    }

    //Applies settings to game, saves settings to player prefs
    public void Apply()
    {
        PlayerPrefs.SetInt("fullscreenmode", currentFullscreenModeIndex);
        PlayerPrefs.SetInt("volume", currentVolume);
        PlayerPrefs.SetString("resolution", currentResolution.ToString());
        Screen.SetResolution(currentResolution.width, currentResolution.height, currentFullscreenMode);
        AudioListener.volume = currentVolume / 100f;
        originalFullscreenModeIndex = currentFullscreenModeIndex;
        originalVolume = currentVolume;
        originalResolutionIndex = resolutionIndex;
    }

    //Returns to menu
    public void Back()
    {
        Reset();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    //The following are all callbacks for the sliders, which update the values and text of the sliders
    public void ChangeFullScreenValue(System.Single value)
    {
        currentFullscreenModeIndex = (int)fullscreenSlider.GetComponent<UnityEngine.UI.Slider>().value;
        currentFullscreenMode = fullScreenModes[currentFullscreenModeIndex];
        foreach (GameObject text in fullscreenText)
        {
            text.GetComponent<TextMeshProUGUI>().text = FullScreenModesStrings[currentFullscreenModeIndex];
        }
    }
    public void ChangeResolutionValue(System.Single value)
    {
        resolutionIndex = (int)resolutionSlider.GetComponent<UnityEngine.UI.Slider>().value;
        currentResolution = resolutions[resolutionIndex];
        foreach (GameObject text in resolutionText)
        {
            text.GetComponent<TextMeshProUGUI>().text = currentResolution.ToString();
        }
    }

    public void ChangeVolumeValue(System.Single value)
    {
        currentVolume = (int)volumeSlider.GetComponent<UnityEngine.UI.Slider>().value;
        foreach (GameObject text in volumeText)
        {
            text.GetComponent<TextMeshProUGUI>().text = currentVolume.ToString() + "%";
        }
    }
}
