using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Global variables and functions
public static class Global
{
    //Base values for XP gain
    public static float BlockXPGain = 2f;
    public static float StrongXPGain = 2f;
    public static float NormalXPGain = 1f;
    //Base values for generating player stats
    public static float LevelUpMultiplier = 1.05f;
    public static float startingArmour = 100f;
    public static float startingHealth = 100f;
    public static float startingDamage = 5f;
    public static float startingXPToNextLevel = 100f;
    public static float PlayerViewRange = 10f;
    public static bool Paused = false;
    //Pauses game logic and animations, loads pause menu
    public static void Pause()
    {
        Paused = true;
        Time.timeScale = 0;
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
    }
    //Loads game over screen
    public static void GameOver()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
    }
    //Logic to exit level, not used as there is only endless mode
    public static void ExitLevel()
    {
        GameDataManager.Instance.SaveGame();
        Debug.Log("Level Complete");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

}

//Holds the data for the current dungeon level
public static class Level
{
    public static int levelNumber = 0;
    private static double difficultyMultiplier = 1.0;
    [SerializeField]
    private static double BaseBlockTime = 3.0;
    //Function to increase the current level by 1
    public static void NextLevel()
    {
        levelNumber++;
        difficultyMultiplier = Math.Pow(Math.E, (double)levelNumber / 15);
    }

    //Function to set the current level to a specific level
    public static void SetLevel(int level)
    {
        levelNumber = level;
        difficultyMultiplier = Math.Pow(Math.E, (double)levelNumber / 15);
    }

    //Returns the difficulty multiplier for the current level
    public static double GetDifficultyMultiplier()
    {
        return difficultyMultiplier;
    }

    //Returns the chance to block an attack by enemy
    public static double GetBlockChance()
    {
        double blockChance = -Math.Pow(Math.E, (double)-levelNumber / 20);
        blockChance *= 0.9;
        blockChance += 1;
        return blockChance;
    }

    //Returns the time the player has to block an attack
    public static double GetBlockTime()
    {
        return BaseBlockTime / Math.Pow(difficultyMultiplier, 0.8);
    }
}

//Holds the data for the dungeon environment, mostly unused for logic, there for future use
public class Environment
{
    public int[] Ints;
    [SerializeField]
    public EnemyType[] PossibleEnemyTypes;
    public Environment(int[] ints)
    {
        Ints = ints;
        PossibleEnemyTypes = new EnemyType[Ints.Length];
        for (int i = 0; i < Ints.Length; i++)
        {
            EnemyTypeManager enemyTypeManager = GameObject.Find("EnemyTypeManager").GetComponent<EnemyTypeManager>();
            PossibleEnemyTypes[i] = enemyTypeManager.getType(Ints[i]);
        }
    }
}