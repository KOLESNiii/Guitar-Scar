using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class stores the information of a chord
public class Chord
{
    public int ChordIndex //index of chord as defined in ChordLibrary
    {get; private set;}
    public float Quality
    {get; private set;}

    public Chord(int index, float quality)
    {
        ChordIndex = index;
        Quality = quality;
    }

}