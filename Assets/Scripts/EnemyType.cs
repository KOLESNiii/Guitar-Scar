using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Base class for all enemy types
public class EnemyType : MonoBehaviour
{
    [SerializeField]
    public List<int> StrongAgainst = new List<int>();
    [SerializeField]
    public List<int> WeakAgainst = new List<int>();
    [SerializeField]
    public List<(int, float)> Attacks = new List<(int, float)>();
    [SerializeField]
    public List<int> PossibleAttacks = new List<int>();
    [SerializeField]
    public float XP = 1f;
    void Start()
    {
        //Sets up the possible attacks, as list of tuples cannot be serialized
        foreach (int attack in PossibleAttacks)
        {
            Attacks.Add((attack, 1.0f / PossibleAttacks.Count));
        }
    }

    //Returns an attack randomly from the list of possible attacks
    public (int, float) getAttacks()
    {
        float rand = Random.Range(0f, 1f);
        float odds = 0f;
        for (int i = 0; i < Attacks.Count; i++) //logic for non-uniform attack distributions
        {
            if (rand < Attacks[i].Item2 + odds)
            {
                return (Attacks[i].Item1, 1.0f);
            }
            else
            {
                odds += Attacks[i].Item2;
            }
        }
        return (Attacks[0].Item1, 1.0f);
    }
}