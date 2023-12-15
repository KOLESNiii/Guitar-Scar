using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
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
    private int currentFullscreenModeIndex = 0;
    private int currentVolume = 100;
    private int resolutionIndex = 0;
    private int originalResolutionIndex;
    private int originalVolume;
    private int originalFullscreenModeIndex;
    private Resolution currentResolution;
    private FullScreenMode currentFullscreenMode;
    // Start is called before the first frame updat
    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = resolutions.Length - 1;
        if (PlayerPrefs.HasKey("fullscreenmode"))
        {
            currentFullscreenModeIndex = PlayerPrefs.GetInt("fullscreenmode");
        }
        else
        {
            currentFullscreenModeIndex = 2;
        }
        originalFullscreenModeIndex = currentFullscreenModeIndex;
        fullscreenSlider.GetComponent<UnityEngine.UI.Slider>().value = currentFullscreenModeIndex;
        currentFullscreenMode = fullScreenModes[currentFullscreenModeIndex];
        if (PlayerPrefs.HasKey("volume"))
        {
            currentVolume = PlayerPrefs.GetInt("volume");
        }
        else
        {
            currentVolume = 100;
        }
        volumeSlider.GetComponent<UnityEngine.UI.Slider>().value = currentVolume;
        originalVolume = currentVolume;
        if (PlayerPrefs.HasKey("resolution"))
        {
            string res = PlayerPrefs.GetString("resolution");
            if (resolutions.Select(x => x.ToString()).Contains(res))
            {
                resolutionIndex = Array.IndexOf(resolutions.Select(x => x.ToString()).ToArray(), res);
                currentResolution = resolutions[resolutionIndex];
            }
            else
            {
                currentResolution = Screen.currentResolution;
                resolutionIndex = Array.IndexOf(resolutions, Screen.currentResolution);
            }
        }
        else
        {
            currentResolution = Screen.currentResolution;
            resolutionIndex = Array.IndexOf(resolutions, Screen.currentResolution);
        }
        resolutionSlider.GetComponent<UnityEngine.UI.Slider>().value = resolutionIndex;
        originalResolutionIndex = resolutionIndex;
    }

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

    public void Back()
    {
        Reset();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
