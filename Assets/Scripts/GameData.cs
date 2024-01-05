using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for holding data about the game, to be saved and loaded
[System.Serializable]
public class GameData
{
    public float damageDealt = 0;
    public float damageTaken = 0;
    public int enemiesKilled = 0;
    public int playerLevel = 0;
    public float playerXP = 0;
    public float playerHealth = Global.startingHealth;
    public int dungeonCount = 0;
    public int numberOfDungeons = -1;
    public bool isEndless = true;
    public List<int> unlockedLevels = new List<int>(){0};

    public GameData(bool isEndless = true, int numberOfDungeons = -1)
    {
        this.isEndless = isEndless;
        this.numberOfDungeons = numberOfDungeons;
        if (!isEndless && numberOfDungeons <= 0) //Validation, if the game is not endless, the number of dungeons must be greater than 0
        {
            Debug.LogError("numberOfDungeons must be greater than 0");
        }
    }
}