using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Class for painting tiles to tilemap
public class TilemapPainter : MonoBehaviour
{
    //Tilemap variables assigned in unity editor
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private Tilemap wallTilemap;
    [SerializeField]
    private Tilemap objectTilemap;
    [SerializeField]
    private TileBase floorTile;
    [SerializeField]
    private PainterHelper painterHelper;
    [SerializeField]
    float objectDensity = 0.05f; //Density of objects, 0 = no objects, 1 = all tiles are objects
    public void PaintFloorTiles(IEnumerable<Vector2Int> positions)
    {
        PaintTiles(positions, floorTilemap, floorTile);
    }
    //Paints floor tiles based off the tile values (orientation and identity) and offset
    public void PaintFloorTiles(ushort[,] tileValues, Vector2Int offset)
    {
        for (int x = 0; x < tileValues.GetLength(0); x++)
        {
            for (int y = 0; y < tileValues.GetLength(1); y++)
            {
                var tile = painterHelper.GetTile(tileValues[x, y]); //Gets tile from tile value
                //If the tile is a floor tile or empty tile, paint it to the floor tilemap, otherwise paint it to the wall tilemap
                var tilemap = painterHelper.floorTiles.Contains(tile) || painterHelper.emptyTile == tile ? floorTilemap : wallTilemap;
                PaintSingleTile(new Vector2Int(x+offset.x, y+offset.y), tilemap, tile); //Paints tile
            }
        }
    }

    //Paints a single type of tile to a tilemap
    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(position, tilemap, tile);
        }
    }

    //Paints a single tile to a tilemap
    private void PaintSingleTile(Vector2Int position, Tilemap tilemap, TileBase tile)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    //Paints object tiles on the random positions
    public void PaintObjectTiles(IEnumerable<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            if (Random.Range(0,1f) < objectDensity) //If random value is less than object density, paint object tile
            {
                PaintSingleTile(position, objectTilemap, painterHelper.GetObjectTile());
            }
        }
    }
}
