using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    void Start()
    {
    }
    void Update()
    {
        var inputs = InputManager.getInputs();
        foreach (var input in inputs)
        {
            if (input.type == InputManager.Type.Chord)
            {
            }
        }
    }
}
