using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public enum Type
    {
        Movement,
        Chord
    }

    public enum Movement
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    public static List<Input> getInputs()
    {
        List<Input> inputs = new List<Input>();
        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Up");
            inputs.Add(new Input(Type.Movement, Movement.Up));
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Down");
            inputs.Add(new Input(Type.Movement, Movement.Down));
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right");
            inputs.Add(new Input(Type.Movement, Movement.Right));
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left");
            inputs.Add(new Input(Type.Movement, Movement.Left));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A");
            inputs.Add(new Input(Type.Chord, chord: new Chord(0, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S");
            inputs.Add(new Input(Type.Chord, chord: new Chord(1, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("D");
            inputs.Add(new Input(Type.Chord, chord: new Chord(2, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F");
            inputs.Add(new Input(Type.Chord, chord: new Chord(3, 1.0f)));
        }
        return inputs;
    }
}

public class Input
{
    public InputManager.Type type;
    public InputManager.Movement Movement;
    public Chord Chord;

    public Input(InputManager.Type type, InputManager.Movement movement = InputManager.Movement.None, Chord chord = null)
    {
        this.type = type;
        this.Movement = movement;
        Chord = chord;
    }
}
