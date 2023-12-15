using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Global
{
    [SerializeField]
    public static float BlockXPGain = 2f;
    [SerializeField]
    public static float StrongXPGain = 2f;
    [SerializeField]
    public static float NormalXPGain = 1f;
    public static float LevelUpMultiplier = 1.05f;
    public static float startingArmour = 100f;
    public static float startingHealth = 100f;
    public static float startingDamage = 5f;
    public static float startingXPToNextLevel = 100f;

    public static void GameOver()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
    }
    public static void ExitLevel()
    {
        GameDataManager.Instance.SaveGame();
        Debug.Log("Level Complete");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

}

public static class Level
{
    public static int levelNumber = 0;
    private static double difficultyMultiplier = 1.0;
    [SerializeField]
    private static double BaseBlockTime = 3.0;
    public static void NextLevel()
    {
        levelNumber++;
        difficultyMultiplier = Math.Pow(Math.E, (double)levelNumber / 15);
    }

    public static void SetLevel(int level)
    {
        levelNumber = level;
        difficultyMultiplier = Math.Pow(Math.E, (double)levelNumber / 15);
    }

    public static double GetDifficultyMultiplier()
    {
        return difficultyMultiplier;
    }

    public static double GetBlockChance()
    {
        double blockChance = -Math.Pow(Math.E, (double)-levelNumber / 20);
        blockChance *= 0.9;
        blockChance += 1;
        return blockChance;
    }

    public static double GetBlockTime()
    {
        return BaseBlockTime / Math.Pow(difficultyMultiplier, 0.8);
    }
}

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

