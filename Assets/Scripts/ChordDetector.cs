using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class ChordDetector
{
    private double[] chromagram = new double[12];
    private double[] chord = new double[108];
    private double[,] chordProfiles;
    private double bias;
    public ChordDetector()
    {
        bias = 1.06;
        ChordLibrary.GenerateChordLibrary();
        chordProfiles = ChordLibrary.GetChordProfiles();
    }
    public void detectChord(double[] chroma)
    {
        for (int i = 0; i < 12; i++)
        {
            chromagram[i] = chroma[i];
        }
        classifyChromagram();
    }
    
    private void classifyChromagram()
    {
        int i, j, fifth, chordIndex;
        for (i = 0; i < 12; i++)
        {
            fifth = (i+7) % 12;
            chromagram[fifth] = chromagram[fifth] - (0.1 * chromagram[i]);
            if (chromagram[fifth] < 0)
            {
                chromagram[fifth] = 0;
            }
        }
        //maj, min, dim5th & aug5th
        for (j = 0; j < 48; j++)
        {
            chord[j] = calculateChordScore(chromagram, getChordProfile(j), bias, 3);
        }
        //different bias for sus chords
        for (j = 48; j < 72; j++)
        {
            chord[j] = calculateChordScore(chromagram, getChordProfile(j), 1, 3);
        }
        //maj 7th
        for (j = 72; j < 84; j++)
        {
            chord[j] = calculateChordScore(chromagram, getChordProfile(j), 1, 4);
        }
        //min 7th & dom 7th
        for (j = 84; j < 108; j++)
        {
            chord[j] = calculateChordScore(chromagram, getChordProfile(j), bias, 4);
        }
        chordIndex = minimumIndex(chord);
        Debug.Log(chordIndex);
        InputManager.AddChordInput(chordIndex);
    }

    private double calculateChordScore(double[] chroma, double[] chordProfile, double biasToUse, int N)
    {
        double sum = 0;
        double delta;
        for (int i = 0; i < 12; i++)
        {
            sum += (1 - chordProfile[i]) * (chroma[i] * chroma[i]);
        }
        delta = Mathf.Sqrt((float)sum) / ((12 - N) * biasToUse);
        return delta;
    }

    private double[] getChordProfile(int index)
    {
        double[] profile = new double[12];
        for (int i = 0; i < 12; i++)
        {
            profile[i] = chordProfiles[index, i];
        }
        return profile;
    }

    private int minimumIndex(double[] array)
    {
        int minIndex = array.Select((value, index) => new {Value = value, Index = index}).OrderBy(item => item.Value).Select(item => item.Index).First();
        string chordValues = "";
        for (int i = 0; i < 108; i++)
        {
            chordValues += array[i].ToString() + ", ";
        }
        Debug.Log(chordValues);
        return minIndex;
    }
}
