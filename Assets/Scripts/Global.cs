using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    [SerializeField]
    public static float BlockXPGain = 10f;
    [SerializeField]
    public static float StrongXPGain = 10f;
    [SerializeField]
    public static float NormalXPGain = 10f;

    public static void GameOver()
    {
        Debug.Log("Game Over");
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

    public static double GetDifficultyMultiplier()
    {
        return difficultyMultiplier;
    }

    public static double GetBlockTime()
    {
        return BaseBlockTime / Math.Pow(difficultyMultiplier, 0.8);
    }
}

