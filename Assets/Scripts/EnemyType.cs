using UnityEngine;

public class EnemyType : MonoBehaviour
    {
        [SerializeField]
        public int[] StrongAgainst;
        [SerializeField]
        public int[] WeakAgainst;
        [SerializeField]
        public int[,] Attacks;
        [SerializeField]
        public float XP
        {private set; get;}

        /*public EnemyType(int[] strongAgainst, int[] weakAgainst, int[,] attacks, float xp)
        {
            StrongAgainst = strongAgainst;
            WeakAgainst = weakAgainst;
            Attacks = attacks;
            XP = xp;
        }*/

        public (int, float) getAttacks()
        {
            float rand = Random.Range(0f, 1f);
            float odds = 0f;
            for (int i = 0; i < Attacks.Length; i++)
            {
                if (rand < Attacks[i, 1] + odds)
                {
                    return (Attacks[i, 0], 1.0f);
                }
                else
                {
                    odds += Attacks[i, 1];
                }
            }
            return (Attacks[0, 0], 1.0f);
        }
    }
