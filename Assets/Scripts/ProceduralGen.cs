using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGen
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        var currentPosition = startPosition;
        path.Add(currentPosition);

        for (int i = 0; i < walkLength; i++)
        {
            currentPosition += Direction2D.RandomDirection();
            path.Add(currentPosition);
        }

        return path;
    }
}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionList = new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public static Vector2Int RandomDirection()
    {
        return cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];
    }
}