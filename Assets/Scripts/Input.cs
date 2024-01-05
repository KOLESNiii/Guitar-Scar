using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static class to handle inputs
public static class InputManager
{
    private static List<Input> inputs = new List<Input>();
    public enum Type
    {
        Movement,
        Chord,
        Pause
    }

    public enum Movement
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
    //Returns a list of inputs from the player
    public static List<Input> getInputs()
    {
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
            Debug.Log("Player input C major");
            inputs.Add(new Input(Type.Chord, chord: new Chord(0, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Player input C# major");
            inputs.Add(new Input(Type.Chord, chord: new Chord(1, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Player input D major");
            inputs.Add(new Input(Type.Chord, chord: new Chord(2, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Player input D# major");
            inputs.Add(new Input(Type.Chord, chord: new Chord(3, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Player input E major");
            inputs.Add(new Input(Type.Chord, chord: new Chord(4, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Player input F major");
            inputs.Add(new Input(Type.Chord, chord: new Chord(5, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Player input F# major");
            inputs.Add(new Input(Type.Chord, chord: new Chord(6, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Player input G major");
            inputs.Add(new Input(Type.Chord, chord: new Chord(7, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Player input G# major");
            inputs.Add(new Input(Type.Chord, chord: new Chord(8, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Player input A minor");
            inputs.Add(new Input(Type.Chord, chord: new Chord(21, 1.0f)));
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Player input Pause");
            inputs.Add(new Input(Type.Pause));
        }
        var inputReturn = new List<Input>(inputs);
        inputs.Clear();
        return inputReturn;
    }

    public static void AddChordInput(int chordIndex)
    {
        double quality;
        int index;
        (quality, index) = ChordLibrary.GetChord_Multiplier(chordIndex);
        inputs.Add(new Input(Type.Chord, chord: new Chord(index, (float)quality)));
    }
}
//Class to represent an input
public class Input
{
    public InputManager.Type type;
    public InputManager.Movement Movement;
    public Chord Chord;
    //Have to specify a type, but optional extra information if the input is a movement or chord
    public Input(InputManager.Type type, InputManager.Movement movement = InputManager.Movement.None, Chord chord = null)
    {
        this.type = type;
        this.Movement = movement;
        Chord = chord;
    }
}