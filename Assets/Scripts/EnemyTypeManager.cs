using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//Manages enemy type data
public class EnemyTypeManager : MonoBehaviour
{
    //variables assigned in unity editor
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
    //Returns the enemy type based on the int passed in
    public EnemyType getType(int Int)
    {
        switch (Int)
        {
            case 0:
                return cmaj;
            case 1:
                return efmaj;
            case 2:
                return fmaj;
            case 3:
                return dmaj;
            case 4:
                return amaj;
            case 5:
                return emaj;
            case 6:
                return dfmaj;
            case 7:
                return gmaj;
            default:
                return all;
        }
    }
    //Returns a random enemy sprite based on the enemy type passed in
    public GameObject getEnemy(EnemyType type)
    {
        switch (type)
        {
            case cmaj:
                return cMajSprites[Random.Range(0, cMajSprites.Count)];
            case efmaj:
                return efMajSprites[Random.Range(0, efMajSprites.Count)];
            case fmaj:
                return fMajSprites[Random.Range(0, fMajSprites.Count)];
            case dmaj:
                return dMajSprites[Random.Range(0, dMajSprites.Count)];
            case amaj:
                return aMajSprites[Random.Range(0, aMajSprites.Count)];
            case emaj:
                return eMajSprites[Random.Range(0, eMajSprites.Count)];
            case dfmaj:
                return dfMajSprites[Random.Range(0, dfMajSprites.Count)];
            case gmaj:
                return gMajSprites[Random.Range(0, gMajSprites.Count)];
            default:
                return allSprites[Random.Range(0, allSprites.Count)];
        }
    }
}