using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This class stores the information of all chords, along with some methods to get information about chords
public static class ChordLibrary
{
    private static double[,] chords = new double[108,12]; //initialises 2D array to store chords
    //C, C#, D, D#, E, F, F#, G, G#, A, A#, B
    //Maj, min, dim, aug, sus2, sus4, maj7, min7, dom7
    private static int[] chordsInGame = {0, 1, 2, 3, 4, 5, 7, 9, //C, C#, D, D#, E, F, G, A maj
                                   12, 13, 14, 16, 18, 21, 22, 23}; //c, c#, d, e, f#, a, a#, b min
    
    //helper function to get chord name from index
    public static string GetChordName(int index) 
    {
        string[] names = {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A",
            "A#", "B"};
        string[] qualities = {"maj", "min", "dim", "aug", "sus2", "sus4", "maj7", "min7", "dom7"};
        return names[index % 12] + " " + qualities[index / 12];
    }

    //Checks if the given indices are relative major and minor chords
    public static bool IsRelativeMajMin(int index1, int index2)
    {
        if (index1 / 12 == 0) // is major
        {
            return index2 / 12 == 1 && (index1 + 9) % 12 == index2 % 12;
        }
        else if (index1 / 12 == 1) // is minor
        {
            return index2 / 12 == 0 && index1 % 12 == index2 % 12;
        }
        else
        {
            return false;
        }
    }

    //Helper function to generate chord library
    public static void SetChord(int index, int root, int third, int fifth)
    {
        for (int i = 0; i < 12; i++)
        {
            chords[index, i] = 0;
        }
        chords[index, root] = 1;
        chords[index, third] = 1;
        chords[index, fifth] = 1;
    }

    //Helper function to generate chord library
    public static void SetChord(int index, int root, int third, int fifth, int seventh)
    {
        for (int i = 0; i < 12; i++)
        {
            chords[index, i] = 0;
        }
        chords[index, root] = 1;
        chords[index, third] = 1;
        chords[index, fifth] = 1;
        chords[index, seventh] = 1;
    }

    //Generates the chord library
    public static void GenerateChordLibrary()
    {
        int index = 0;
        int root;
        int third;
        int fifth;
        int seventh;
        int[][] offsets = new int[9][];
        offsets[0] = new int[] {4, 7}; //maj
        offsets[1] = new int[] {3, 7}; //min
        offsets[2] = new int[] {3, 6}; //dim
        offsets[3] = new int[] {4, 8}; //aug
        offsets[4] = new int[] {2, 7}; //sus2
        offsets[5] = new int[] {5, 7}; //sus4
        offsets[6] = new int[] {4, 7, 11}; //maj7
        offsets[7] = new int[] {3, 7, 10}; //min7
        offsets[8] = new int[] {4, 7, 10}; //dom7
        foreach (int[] offset in offsets) //iterate through chord types
        {
            for (int i = 0; i < 12; i++) //iterates through all root notes
            {
                root = i % 12;
                third = (i + offset[0]) % 12;
                fifth = (i + offset[1]) % 12;
                if (offset.Length == 3)
                {
                    seventh = (i + offset[2]) % 12;
                    SetChord(index, root, third, fifth, seventh);
                }
                else
                {
                    SetChord(index, root, third, fifth);
                }
                index++;
            }
        }
    }
    
    //Gets the multiplier for the given chord index, and the index of the closest chord in the game
    public static (double, int) GetChord_Multiplier(int index)
    {
        if (chordsInGame.Contains(index)) //if chord is in game
        {
            return (1.0, index);
        }
        else //if chord is not in game
        {
            int[] differences = new int[12];
            int[] TotalDifferences = new int[chordsInGame.Length];
            for (int i = 0; i < chordsInGame.Length; i++) //iterate through chords in game
            {
                for (int j = 0; j < 12; j++)
                {
                    differences[j] = (int)Math.Abs(chords[chordsInGame[i], j] - chords[index, j]); //calculate difference between chords
                }
                TotalDifferences[i] = differences.Sum();
            }
            int minValue = TotalDifferences.Min(); //get minimum difference, hence closest chord
            int minIndex = chordsInGame[Array.IndexOf(TotalDifferences, minValue)]; //get index of closest chord
            double multiplier = GetMultiplier(minValue); //get multiplier based off number of differences
            return (multiplier, minIndex);
        }
    }

    //Gets the multiplier for the given difference
    public static double GetMultiplier(int difference)
    {
        if (difference > 4)
        {
            return 0.0;
        }
        return 0.06 * Math.Pow(difference, 2) - 0.48 * difference + 1.0;
    }

    public static double[,] GetChordProfiles()
    {
        return chords;
    }
}