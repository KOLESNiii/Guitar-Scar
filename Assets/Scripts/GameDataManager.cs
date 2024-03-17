using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System;
using System.IO;

//Singleton class to manage the saving and loading of game data
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance
    {get; private set;}
    public string fileName;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects; //Objects that handle loading and saving logic
    private FileDataHandler fileDataHandler;
    int saveSlot = -1; //The save slot to save to, -1 means no save slot selected

    //Singleton logic
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
    //Gets the available save slots for a given level or endless mode
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
        for (int i = 0; i < 3; i++) //iterates through the 3 save slots
        {
            fileDataHandler.SetDataFilePath(i.ToString());
            output[i] = fileDataHandler.Load();
        }
        return output;
    }

    //Sets the save slot to save to and laod from
    public void setSaveSlot(int saveSlot, bool isEndless, int levelNumber = 0)
    {
        this.saveSlot = saveSlot;
        string folderName = "";
        if (isEndless & levelNumber != 0) //Validation, endless mode cannot have level numbers
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
        if (saveSlot < 0 || saveSlot > 2) //Validation, save slot must be between 0 and 2
        {
            Debug.LogError("Invalid save slot");
        }
        else
        {
            fileDataHandler = new FileDataHandler(Path.Combine(new string[]{Application.persistentDataPath, "Saves", folderName}), saveSlot.ToString());
        }
        
    }
    //Sets default values for the game data manager
    void Start()
    {
        this.dataPersistenceObjects = GetAllDataPersistenceObjects();
        fileDataHandler = new FileDataHandler(Path.Combine(Application.persistentDataPath, "Saves"), fileName);
    }

    //Gets default data
    public void NewGame()
    {
        gameData = new GameData();
    }

    //Loads data from the file
    public void LoadGame()
    {
        dataPersistenceObjects = GetAllDataPersistenceObjects();
        if (saveSlot == -1) //Validation, save slot must be selected
        {
            Debug.LogError("No save slot selected");
            return;
        }
        gameData = fileDataHandler.Load(); //Loads data from file
        if (gameData == null) //Validation, if no data is loaded due to no save file, reverts to default values
        {
            Debug.LogError("No game data to load, reverting to default values");
            NewGame();
        }
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects) //Loads data into all data persistence objects
        {
            dataPersistenceObject.LoadData(gameData);
        }
    }

    //Saves data to the file
    public void SaveGame()
    {
        dataPersistenceObjects = GetAllDataPersistenceObjects();
        if (saveSlot == -1) //Validation, save slot must be selected
        {
            Debug.LogError("No save slot selected");
            return;
        }
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects) //Saves data from all data persistence objects
        {
            dataPersistenceObject.SaveData(ref gameData);
        }
        fileDataHandler.Save(gameData); //Saves data to file
    }

    //Overwrites the save slot with the current data
    public void OverwriteSaveSlot()
    {
        fileDataHandler.Save(gameData);
    }

    public void setHighScore(int score)
    {
        GameData highScoreData = new GameData(); //Creates new game data object
        highScoreData.highScore = score;
        FileDataHandler HighscoreFileDataHandler = new FileDataHandler(Path.Combine(Application.persistentDataPath), "HighScore");
        HighscoreFileDataHandler.Save(highScoreData);
    }

    public int getHighScore()
    {
        FileDataHandler HighscoreFileDataHandler = new FileDataHandler(Path.Combine(Application.persistentDataPath), "HighScore");
        GameData highScoreData = HighscoreFileDataHandler.Load(); //Loads high score data
        try
        {
            int highScore = highScoreData.highScore;
            return highScore;
        }
        catch (NullReferenceException e) //Validation, if no high score data is found, returns 0
        {
            Debug.LogWarning("No high score data found");
            return 0;
        };
    }

    //Gets all data persistence objects in the scene
    private List<IDataPersistence> GetAllDataPersistenceObjects()
    {
        List<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
        return dataPersistenceObjects;
    }
}