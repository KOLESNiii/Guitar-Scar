using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapPainter : MonoBehaviour
{
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
    float objectDensity = 0.05f;
    public void PaintFloorTiles(IEnumerable<Vector2Int> positions)
    {
        PaintTiles(positions, floorTilemap, floorTile);
    }

    public void PaintFloorTiles(ushort[,] tileValues, Vector2Int offset)
    {
        for (int x = 0; x < tileValues.GetLength(0); x++)
        {
            for (int y = 0; y < tileValues.GetLength(1); y++)
            {
                var tile = painterHelper.GetTile(tileValues[x, y]);
                var tilemap = painterHelper.floorTiles.Contains(tile) || painterHelper.emptyTile == tile ? floorTilemap : wallTilemap;
                PaintSingleTile(new Vector2Int(x+offset.x, y+offset.y), tilemap, tile);
            }
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(position, tilemap, tile);
        }
    }

    private void PaintSingleTile(Vector2Int position, Tilemap tilemap, TileBase tile)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void PaintObjectTiles(IEnumerable<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            if (Random.Range(0,1f) < objectDensity)
            {
                PaintSingleTile(position, objectTilemap, painterHelper.GetObjectTile());
            }
        }
    }
}
