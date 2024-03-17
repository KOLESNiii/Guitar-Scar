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
    [SerializeField]
    private GameObject colourblindModeSlider;
    [SerializeField]
    private GameObject[] colourblindModeText;
    private Resolution[] AllowedResolutions;
    private string[] FullScreenModesStrings = new string[]{"Windowed", "Borderless", "Fullscreen"};
    private string[] ColourblindModeStrings = new string[]{"Normal Vision", "Protanopia & Deuteranopia", "Tritanopia"};
    private int fullscreenModeIndex = 0; //0 = windowed, 1 = borderless, 2 = fullscreen
    private int volume = 100;
    private int colourblindMode = 0; //0 = normal vision, 1 = protanopia & deuteranopia, 2 = tritanopia
    private int resolutionIndex = 0; //index of current resolution in resolutions array
    private Resolution resolution;
    //Initializes settings to default settings or saved settings
    void Start()
    {
        AllowedResolutions = Screen.resolutions; //gets all resolutions supported by monitor
        resolutionSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = AllowedResolutions.Length - 1; //set max value of resolution slider to number of resolutions
        (fullscreenModeIndex, volume, resolutionIndex, resolution, colourblindMode) = SettingsLoader.LoadSettings();
        UpdateSliderValues();
    }

    //Resets settings to values before changes were made
    public void Reset()
    {
        //Reloads settings back to last applied state
        MenuMusicManager.instance.PlayExit();
        (fullscreenModeIndex, volume, resolutionIndex, resolution, colourblindMode) = SettingsLoader.LoadSettings();
        UpdateSliderValues();
    }

    private void UpdateSliderValues()
    //Sets the slider values to the currently selected settings
    {
        fullscreenSlider.GetComponent<UnityEngine.UI.Slider>().value = fullscreenModeIndex;
        volumeSlider.GetComponent<UnityEngine.UI.Slider>().value = volume;
        resolutionSlider.GetComponent<UnityEngine.UI.Slider>().value = resolutionIndex;
        colourblindModeSlider.GetComponent<UnityEngine.UI.Slider>().value = colourblindMode;
    }

    //Applies settings to game, saves settings to player prefs
    public void Apply()
    {
        MenuMusicManager.instance.PlayAccept();
        SettingsLoader.SaveSettings(fullscreenModeIndex, volume, resolution, colourblindMode);
        SettingsLoader.ApplySettings(fullscreenModeIndex, volume, resolution, colourblindMode);
    }

    //Returns to menu
    public void Back()
    {
        Reset();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    //The following are all callbacks for the sliders, which update the values of the settings and text of the sliders
    public void ChangeFullScreenValue(System.Single value)
    {
        fullscreenModeIndex = (int)fullscreenSlider.GetComponent<UnityEngine.UI.Slider>().value;
        foreach (GameObject text in fullscreenText)
        {
            text.GetComponent<TextMeshProUGUI>().text = FullScreenModesStrings[fullscreenModeIndex];
        }
    }
    public void ChangeResolutionValue(System.Single value)
    {
        resolutionIndex = (int)resolutionSlider.GetComponent<UnityEngine.UI.Slider>().value;
        resolution = AllowedResolutions[resolutionIndex];
        foreach (GameObject text in resolutionText)
        {
            text.GetComponent<TextMeshProUGUI>().text = resolution.ToString();
        }
    }

    public void ChangeVolumeValue(System.Single value)
    {
        volume = (int)volumeSlider.GetComponent<UnityEngine.UI.Slider>().value;
        foreach (GameObject text in volumeText)
        {
            text.GetComponent<TextMeshProUGUI>().text = volume.ToString() + "%";
        }
    }

    public void ChangeColourblindValue(System.Single value)
    {
        colourblindMode = (int)colourblindModeSlider.GetComponent<UnityEngine.UI.Slider>().value;
        foreach (GameObject text in colourblindModeText)
        {
            text.GetComponent<TextMeshProUGUI>().text = ColourblindModeStrings[colourblindMode];
        }
    }
}