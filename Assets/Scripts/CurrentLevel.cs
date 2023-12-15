using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentLevel : MonoBehaviour, IDataPersistence
{
    public static CurrentLevel Instance
    {get; private set;}
    public Environment Environment;
    public int numDungeons = 1;
    public int currentDungeon = 0;
    public float playerDamageDealt = 0f;
    public float playerDamageTaken = 0f;
    public int enemiesKilled = 0;
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
            GameDataManager.Instance.SaveGame();
            SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            DontDestroyOnLoad(player);
        }
    }

    public void LoadData(GameData data)
    {
        numDungeons = data.numberOfDungeons;
        currentDungeon = data.dungeonCount;
        playerDamageDealt = data.damageDealt;
        playerDamageTaken = data.damageTaken;
        enemiesKilled = data.enemiesKilled;
        Level.SetLevel(data.dungeonCount);
    }

    public void SaveData(ref GameData data)
    {
        data.numberOfDungeons = numDungeons;
        data.dungeonCount = currentDungeon;
        data.damageDealt = playerDamageDealt;
        data.damageTaken = playerDamageTaken;
        data.enemiesKilled = enemiesKilled;
    }
}
