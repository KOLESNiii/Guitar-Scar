using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    // Start is called before the first frame update
    private Texture2D texture;
    public SpriteRenderer sr;
    private Vector2Int WorldSize;
    private Vector2Int TextureSize;
    private Vector2Int pixelScale;
    private Vector2 worldScale;
    public void GenerateTexture(Vector3 cellSize, Vector2Int gridSize, Vector2Int coordinates)
    {
        sr = GetComponent<SpriteRenderer>();
        Vector2Int IntCellSize = new Vector2Int((int)cellSize.x, (int)cellSize.y);
        Vector2Int newCellSize = new Vector2Int((int)cellSize.x/2, (int)cellSize.y/2);
        WorldSize = gridSize * IntCellSize;
        gridSize = gridSize * 2;
        TextureSize = gridSize;
        texture = new Texture2D(gridSize.x, gridSize.y);
        Color[] colors = new Color[gridSize.x * gridSize.y];
        for (int i = 0; i < gridSize.x * gridSize.y; i++)
        {
            colors[i] = Color.black;
        }
        texture.SetPixels(colors);
    }

    public void MakeHole(Vector2 position, float holeRadius) {
        Vector2Int pixelPosition = WorldToTexture(position);
        int radius = Mathf.RoundToInt(holeRadius * pixelScale.x / worldScale.x);
        int px, nx, py, ny, distance;
        for (int i = 0; i < radius; i++) {
            distance = Mathf.RoundToInt(Mathf.Sqrt(radius * radius - i * i));
            for (int j = 0; j < distance; j++) {
                px = Mathf.Clamp(pixelPosition.x + i, 0, pixelScale.x);
                nx = Mathf.Clamp(pixelPosition.x - i, 0, pixelScale.x);
                py = Mathf.Clamp(pixelPosition.y + j, 0, pixelScale.y);
                ny = Mathf.Clamp(pixelPosition.y - j, 0, pixelScale.y);

                texture.SetPixel(px, py, Color.black);
                texture.SetPixel(nx, py, Color.black);
                texture.SetPixel(px, ny, Color.black);
                texture.SetPixel(nx, ny, Color.black);
            }
        }
        texture.Apply();
        CreateSprite();
    }

    private void CreateSprite() {
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * .5f, 100);
    }

    private Vector2Int WorldToTexture(Vector2 worldPosition)
    {
        Vector2Int PixelPosition = Vector2Int.zero;
        float dx = worldPosition.x - transform.position.x;
        float dy = worldPosition.y - transform.position.y;

        PixelPosition.x = Mathf.RoundToInt(.5f * pixelScale.x + dx * (pixelScale.x / worldScale.x));
        PixelPosition.y = Mathf.RoundToInt(.5f * pixelScale.y + dy * (pixelScale.y / worldScale.y));
        return PixelPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
