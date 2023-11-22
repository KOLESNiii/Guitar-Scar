using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    [SerializeField]
    private float Proportion = 0.43f;
    [SerializeField]
    private int MaxWidth = 100;
    [SerializeField]
    private int MaxHeight = 100;
    [SerializeField]
    private int MinWidth = 10;
    [SerializeField]
    private int MinHeight = 10;
    [SerializeField]
    private int NumPasses = 7;
    [SerializeField]
    private int NumAttempts = 1000;
    private int AreaMin;
    [SerializeField]
    private int RoomGap = 2;


    public DungeonGen()
    {
        AreaMin = (int)(MinWidth * MinHeight * 2.5);
    }

    public void runGeneration()
    {
        var grid = new Grid(RoomGap, NumAttempts);
        int maxRooms = UnityEngine.Random.Range(7,8);
        while (NumAttempts < 1000 && grid.Rooms.Count < maxRooms)
        {
            var room = new Room(MinWidth, MaxWidth, MinHeight, MaxHeight, Proportion, AreaMin, NumPasses);
            grid.addRoom(room);
            NumAttempts++;
        }
        grid.updateHashSetOfTiles();
        foreach (var tile in grid.Tiles)
        {
            Console.WriteLine($"{tile.x}, {tile.y}");
        }
    }
}
public class Grid
{
    public List<Room> Rooms;
    private int RoomGap;
    private int NumAttempts;
    public HashSet<Vector2Int> Tiles;
    public Grid(int roomGap = 2, int numAttempts = 1000)
    {
        Rooms = new List<Room>();
        RoomGap = roomGap;
        NumAttempts = numAttempts;
        Tiles = new HashSet<Vector2Int>{};
    }

    private bool doesOverlap(Rect rect)
    {
        foreach (Room room in Rooms)
        {
            if (rect.Overlaps(room.Rect))
            {
                return true;
            }
        }
        return false;
    }

    public bool addRoom(Room room)
    {
        if (room.IsEmpty)
        {
            return false;
        }
        room.makeRectangle();
        if (Rooms.Count == 0)
        {
            room.Rect.center = new Vector2Int(0,0);
            Rooms.Add(room);
        }
        else
        {
            room.Rect.center = new Vector2Int(0,0);
            var angle = UnityEngine.Random.Range(0f, 360f) / 180f * Mathf.PI;
            var movement = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 4;
            movement = new Vector2Int((int)movement.x, (int)movement.y);
            bool valid = true;
            while (!valid)
            {
                room.Rect.center += movement;
                if (!doesOverlap(room.Rect))
                {
                    valid = true;
                }
            }
            Rooms.Add(room);
        }
        return true; 
    }

    public void updateHashSetOfTiles()
    {
        foreach (Room room in Rooms)
        {
            for (int x = 0; x < room.Rect.width; x++)
            {
                for (int y = 0; y < room.Rect.height; y++)
                {
                    if (room.Grid[x+(int)room.Rect.x][y+(int)room.Rect.y])
                    {
                        Tiles.Add(new Vector2Int(x+(int)room.Rect.x, y+(int)room.Rect.y));
                    }
                }
            }
        }
    }
}

public class Room
{
    public List<List<bool>> Grid
    {private set; get;}
    private float Proportion;
    private int AreaMin;
    private int NumPasses;
    private int MinWidth;
    private int MaxWidth;
    private int MinHeight;
    private int MaxHeight;
    private int Width;
    private int Height;
    public bool IsEmpty = false;
    public Rect Rect;
    
    public Room(int minWidth, int maxWidth, int minHeight, int maxHeight, float proportion, int areaMin, int numPasses)
    {
        MinWidth = minWidth;
        MaxWidth = maxWidth;
        MinHeight = minHeight;
        MaxHeight = maxHeight;
        Proportion = proportion;
        AreaMin = areaMin;
        NumPasses = numPasses;
        Grid = new List<List<bool>>();
        var endLoop = false;
        int tryCount = 0;
        while (!endLoop && tryCount < 100)
        {
            Width = UnityEngine.Random.Range(MinWidth, MaxWidth);
            Height = UnityEngine.Random.Range(MinHeight, MaxHeight);
            createRoom();
            for (int i = 0; i < NumPasses; i++)
            {
                cellularAutomataPass();
            }
            if (getLargestRegion())
            {
                makeRectangle();
                if (Rect.width >= MinWidth && Rect.height >= MinHeight)
                {
                    getLargestRegion(target:false);
                    endLoop = true;
                }
            }
            tryCount++;
        }
        if (endLoop == false)
        {
            Debug.Log("Failed to create room");
            IsEmpty = true;
            Rect = new Rect(0,0,0,0);
        }
        makeRectangle();
        setTightRectangleGrid();
    }

    private void createRoom()
    {
        for (int x = 0; x < Width; x++)
        {
            Grid.Add(Enumerable.Repeat(false, Height).ToList());
        }
        for (int x = 2; x < Width-2; x++)
        {
            for (int y = 2; y < Height-2; y++)
            {
                Grid[x][y] = UnityEngine.Random.Range(0f, 1f) < Proportion ? true : false;
            }
        }
    }

