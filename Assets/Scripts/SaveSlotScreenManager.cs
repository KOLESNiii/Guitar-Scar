using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotScreenManager : MonoBehaviour
{
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
    int selectedSaveSlot = -1;
    void Start()
    {
        saveSlots = GameDataManager.Instance.getSaveSlots(true);
    }
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

    public void Select()
    {
        if (selectedSaveSlot == -1)
        {
            return;
        }
        GameDataManager.Instance.setSaveSlot(selectedSaveSlot, true);
        GameObject player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(player);
        GameDataManager.Instance.LoadGame();
        SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
    }

    public void UpdateText()
    {
        if (selectedSaveSlot == -1)
        {
            EmptySaveSlotMessage.GetComponent<TMPro.TextMeshProUGUI>().text = "Select a Save Slot";
            DamageDealtText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            DamageTakenText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            EnemiesKilledText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            PlayerLevelText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            DungeonsClearedText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
        }
        else if (saveSlots[selectedSaveSlot] == null)
        {
            EmptySaveSlotMessage.GetComponent<TMPro.TextMeshProUGUI>().text = "Empty Save Slot";
            DamageDealtText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            DamageTakenText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            EnemiesKilledText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            PlayerLevelText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
            DungeonsClearedText.GetComponent<TMPro.TextMeshProUGUI>().text = "-";
        }
        else
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
