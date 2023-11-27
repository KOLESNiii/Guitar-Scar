using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PainterHelper : MonoBehaviour
{
    [SerializeField]
    public List<TileBase> floorTiles;
    [SerializeField]
    float floorTileChance = 0.0f;
    [SerializeField]
    private TileBase bottomRightWallTile;
    [SerializeField]
    private TileBase bottomLeftWallTile;
    [SerializeField]
    private TileBase topRightWallTile;
    [SerializeField]
    private TileBase topLeftWallTile;
    [SerializeField]
    private TileBase topWallTile;
    [SerializeField]
    private TileBase bottomWallTile;
    [SerializeField]
    private TileBase leftWallTile;
    [SerializeField]
    private TileBase rightWallTile;
    [SerializeField]
    private TileBase bottomRightWallInverseTile;
    [SerializeField]
    private TileBase bottomLeftWallInverseTile;
    [SerializeField]
    private TileBase topRightWallInverseTile;
    [SerializeField]
    private TileBase topLeftWallInverseTile;
    [SerializeField]
    public TileBase emptyTile;
    [SerializeField]
    private TileBase errorTile;
    [SerializeField]
    private List<TileBase> objectTiles;

    private TileBase getPlainTile()
    {
        var randomValue = Random.Range(0, 1f);
        float chanceAfter = (1 - floorTileChance) / (floorTiles.Count - 1);
        if (randomValue < floorTileChance)
        {
            return floorTiles[0];
        }
        else
        {
            return floorTiles[1 + Mathf.FloorToInt((randomValue - floorTileChance) / chanceAfter)];
        }
    }
    public TileBase GetTile(ushort tileValue)
    {
        if (tileValue >= 256)
        {
            return getPlainTile();
        }
        switch (tileValue)
        {
            case 0:
                return emptyTile;
            case 0b0000_0001:
                return bottomRightWallInverseTile;
            case 0b0000_0010:
                return rightWallTile;
            case 0b0000_0011:
                return rightWallTile;
            case 0b0000_0100:
                return topRightWallInverseTile;
            case 0b0000_0101:
                return rightWallTile;
            case 0b0000_0110:
                return rightWallTile;
            case 0b0000_0111:
                return rightWallTile;
            case 0b0000_1000:
                return topWallTile;
            case 0b0000_1001:
                return topRightWallTile;
            case 0b0000_1010:
                return topRightWallTile;
            case 0b0000_1011:
                return topRightWallTile;
            case 0b0000_1100:
                return topWallTile;
            case 0b0000_1101:
                return topRightWallTile;
            case 0b0000_1110:
                return topRightWallTile;
            case 0b0000_1111:
                return topRightWallTile;
            case 0b0001_0000:
                return topLeftWallInverseTile;
            case 0b0001_0001:
                return bottomRightWallInverseTile;
            case 0b0001_0010:
                return topRightWallTile;
            case 0b0001_0011:              
                return topRightWallTile;
            case 0b0001_0100:
                return topWallTile;
            case 0b0001_0101:
                return topRightWallTile;
            case 0b0001_0110:
                return topRightWallTile;
            case 0b0001_0111:
                return topRightWallTile;
            case 0b0001_1000:
                return topWallTile;
            case 0b0001_1001:
                return topRightWallTile;
            case 0b0001_1010:
                return topRightWallTile;
            case 0b0001_1011:
                return topRightWallTile;
            case 0b0001_1100:
                return topWallTile;
            case 0b0001_1101:
                return topRightWallTile;
            case 0b0001_1110:
                return topRightWallTile;
            case 0b0001_1111:
                return topRightWallTile;
            case 0b0010_0000:
                return leftWallTile;
            case 0b0010_0001:
                return bottomLeftWallTile;
            case 0b0010_0010:
                return getPlainTile();
            case 0b0010_0011:  
                return getPlainTile();
            case 0b0010_0100:
                return topLeftWallTile;
            case 0b0010_0101:
                return getPlainTile();
            case 0b0010_0110:
                return getPlainTile();
            case 0b0010_0111:
                return getPlainTile();
            case 0b0010_1000:
                return topLeftWallTile;
            case 0b0010_1001:
                return emptyTile;
            case 0b0010_1010:
                return emptyTile;
            case 0b0010_1011:
                return emptyTile;
            case 0b0010_1100:
                return topLeftWallTile;
            case 0b0010_1101:
                return emptyTile;
            case 0b0010_1110:
                return getPlainTile();
            case 0b0010_1111:
                return getPlainTile();
            case 0b0011_0000:
                return leftWallTile;
            case 0b0011_0001:
                return bottomLeftWallTile;
            case 0b0011_0010:
                return getPlainTile();
            case 0b0011_0011:
                return getPlainTile();
            case 0b0011_0100:
                return topLeftWallTile;
            case 0b0011_0101:
                return getPlainTile();
            case 0b0011_0110:
                return getPlainTile();
            case 0b0011_0111:
                return getPlainTile();
            case 0b0011_1000:
                return topLeftWallTile;
            case 0b0011_1001:
                return getPlainTile();
            case 0b0011_1010:
                return getPlainTile();
            case 0b0011_1011:
                return getPlainTile();
            case 0b0011_1100:
                return topLeftWallTile;
            case 0b0011_1101:
                return getPlainTile();
            case 0b0011_1110:
                return getPlainTile();
            case 0b0011_1111:
                return getPlainTile();
            case 0b0100_0000:
                return bottomLeftWallInverseTile;
            case 0b0100_0001:
                return bottomWallTile;
            case 0b0100_0010:
                return bottomRightWallTile;
            case 0b0100_0011:
                return bottomRightWallTile;
            case 0b0100_0100:
                return bottomLeftWallInverseTile;
            case 0b0100_0101:
                return bottomRightWallTile;
            case 0b0100_0110:
                return bottomRightWallTile;
            case 0b0100_0111:
                return bottomRightWallTile;
            case 0b0100_1000:
                return topLeftWallTile;
            case 0b0100_1001:
                return getPlainTile();
            case 0b0100_1010:
                return emptyTile;
            case 0b0100_1011:
                return emptyTile;
            case 0b0100_1100:
                return topLeftWallTile;
            case 0b0100_1101:
                return getPlainTile();
            case 0b0100_1110:
                return getPlainTile();
            case 0b0100_1111:
                return getPlainTile(); 
            case 0b0101_0000:
                return leftWallTile;
            case 0b0101_0001:
                return bottomLeftWallTile;
            case 0b0101_0010:
                return getPlainTile();
            case 0b0101_0011:              
                return getPlainTile();
            case 0b0101_0100:
                return topLeftWallTile;
            case 0b0101_0101:
                return getPlainTile();
            case 0b0101_0110:
                return emptyTile;
            case 0b0101_0111:
                return emptyTile;
            case 0b0101_1000:
                return topLeftWallTile;
            case 0b0101_1001:
                return emptyTile;
            case 0b0101_1010:
                return emptyTile;
            case 0b0101_1011:
                return emptyTile;
            case 0b0101_1100:
                return topLeftWallTile;
            case 0b0101_1101:
                return getPlainTile();
            case 0b0101_1110:
                return getPlainTile();
            case 0b0101_1111:
                return getPlainTile();
            case 0b0110_0000:
                return leftWallTile;
            case 0b0110_0001:
                return bottomLeftWallTile;
            case 0b0110_0010:
                return getPlainTile();
            case 0b0110_0011:  
                return getPlainTile();
            case 0b0110_0100:
                return topLeftWallTile;
            case 0b0110_0101:
                return getPlainTile();
            case 0b0110_0110:
                return getPlainTile();
            case 0b0110_0111:
                return getPlainTile();
            case 0b0110_1000:
                return topLeftWallTile;
            case 0b0110_1001:
                return emptyTile;
            case 0b0110_1010:
                return emptyTile;
            case 0b0110_1011:
                return emptyTile;
            case 0b0110_1100:
                return topLeftWallTile;
            case 0b0110_1101:
                return emptyTile;
            case 0b0110_1110:
                return getPlainTile();
            case 0b0110_1111:
                return getPlainTile();
            case 0b0111_0000:
                return leftWallTile;
            case 0b0111_0001:
                return bottomLeftWallTile;
            case 0b0111_0010:
                return getPlainTile();
            case 0b0111_0011:
                return getPlainTile();
            case 0b0111_0100:
                return topLeftWallTile;
            case 0b0111_0101:
                return getPlainTile();
            case 0b0111_0110:
                return getPlainTile();
            case 0b0111_0111:
                return getPlainTile();
            case 0b0111_1000:
                return topLeftWallTile;
            case 0b0111_1001:
                return getPlainTile();
            case 0b0111_1010:
                return getPlainTile();
            case 0b0111_1011:
                return getPlainTile();
            case 0b0111_1100:
                return topLeftWallTile;
            case 0b0111_1101:
                return getPlainTile();
            case 0b0111_1110:
                return getPlainTile();
            case 0b0111_1111:
                return getPlainTile();
            case 0b1000_0000:
                return bottomWallTile;
            case 0b1000_0001:
                return bottomWallTile;
            case 0b1000_0010:
                return bottomRightWallTile;
            case 0b1000_0011:
                return bottomRightWallTile;
            case 0b1000_0100:
                return bottomRightWallTile;
            case 0b1000_0101:
                return bottomRightWallTile;
            case 0b1000_0110:
                return bottomRightWallTile;
            case 0b1000_0111:
                return bottomRightWallTile;
            case 0b1000_1000:
                return emptyTile;
            case 0b1000_1001:
                return getPlainTile();
            case 0b1000_1010:
                return emptyTile;
            case 0b1000_1011:
                return getPlainTile();
            case 0b1000_1100:
                return getPlainTile();
            case 0b1000_1101:
                return getPlainTile();
            case 0b1000_1110:
                return getPlainTile();
            case 0b1000_1111:
                return getPlainTile();
            case 0b1001_0000:
                return bottomLeftWallTile;
            case 0b1001_0001:
                return bottomLeftWallTile;
            case 0b1001_0010:
                return emptyTile;
            case 0b1001_0011:              
                return getPlainTile();
            case 0b1001_0100:
                return getPlainTile();
            case 0b1001_0101:
                return getPlainTile();
            case 0b1001_0110:
                return emptyTile;
            case 0b1001_0111:
                return getPlainTile();
            case 0b1001_1000:
                return getPlainTile();
            case 0b1001_1001:
                return getPlainTile();
            case 0b1001_1010:
                return emptyTile;
            case 0b1001_1011:
                return getPlainTile();
            case 0b1001_1100:
                return getPlainTile();
            case 0b1001_1101:
                return getPlainTile();
            case 0b1001_1110:
                return emptyTile;
            case 0b1001_1111:
                return getPlainTile();
            case 0b1010_0000:
                return bottomLeftWallTile;
            case 0b1010_0001:
                return bottomLeftWallTile;
            case 0b1010_0010:
                return emptyTile;
            case 0b1010_0011:  
                return getPlainTile();
            case 0b1010_0100:
                return emptyTile;
            case 0b1010_0101:
                return getPlainTile();
            case 0b1010_0110:
                return emptyTile;
            case 0b1010_0111:
                return getPlainTile();
            case 0b1010_1000:
                return emptyTile;
            case 0b1010_1001:
                return emptyTile;
            case 0b1010_1010:
                return emptyTile;
            case 0b1010_1011:
                return emptyTile;
            case 0b1010_1100:
                return emptyTile;
            case 0b1010_1101:
                return emptyTile;
            case 0b1010_1110:
                return emptyTile;
            case 0b1010_1111:
                return emptyTile;
            case 0b1011_0000:
                return bottomLeftWallTile;
            case 0b1011_0001:
                return bottomLeftWallTile;
            case 0b1011_0010:
                return emptyTile;
            case 0b1011_0011:
                return getPlainTile();
            case 0b1011_0100:
                return emptyTile;
            case 0b1011_0101:
                return emptyTile;
            case 0b1011_0110:
                return emptyTile;
            case 0b1011_0111:
                return getPlainTile();
            case 0b1011_1000:
                return getPlainTile();
            case 0b1011_1001:
                return getPlainTile();
            case 0b1011_1010:
                return emptyTile;
            case 0b1011_1011:
                return getPlainTile();
            case 0b1011_1100:
                return emptyTile;
            case 0b1011_1101:
                return getPlainTile();
            case 0b1011_1110:
                return emptyTile;
            case 0b1011_1111:
                return getPlainTile();
            case 0b1100_0000:
                return bottomWallTile;
            case 0b1100_0001:
                return bottomWallTile;
            case 0b1100_0010:
                return bottomRightWallTile;
            case 0b1100_0011:
                return bottomRightWallTile;
            case 0b1100_0100:
                return bottomRightWallTile;
            case 0b1100_0101:
                return bottomRightWallTile;
            case 0b1100_0110:
                return bottomRightWallTile;
            case 0b1100_0111:
                return bottomRightWallTile;
            case 0b1100_1000:
                return getPlainTile();
            case 0b1100_1001:
                return getPlainTile();
            case 0b1100_1010:
                return emptyTile;
            case 0b1100_1011:
                return getPlainTile();
            case 0b1100_1100:
                return getPlainTile();
            case 0b1100_1101:
                return getPlainTile();
            case 0b1100_1110:
                return getPlainTile();
            case 0b1100_1111:
                return getPlainTile();
            case 0b1101_0000:
                return bottomLeftWallTile;
            case 0b1101_0001:
                return bottomLeftWallTile;
            case 0b1101_0010:
                return emptyTile;
            case 0b1101_0011:              
                return getPlainTile();
            case 0b1101_0100:
                return getPlainTile();
            case 0b1101_0101:
                return getPlainTile();
            case 0b1101_0110:
                return emptyTile;
            case 0b1101_0111:
                return getPlainTile();
            case 0b1101_1000:
                return getPlainTile();
            case 0b1101_1001:
                return getPlainTile();
            case 0b1101_1010:
                return emptyTile;
            case 0b1101_1011:
                return getPlainTile();
            case 0b1101_1100:
                return getPlainTile();
            case 0b1101_1101:
                return getPlainTile();
            case 0b1101_1110:
                return getPlainTile();
            case 0b1101_1111:
                return getPlainTile();
            case 0b1110_0000:
                return bottomLeftWallTile;
            case 0b1110_0001:
                return bottomLeftWallTile;
            case 0b1110_0010:
                return getPlainTile();
            case 0b1110_0011:  
                return getPlainTile();
            case 0b1110_0100:
                return getPlainTile();
            case 0b1110_0101:
                return getPlainTile();
            case 0b1110_0110:
                return getPlainTile();
            case 0b1110_0111:
                return getPlainTile();
            case 0b1110_1000:
                return getPlainTile();
            case 0b1110_1001:
                return getPlainTile();
            case 0b1110_1010:
                return emptyTile;
            case 0b1110_1011:
                return emptyTile;
            case 0b1110_1100:
                return getPlainTile();
            case 0b1110_1101:
                return getPlainTile();
            case 0b1110_1110:
                return getPlainTile();
            case 0b1110_1111:
                return getPlainTile();
            case 0b1111_0000:
                return bottomLeftWallTile;
            case 0b1111_0001:
                return bottomLeftWallTile;
            case 0b1111_0010:
                return emptyTile;
            case 0b1111_0011:
                return getPlainTile();
            case 0b1111_0100:
                return getPlainTile();
            case 0b1111_0101:
                return getPlainTile();
            case 0b1111_0110:
                return getPlainTile();
            case 0b1111_0111:
                return getPlainTile();
            case 0b1111_1000:
                return getPlainTile();
            case 0b1111_1001:
                return getPlainTile();
            case 0b1111_1010:
                return emptyTile;
            case 0b1111_1011:
                return getPlainTile();
            case 0b1111_1100:
                return getPlainTile();
            case 0b1111_1101:
                return getPlainTile();
            case 0b1111_1110:
                return getPlainTile();
            case 0b1111_1111:
                return getPlainTile();
            default:
                Debug.LogError("Tile value not found: " + tileValue);
                return errorTile;

        }
    }

    public TileBase GetObjectTile()
    {
        return objectTiles[Random.Range(0, objectTiles.Count)];
    }
}
