using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public int[] chordsToTest = new int[] {0, 2, 7, 21, 16, 23};
    void Start()
    {
        for (int i = 0; i < chordsToTest.Length; i++)
        {
            for (int j = 0; j < chordsToTest.Length; j++)
            {
                Debug.Log(ChordLibrary.GetChordName(chordsToTest[i]) + " and " + ChordLibrary.GetChordName(chordsToTest[j]) + " are relative major and minor: " + ChordLibrary.IsRelativeMajMin(chordsToTest[i], chordsToTest[j]));
            }
        }
    }
}
