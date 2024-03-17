using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Singleton class that stores information about the current dungeon level, quasi-static
public class CurrentLevel : MonoBehaviour, IDataPersistence
{
    public static CurrentLevel Instance //singleton
    {get; private set;}
    public Environment Environment;
    public int numDungeons = 1; //number of dungeons in the current level
    public int currentDungeon = 0; //current dungeon in the level
    public float playerDamageDealt = 0f;
    public float playerDamageTaken = 0f; 
    public int enemiesKilled = 0;
    public int timeTaken = 0;
    public DateTime startTime;
    //Logic for singleton, to ensure that the class is quasi-static
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("CurrentLevel already exists in scene!");
        }
    }
    public void SetEnvironment(Environment environment)
    {
        Environment = environment;
    }

    //Loads next dungeon in level, exits level if last dungeon
    public void NextDungeon()
    {
        currentDungeon++;
        if (currentDungeon == numDungeons)
        {
            Global.ExitLevel();
            currentDungeon = 0;
        }
        else
        {
            Level.NextLevel();
            GameDataManager.Instance.SaveGame(); //saves game
            SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            DontDestroyOnLoad(player); //prevents player from being destroyed on scene load
        }
    }

    public int CalculateScore()
    {
        float damageRatio = playerDamageDealt / playerDamageTaken;
        double levelNumberScaled = Math.Pow(currentDungeon, 1.5f);
        double timeTakenScaled = Math.Sqrt(timeTaken);
        return (int)(1000 * damageRatio * levelNumberScaled * timeTakenScaled);
    }

    //loads save file data
    public void LoadData(GameData data)
    {
        numDungeons = data.numberOfDungeons;
        currentDungeon = data.dungeonCount;
        playerDamageDealt = data.damageDealt;
        playerDamageTaken = data.damageTaken;
        enemiesKilled = data.enemiesKilled;
        timeTaken = data.timeTaken;
        Level.SetLevel(data.dungeonCount);
        startTime = DateTime.Now;
    }

    //saves data to save file
    public void SaveData(ref GameData data)
    {
        timeTaken += DateTime.Now.Subtract(startTime).Seconds;
        startTime = DateTime.Now;
        int score = CalculateScore();
        Global.UpdateHighScore(score);
        data.timeTaken = timeTaken;
        data.numberOfDungeons = numDungeons;
        data.dungeonCount = currentDungeon;
        data.damageDealt = playerDamageDealt;
        data.damageTaken = playerDamageTaken;
        data.enemiesKilled = enemiesKilled;
        data.highScore = score;
    }
}