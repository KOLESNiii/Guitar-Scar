using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This class is used to control most buttons in most scenes
public class ButtonInputController : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    //Takes the player to game mode selection scene
    public void GameModeSelection()
    {
        SceneManager.LoadScene("EndlessVsLevelsSelection", LoadSceneMode.Single);
    }
    //Takes the player to the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    //Takes the player to the level selection scene
    public void LevelSelection()
    {
        SceneManager.LoadScene("LevelSelection", LoadSceneMode.Single);
    }
    //Takes the player to the endless mode save selection scene
    public void EndlessMode()
    {
        SceneManager.LoadScene("EndlessMode", LoadSceneMode.Single);
    }
    //Takes the player to the training mode scene
    public void TrainingMode()
    {
        SceneManager.LoadScene("TrainingMode", LoadSceneMode.Single);
    }
    //Takes the player to the settings scene
    public void Settings()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Single);
    }
    //Exits the program
    public void Quit()
    {
        Debug.Log("Exiting...");
        Application.Quit();
    }
    //Reloads the latest save file
    public void ReloadLatestSave()
    {
        GameObject playerToDestroy = GameObject.FindGameObjectWithTag("Player");
        if (playerToDestroy != null) //destroys player if player exists
        {
            Destroy(playerToDestroy);
        }
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity); //instantiates player
        DontDestroyOnLoad(player);
        GameDataManager.Instance.LoadGame(); //loads selected game save
        SceneManager.LoadScene("Dungeon", LoadSceneMode.Single); //loads dungeon scene
    }

    //Closes pause menu and resumes game
    public void ClosePauseMenu()
    {
        SceneManager.UnloadSceneAsync("PauseMenu");
        Global.Paused = false;
        Time.timeScale = 1;
    }
}