    private void cellularAutomataPass()
    {
        var newGrid = new List<List<bool>>();
        for (int x = 0; x < Grid.Count; x++)
        {
            newGrid.Add(Enumerable.Repeat(false, Grid[0].Count).ToList());
        }
        for (int x = 1; x < Grid.Count-1; x++)
        {
            for (int y = 1; y < Grid[0].Count-1; y++)
            {
                var count = 0;
                var adjacents = getAdjacents(x, y);
                foreach (Vector2Int adjacent in adjacents)
                {
                    if (Grid[adjacent.x][adjacent.y])
                    {
                        count++;
                    }
                }
                newGrid[x][y] = count >= 4 ? true : false;
            }
        }
        Grid = newGrid;
    }

    private List<Vector2Int> getAdjacents(int x, int y, bool diagonals = true)
    {
        var adjacents = new List<Vector2Int>();
        int minX = 0;
        int minY = 0;
        int maxX = Width - 1;
        int maxY = Height - 1;
        if (x != minX)
        {
            adjacents.Add(new Vector2Int(x - 1, y));
        }
        if (x != maxX)
        {
            adjacents.Add(new Vector2Int(x + 1, y));
        }
        if (y != minY)
        {
            adjacents.Add(new Vector2Int(x, y - 1));
        }
        if (y != maxY)
        {
            adjacents.Add(new Vector2Int(x, y + 1));
        }
        if (diagonals)
        {
            if (x != minX && y != minY)
            {
                adjacents.Add(new Vector2Int(x - 1, y - 1));
            }
            if (x != minX && y != maxY)
            {
                adjacents.Add(new Vector2Int(x - 1, y + 1));
            }
            if (x != maxX && y != minY)
            {
                adjacents.Add(new Vector2Int(x + 1, y - 1));
            }
            if (x != maxX && y != maxY)
            {
                adjacents.Add(new Vector2Int(x + 1, y + 1));
            }
        }
        return adjacents;
    }

    private bool getLargestRegion(bool target = true)
    {
        HashSet<Vector2Int> largestRegion = new HashSet<Vector2Int>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Grid[x][y] == target)
                {
                    HashSet<Vector2Int> newRegion = new HashSet<Vector2Int>();
                    Vector2Int tile = new Vector2Int(x, y);
                    var toBeFilled = new Queue<Vector2Int>();
                    toBeFilled.Enqueue(tile);
                    while (toBeFilled.TryDequeue(out tile))
                    {
                        if (!newRegion.Contains(tile))
                        {
                            newRegion.Add(tile);
                            Grid[tile.x][tile.y] = !target;
                            var neighbours = getAdjacents(tile.x, tile.y, diagonals:false);
                            foreach (Vector2Int neighbour in neighbours)
                            {
                                if (Grid[neighbour.x][neighbour.y] == target)
                                {
                                    if (!toBeFilled.Contains(neighbour) && !newRegion.Contains(neighbour))
                                    {
                                        toBeFilled.Enqueue(neighbour);
                                    }
                                }
                            }
                        }
                    }
                    if (newRegion.Count > largestRegion.Count)
                    {
                        largestRegion.Clear();
                        largestRegion.UnionWith(newRegion);
                    }
                }
            }
        }
        foreach (Vector2Int tile in largestRegion)
        {
            Grid[tile.x][tile.y] = target;
        }

        if (largestRegion.Count < AreaMin)
        {
            return false;
        }
        return true;
    }

    public void setTightRectangleGrid(int gap = 2)
    {
        makeRectangle();
        var gridCopy = new List<List<bool>>();
        for (int x = 0; x < Rect.width; x++) 
        {
            gridCopy.Add(Enumerable.Repeat(false, (int)Rect.height).ToList());
        }
        for (int x = 0; x < Rect.width; x++)
        {
            for (int y = 0; y < Rect.height; y++)
            {
                gridCopy[x][y] = Grid[x+(int)Rect.x][y+(int)Rect.y];
            }
        }
        Grid = gridCopy;
        return;
    }

    private int sum(List<bool> source)
    {
        int count = source.Where(x => x == true).Count();
        return count;
    }
    private List<bool> getRow(List<List<bool>> source, int rowNumber)
    {
        return Enumerable.Range(0, source[0].Count).Select(x => source[x][rowNumber]).ToList();
    }
    public void makeRectangle(int gap = 2)
    {
        int minX = 0;
        int minY = 0;
        int maxX = Width - 1;
        int maxY = Height -1;
        try
        {
            while (sum(Grid[minX]) == 0)
            {
                minX++;
            }
            while (sum(Grid[maxX]) == 0)
            {
                maxX --;
            }
            while (sum(getRow(Grid, minY)) == 0)
            {
                minY ++;
            }
            while (sum(getRow(Grid, maxY)) == 0)
            {
                maxY --;
            }
        }
        catch
        {
            Rect = new Rect(0,0,0,0);
            return;
        }
        Rect = new Rect(minX-gap, minY-gap, maxX - minX + 2*gap, maxY - minY+2*gap);
        return;
    }
}
