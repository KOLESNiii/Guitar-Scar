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
        int maxRooms = Random.Range(7,8);
        while (NumAttempts < 1000 && grid.rooms.Count < maxRooms)
        {
            var room = new Room(MinWidth, MaxWidth, MinHeight, MaxHeight, Proportion, AreaMin, NumPasses);
            grid.addRoom(room);
            NumAttempts++;
        }
    }
}
public class Grid
{
    public List<Room> rooms;
    private int RoomGap;
    private int NumAttempts;
    public Grid(int roomGap = 2, int numAttempts = 1000)
    {
        rooms = new List<Room>();
        RoomGap = roomGap;
        NumAttempts = numAttempts;
    }

    private bool doesOverlap(Rect rect)
    {
        foreach (Room room in rooms)
        {
            if (rect.Overlaps(room.rect))
            {
                return true;
            }
        }
        return false;
    }

    public bool addRoom(Room room)
    {
        if (room.isEmpty)
        {
            return false;
        }
        room.makeRectangle();
        if (rooms.Count == 0)
        {
            room.rect.center = new Vector2Int(0,0);
            rooms.Add(room);
        }
        else
        {
            room.rect.center = new Vector2Int(0,0);
            var angle = Random.Range(0f, 360f) / 180f * Mathf.PI;
            var movement = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 4;
            movement = new Vector2Int((int)movement.x, (int)movement.y);
            bool valid = true;
            while (!valid)
            {
                room.rect.center += movement;
                if (!doesOverlap(room.rect))
                {
                    valid = true;
                }
            }
            rooms.Add(room);
        }
        return true;
        
    }
}

public class Room
{
    private List<List<bool>> grid;
    private float Proportion;
    private int AreaMin;
    private int NumPasses;
    private int MinWidth;
    private int MaxWidth;
    private int MinHeight;
    private int MaxHeight;
    private int Width;
    private int Height;
    public bool isEmpty = false;
    public Rect rect;
    
    public Room(int minWidth, int maxWidth, int minHeight, int maxHeight, float proportion, int areaMin, int numPasses)
    {
        MinWidth = minWidth;
        MaxWidth = maxWidth;
        MinHeight = minHeight;
        MaxHeight = maxHeight;
        Proportion = proportion;
        AreaMin = areaMin;
        NumPasses = numPasses;
        grid = new List<List<bool>>();
        var endLoop = false;
        int tryCount = 0;
        while (!endLoop && tryCount < 100)
        {
            Width = Random.Range(MinWidth, MaxWidth);
            Height = Random.Range(MinHeight, MaxHeight);
            createRoom();
            for (int i = 0; i < NumPasses; i++)
            {
                cellularAutomataPass();
            }
            makeRectangle();
            if (getLargestRegion() && rect.width >= MinWidth && rect.height >= MinHeight)
            {
                getLargestRegion(target:false);
                endLoop = true;
            }
            tryCount++;
        }
        if (endLoop == false)
        {
            Debug.Log("Failed to create room");
            isEmpty = true;
            rect = new Rect(0,0,0,0);
        }
        makeRectangle();
    }

    private void createRoom()
    {
        for (int x = 0; x < Width; x++)
        {
            grid.Add(Enumerable.Repeat(false, Height).ToList());
        }
        for (int x = 2; x < Width-2; x++)
        {
            for (int y = 2; y < Height-2; y++)
            {
                grid[x][y] = Random.Range(0f, 1f) < Proportion ? true : false;
            }
        }
    }

    private void cellularAutomataPass()
    {
        var newGrid = new List<List<bool>>();
        for (int x = 0; x < grid.Count; x++)
        {
            newGrid.Add(Enumerable.Repeat(false, grid[0].Count).ToList());
        }
        for (int x = 1; x < grid.Count-1; x++)
        {
            for (int y = 1; y < grid[0].Count-1; y++)
            {
                var count = 0;
                var adjacents = getAdjacents(x, y);
                foreach (Vector2Int adjacent in adjacents)
                {
                    if (grid[adjacent.x][adjacent.y])
                    {
                        count++;
                    }
                }
                newGrid[x][y] = count >= 4 ? true : false;
            }
        }
        grid = newGrid;
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
                if (grid[x][y] == target)
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
                            grid[tile.x][tile.y] = !target;
                            var neighbours = getAdjacents(tile.x, tile.y, diagonals:false);
                            foreach (Vector2Int neighbour in neighbours)
                            {
                                if (grid[neighbour.x][neighbour.y] == target)
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
            grid[tile.x][tile.y] = target;
        }

        if (largestRegion.Count < AreaMin)
        {
            return false;
        }
        return true;
    }

    public List<List<bool>> getTightRectangle(int width = 0)
    {
        makeRectangle();
        var gridCopy = new List<List<bool>>();
        for (int x = 0; x < rect.width+2*width; x++) 
        {
            gridCopy.Add(Enumerable.Repeat(false, (int)rect.height+2*width).ToList());
        }
        for (int x = 0; x < rect.width; x++)
        {
            for (int y = 0; y < rect.height; y++)
            {
                gridCopy[x+width][y+width] = grid[x+(int)rect.x][y+(int)rect.y];
            }
        }
        return grid;
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
    public void makeRectangle()
    {
        int minX = 0;
        int minY = 0;
        int maxX = Width - 1;
        int maxY = Height -1;
        try
        {
            while (sum(grid[minX]) == 0)
            {
                minX++;
            }
            while (sum(grid[maxX]) == 0)
            {
                maxX --;
            }
            while (sum(getRow(grid, minY)) == 0)
            {
                minY ++;
            }
            while (sum(getRow(grid, maxY)) == 0)
            {
                maxY --;
            }
        }
        catch
        {
            rect = new Rect(0,0,0,0);
            return;
        }
        rect = new Rect((int)minX, (int)minY, (int)maxX - minX, (int)maxY - minY);
        return;
    }
}
