using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System;
using System.IO;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance
    {get; private set;}
    public string fileName;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler fileDataHandler;
    int saveSlot = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("GameDataManager already exists in scene!");
        }
    }
    public GameData[] getSaveSlots(bool isEndless,  int levelNumber = 0)
    {
        var output = new GameData[3];
        if (isEndless)
        {
            fileDataHandler = new FileDataHandler(Path.Combine(new string[]{Application.persistentDataPath, "Saves", "Endless"}), "0");
        }
        else
        {
            fileDataHandler = new FileDataHandler(Path.Combine(new string[]{Application.persistentDataPath, "Saves", "Level", levelNumber.ToString()}), "0");
        }
        for (int i = 0; i < 3; i++)
        {
            fileDataHandler.SetDataFilePath(i.ToString());
            output[i] = fileDataHandler.Load();
        }
        return output;
    }

    public void setSaveSlot(int saveSlot, bool isEndless, int levelNumber = 0)
    {
        this.saveSlot = saveSlot;
        string folderName = "";
        if (isEndless & levelNumber != 0)
        {
            Debug.LogError("Endless cannot have level numbers");

        }
        else if (isEndless)
        {
            folderName = "Endless";
        }
        else
        {
            folderName = Path.Combine("Level", levelNumber.ToString());
        }
        if (saveSlot < 0 || saveSlot > 2)
        {
            Debug.LogError("Invalid save slot");
        }
        else
        {
            fileDataHandler = new FileDataHandler(Path.Combine(new string[]{Application.persistentDataPath, "Saves", folderName}), saveSlot.ToString());
        }
        
    }
    void Start()
    {
        this.dataPersistenceObjects = GetAllDataPersistenceObjects();
        fileDataHandler = new FileDataHandler(Path.Combine(Application.persistentDataPath, "Saves"), fileName);
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        dataPersistenceObjects = GetAllDataPersistenceObjects();
        if (saveSlot == -1)
        {
            Debug.LogError("No save slot selected");
            return;
        }
        gameData = fileDataHandler.Load();
        if (gameData == null)
        {
            Debug.LogError("No game data to load, reverting to default values");
            NewGame();
        }
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        dataPersistenceObjects = GetAllDataPersistenceObjects();
        if (saveSlot == -1)
        {
            Debug.LogError("No save slot selected");
            return;
        }
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref gameData);
        }
        fileDataHandler.Save(gameData);
    }

    private List<IDataPersistence> GetAllDataPersistenceObjects()
    {
        List<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
        return dataPersistenceObjects;
    }

}
