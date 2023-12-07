using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chord
{
    public int ChordIndex
    {get; private set;}
    public float Quality
    {get; private set;}

    public Chord(int index, float quality)
    {
        ChordIndex = index;
        Quality = quality;
    }

}
