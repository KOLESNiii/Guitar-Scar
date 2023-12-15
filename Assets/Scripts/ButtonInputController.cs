using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonInputController : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    public void GameModeSelection()
    {
        SceneManager.LoadScene("EndlessVsLevelsSelection", LoadSceneMode.Single);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    public void LevelSelection()
    {
        SceneManager.LoadScene("LevelSelection", LoadSceneMode.Single);
    }
    public void EndlessMode()
    {
        SceneManager.LoadScene("EndlessMode", LoadSceneMode.Single);
    }
    public void TrainingMode()
    {
        SceneManager.LoadScene("TrainingMode", LoadSceneMode.Single);
    }
    public void Settings()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Single);
    }
    public void Quit()
    {
        Debug.Log("Exiting...");
        Application.Quit();
    }
    public void ReloadLatestSave()
    {
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(player);
        GameDataManager.Instance.LoadGame();
        SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
    }
}
