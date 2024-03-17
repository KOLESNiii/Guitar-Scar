using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Audio;

public static class SettingsLoader
{
    private static AudioMixer mixer = Resources.Load<AudioMixer>("Volume");
    private static FullScreenMode[] fullScreenModes = new FullScreenMode[]{FullScreenMode.Windowed, FullScreenMode.FullScreenWindow, FullScreenMode.ExclusiveFullScreen}; 
    public static (int, int, int, Resolution, int) LoadSettings()
    //Returns fullscreen mode index, volume, resolution index, resolution, colourblind mode
    {
        Resolution[] resolutions = Screen.resolutions; //gets all resolutions supported by monitor
        int fullscreenModeIndex;
        int volume;
        int resolutionIndex;
        Resolution resolution;
        int ColourblindMode;

        if (PlayerPrefs.HasKey("fullscreenmode")) //if there is a saved fullscreen mode
        {
            fullscreenModeIndex = PlayerPrefs.GetInt("fullscreenmode");
        }
        else //if there is no saved fullscreen mode, default to fullscreen
        {
            fullscreenModeIndex = 2;
        }
        if (PlayerPrefs.HasKey("volume")) //if there is a saved volume
        {
            volume = PlayerPrefs.GetInt("volume");
        }
        else //if there is no saved volume, default to 100
        {
            volume = 100;
        }
        if (PlayerPrefs.HasKey("resolution")) //if there is a saved resolution
        {
            string res = PlayerPrefs.GetString("resolution");
            if (resolutions.Select(x => x.ToString()).Contains(res)) //if saved resolution is supported by monitor
            {
                resolutionIndex = Array.IndexOf(resolutions.Select(x => x.ToString()).ToArray(), res);
                resolution = resolutions[resolutionIndex];
            }
            else //if saved resolution is not supported by monitor, default to current resolution
            {
                resolution = Screen.currentResolution;
                resolutionIndex = Array.IndexOf(resolutions, Screen.currentResolution);
            }
        }
        else //if there is no saved resolution, default to current resolution, which is typically the highest supported resolution
        {
            resolution = Screen.currentResolution;
            resolutionIndex = Array.IndexOf(resolutions, Screen.currentResolution);
        }
        if (PlayerPrefs.HasKey("colourblind"))
        {
            ColourblindMode = PlayerPrefs.GetInt("colourblind");
        }
        else
        {
            ColourblindMode = 0;
        }
        return (fullscreenModeIndex, volume, resolutionIndex, resolution, ColourblindMode);
    }

    public static void SaveSettings(int fullscreenModeIndex, int volume, Resolution resolution, int colourblindMode)
    //Saves settings to Unity player prefs (persistent data storage)
    {
        PlayerPrefs.SetInt("fullscreenmode", fullscreenModeIndex);
        PlayerPrefs.SetInt("volume", volume);
        PlayerPrefs.SetString("resolution", resolution.ToString());
        PlayerPrefs.SetInt("colourblind", colourblindMode);
    }

    public static void ApplySettings(int fullscreenModeIndex, int volume, Resolution resolution, int colourblindMode)
    //Applies the settings given
    {
        FullScreenMode screenMode = fullScreenModes[fullscreenModeIndex];
        Screen.SetResolution(resolution.width, resolution.height, screenMode);
        mixer.SetFloat("Vol", MenuMusicManager.GetDecibelValue(volume));
        Global.colourblindMode = colourblindMode;
    }
}
