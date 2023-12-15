using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    /*
    [SerializeField]
    private bool isCMaj = false;
    [SerializeField]
    private bool isEFMaj = false;
    [SerializeField]
    private bool isFMaj = false;
    [SerializeField]
    private bool isDMaj = false;
    [SerializeField]
    private bool isAMaj = false;
    [SerializeField]
    private bool isEMaj = false;
    [SerializeField]
    private bool isDFMaj = false;
    [SerializeField]
    private bool isGMaj = false;
    [SerializeField]
    private bool isAll = false;
    */

    /*void Start()
    {
        if (isCMaj)
        {
            StrongAgainst.AddRange(EnemyTypeManager.cmaj.StrongAgainst);
            WeakAgainst.AddRange(EnemyTypeManager.cmaj.WeakAgainst);
            Attacks.AddRange(EnemyTypeManager.cmaj.Attacks);
            XP += EnemyTypeManager.cmaj.XP;
        }
        if (isEFMaj)
        {
            StrongAgainst.AddRange(EnemyTypeManager.efmaj.StrongAgainst);
            WeakAgainst.AddRange(EnemyTypeManager.efmaj.WeakAgainst);
            Attacks.AddRange(EnemyTypeManager.efmaj.Attacks);
            XP += EnemyTypeManager.efmaj.XP;
        }
        if (isFMaj)
        {
            StrongAgainst.AddRange(EnemyTypeManager.fmaj.StrongAgainst);
            WeakAgainst.AddRange(EnemyTypeManager.fmaj.WeakAgainst);
            Attacks.AddRange(EnemyTypeManager.fmaj.Attacks);
            XP += EnemyTypeManager.fmaj.XP;
        }
        if (isDMaj)
        {
            StrongAgainst.AddRange(EnemyTypeManager.dmaj.StrongAgainst);
            WeakAgainst.AddRange(EnemyTypeManager.dmaj.WeakAgainst);
            Attacks.AddRange(EnemyTypeManager.dmaj.Attacks);
            XP += EnemyTypeManager.dmaj.XP;
        }
        if (isAMaj)
        {
            StrongAgainst.AddRange(EnemyTypeManager.amaj.StrongAgainst);
            WeakAgainst.AddRange(EnemyTypeManager.amaj.WeakAgainst);
            Attacks.AddRange(EnemyTypeManager.amaj.Attacks);
            XP += EnemyTypeManager.amaj.XP;
        }
        if (isEMaj)
        {
            StrongAgainst.AddRange(EnemyTypeManager.emaj.StrongAgainst);
            WeakAgainst.AddRange(EnemyTypeManager.emaj.WeakAgainst);
            Attacks.AddRange(EnemyTypeManager.emaj.Attacks);
            XP += EnemyTypeManager.emaj.XP;
        }
        if (isDFMaj)
        {
            StrongAgainst.AddRange(EnemyTypeManager.dfmaj.StrongAgainst);
            WeakAgainst.AddRange(EnemyTypeManager.dfmaj.WeakAgainst);
            Attacks.AddRange(EnemyTypeManager.dfmaj.Attacks);
            XP += EnemyTypeManager.dfmaj.XP;
        }
        if (isGMaj)
        {
            StrongAgainst.AddRange(EnemyTypeManager.gmaj.StrongAgainst);
            WeakAgainst.AddRange(EnemyTypeManager.gmaj.WeakAgainst);
            Attacks.AddRange(EnemyTypeManager.gmaj.Attacks);
            XP += EnemyTypeManager.gmaj.XP;
        }
        if (isAll)
        {
            StrongAgainst = EnemyTypeManager.all.StrongAgainst;
            WeakAgainst = EnemyTypeManager.all.WeakAgainst;
            Attacks = EnemyTypeManager.all.Attacks;
            XP = EnemyTypeManager.all.XP;
        }
    }
    
    */
    void Start()
    {
        foreach (int attack in PossibleAttacks)
        {
            Attacks.Add((attack, 1.0f / PossibleAttacks.Count));
        }
    }

    public (int, float) getAttacks()
    {
        float rand = Random.Range(0f, 1f);
        float odds = 0f;
        for (int i = 0; i < Attacks.Count; i++)
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

/*public static class EnemyTypeManager
{
    public static EnemyType cmaj = new EnemyType
    (
        strongAgainst: new List<int> {0, 5, 7, 21, 16},
        weakAgainst : new List<int> {13, 3, 22, 14, 12, 1},
        attacks : new List<(int, float)> {(0, 0.25f), (5, 0.25f), (21, 0.25f), (7, 0.25f)},
        xp : 10f 
    );

    public static EnemyType efmaj = new EnemyType
    (
        strongAgainst: new List<int> {3, 12, 5},
        weakAgainst: new List<int> {7, 23, 13, 14, 16},
        attacks: new List<(int, float)> {(12, 0.3333f), (3, 0.3333f), (5, 0.3333f)},
        xp: 10f
    );

    public static EnemyType fmaj = new EnemyType
    (
        strongAgainst: new List<int> {0, 5, 7, 14},
        weakAgainst: new List<int> {1, 4, 22, 23, 13, 16, 18},
        attacks: new List<(int, float)> {(5, 0.25f), (14, 0.25f), (0, 0.25f), (7, 0.25f)},
        xp: 10f
    );

    public static EnemyType dmaj = new EnemyType
    (
        strongAgainst: new List<int> {2, 23, 5},
        weakAgainst: new List<int> {12, 13, 16, 22},
        attacks: new List<(int, float)> {(2, 0.3333f), (23, 0.3333f), (5, 0.3333f)},
        xp: 10f
    );

    public static EnemyType amaj = new EnemyType
    (
        strongAgainst: new List<int> {9, 18, 2, 5, 14},
        weakAgainst: new List<int> {22, },
        attacks: new List<(int, float)> {(9, 0.25f), (18, 0.25f), (2, 0.25f), (14, 0.25f)},
        xp: 10f
    );

    public static EnemyType emaj = new EnemyType
    (
        strongAgainst: new List<int> {4, 13, 9},
        weakAgainst: new List<int> {2, 3, 5, 22, 12, 14},
        attacks: new List<(int, float)> {(4, 0.3333f), (13, 0.3333f), (9, 0.3333f)},
        xp: 10f
    );

    public static EnemyType dfmaj = new EnemyType
    (
        strongAgainst: new List<int> {1, 22, 9, 18},
        weakAgainst: new List<int> {0, 2, 23, 12},
        attacks: new List<(int, float)> {(1, 0.25f), (22, 0.25f), (9, 0.25f), (18, 0.25f)},
        xp: 10f
    );

    public static EnemyType gmaj = new EnemyType
    (
        strongAgainst: new List<int> {7, 16, 2},
        weakAgainst: new List<int> {18},
        attacks: new List<(int, float)> {(7, 0.25f), (16, 0.25f), (9, 0.25f), (18, 0.25f)},
        xp: 10f
    );
    
    public static EnemyType all = new EnemyType
    (
        strongAgainst: new List<int>(),
        weakAgainst: new List<int>(),
        attacks: new List<(int, float)> {(0, 0.0625f), (1, 0.0625f), (2, 0.0625f), (3, 0.0625f), (4, 0.0625f), (5, 0.0625f), (7, 0.0625f), (9, 0.0625f),
                                        (12, 0.0625f), (13, 0.0625f), (14, 0.0625f), (16, 0.0625f), (18, 0.0625f), (21, 0.0625f), (22, 0.0625f), (23, 0.0625f)},
        xp: 20f
    );
}
*/
