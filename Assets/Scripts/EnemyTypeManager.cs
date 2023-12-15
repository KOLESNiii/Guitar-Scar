using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTypeManager : MonoBehaviour
{
    public EnemyType cmaj;
    public EnemyType efmaj;
    public EnemyType fmaj;
    public EnemyType dmaj;
    public EnemyType amaj;
    public EnemyType emaj;
    public EnemyType dfmaj;
    public EnemyType gmaj;
    public EnemyType all;
    public List<GameObject> cMajSprites;
    public List<GameObject> efMajSprites;
    public List<GameObject> fMajSprites;
    public List<GameObject> dMajSprites;
    public List<GameObject> aMajSprites;
    public List<GameObject> eMajSprites;
    public List<GameObject> dfMajSprites;
    public List<GameObject> gMajSprites;
    public List<GameObject> allSprites;
    public EnemyType getType(int Int)
    {
        if (Int == 0)
        {
            return cmaj;
        }
        else if (Int == 1)
        {
            return efmaj;
        }
        else if (Int == 2)
        {
            return fmaj;
        }
        else if (Int == 3)
        {
            return dmaj;
        }
        else if (Int == 4)
        {
            return amaj;
        }
        else if (Int == 5)
        {
            return emaj;
        }
        else if (Int == 6)
        {
            return dfmaj;
        }
        else if (Int == 7)
        {
            return gmaj;
        }
        else
        {
            return all;
        }
    }
    public GameObject getEnemy(EnemyType type)
    {
        if (type == cmaj)
        {
            return cMajSprites[Random.Range(0, cMajSprites.Count)];
        } 
        else if (type == efmaj)
        {
            return efMajSprites[Random.Range(0, efMajSprites.Count)];
        }
        else if (type == fmaj)
        {
            return fMajSprites[Random.Range(0, fMajSprites.Count)];
        }
        else if (type == dmaj)
        {
            return dMajSprites[Random.Range(0, dMajSprites.Count)];
        }
        else if (type == amaj)
        {
            return aMajSprites[Random.Range(0, aMajSprites.Count)];
        }
        else if (type == emaj)
        {
            return eMajSprites[Random.Range(0, eMajSprites.Count)];
        }
        else if (type == dfmaj)
        {
            return dfMajSprites[Random.Range(0, dfMajSprites.Count)];
        }
        else if (type == gmaj)
        {
            return gMajSprites[Random.Range(0, gMajSprites.Count)];
        }
        else
        {
            return allSprites[Random.Range(0, allSprites.Count)];
        }
    }
}
