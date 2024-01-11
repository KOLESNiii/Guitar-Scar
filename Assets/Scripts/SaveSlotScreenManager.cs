using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//Class for managing the save slot screen
public class SaveSlotScreenManager : MonoBehaviour
{
    //UI elements assigned in unity editor
    [SerializeField]
    GameObject DamageDealtText;
    [SerializeField]
    GameObject DamageTakenText;
    [SerializeField]
    GameObject EnemiesKilledText;
    [SerializeField]
    GameObject PlayerLevelText;
    [SerializeField]
    GameObject DungeonsClearedText;
    [SerializeField]
    GameObject EmptySaveSlotMessage;
    [SerializeField]
    GameObject PlayerPrefab;
    [SerializeField]
    GameObject SaveSlot1;
    [SerializeField]
    GameObject SaveSlot2;
    [SerializeField]
    GameObject SaveSlot3;
    GameData[] saveSlots = new GameData[3];
    int selectedSaveSlot = -1; //Selected save slot, -1 if none selected
    //Get the save slots from the game data manager
    void Start()
    {
        saveSlots = GameDataManager.Instance.getSaveSlots(true);
    }
    //Callback functions for save slot toggles, identical apart from selected save slot identity
    public void ToggleSaveSlot1(bool newValue)
    {
        if (SaveSlot1.GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            Debug.Log("Save Slot 0");
            selectedSaveSlot = 0;
            UpdateText();
        }
    }
    public void ToggleSaveSlot2(bool newValue)
    {
        if (SaveSlot2.GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            Debug.Log("Save Slot 1");
            selectedSaveSlot = 1;
            UpdateText();
        }
    }
    public void ToggleSaveSlot3(bool newValue)
    {
        if (SaveSlot3.GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            Debug.Log("Save Slot 2");
            selectedSaveSlot = 2;
            UpdateText();
        }
    }
    //Procedure for loading a save slot
    public void Select()
    {
        if (selectedSaveSlot == -1) //Validation check
        {
            return;
        }
        GameDataManager.Instance.setSaveSlot(selectedSaveSlot, true); //Set the save slot in the game data manager
        GameObject player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity); //Instantiate the player
        DontDestroyOnLoad(player); //Don't destroy the player when loading a new scene
        GameDataManager.Instance.LoadGame(); //Load the game data
        SceneManager.LoadScene("Dungeon", LoadSceneMode.Single); //Load the dungeon scene
    }

    //Procedure for starting a new game, nearly identical to Select()
    public void NewGame()
    {
        if (selectedSaveSlot == -1)
        {
            return;
        }
        GameDataManager.Instance.setSaveSlot(selectedSaveSlot, true);
        GameDataManager.Instance.NewGame(); //Get an empty gameData object
        GameDataManager.Instance.OverwriteSaveSlot();  //Overwrite the save slot with the empty gameData object
        GameObject player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(player);
        GameDataManager.Instance.LoadGame();
        SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
    }

    //Updates the text on the save slot screen, giving information about the selected save slot
    public void UpdateText()  
    {
        if (selectedSaveSlot == -1) //If no save slot is selected
        {
            EmptySaveSlotMessage.GetComponent<TMPro.TextMeshProUGUI>().text = "Select a Save Slot";
            DamageDealtText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            DamageTakenText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            EnemiesKilledText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            PlayerLevelText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            DungeonsClearedText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
        }
        else if (saveSlots[selectedSaveSlot] == null) //If the selected save slot is empty
        {
            EmptySaveSlotMessage.GetComponent<TMPro.TextMeshProUGUI>().text = "Empty Save Slot";
            DamageDealtText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            DamageTakenText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            EnemiesKilledText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            PlayerLevelText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            DungeonsClearedText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
        }
        else //If the selected save slot is not empty
        {
            EmptySaveSlotMessage.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            DamageDealtText.GetComponent<TMPro.TextMeshProUGUI>().text = saveSlots[selectedSaveSlot].damageDealt.ToString();
            DamageTakenText.GetComponent<TMPro.TextMeshProUGUI>().text = saveSlots[selectedSaveSlot].damageTaken.ToString();
            EnemiesKilledText.GetComponent<TMPro.TextMeshProUGUI>().text = saveSlots[selectedSaveSlot].enemiesKilled.ToString();
            PlayerLevelText.GetComponent<TMPro.TextMeshProUGUI>().text = saveSlots[selectedSaveSlot].playerLevel.ToString();
            DungeonsClearedText.GetComponent<TMPro.TextMeshProUGUI>().text = saveSlots[selectedSaveSlot].dungeonCount.ToString();
        }
    }
}
