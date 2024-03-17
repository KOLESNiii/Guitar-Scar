using UnityEngine;

public class GameStartSettingsLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        (int, int, int, Resolution, int) settings = SettingsLoader.LoadSettings();
        SettingsLoader.ApplySettings(settings.Item1, settings.Item2, settings.Item4, settings.Item5);
        Destroy(this);
    }
}
