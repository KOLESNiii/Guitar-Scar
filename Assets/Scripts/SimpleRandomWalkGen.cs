using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class SimpleRandomWalkGen : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    public int walkLength = 10;
    [SerializeField]
    public bool startRandomlyEachIteration = true;
    
    public void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();
        foreach (var position in floorPositions)
        {
            Debug.Log(position);
        } 
    }

    protected HashSet<Vector2Int> RunRandomWalk()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        var currentPosition = startPosition;

        for (int i = 0; i < iterations; i++)
        {
            var path = ProceduralGen.SimpleRandomWalk(currentPosition, walkLength);
            floorPositions.UnionWith(path); 
            if (startRandomlyEachIteration)
            {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }

        return floorPositions ;
    }
}
