using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGen : MonoBehaviour
{
    [SerializeField]
    private float Proportion = 0.43f; //Proportion of tiles that are filled in initially before cellular automata
    [SerializeField]
    private int MaxWidth = 100; //Maximum width of a room
    [SerializeField]
    private int MaxHeight = 100; //Maximum height of a room
    [SerializeField]
    private int MinWidth = 10; //Minimum width of a room
    [SerializeField]
    private int MinHeight = 10; //Minimum height of a room
    [SerializeField]
    private int NumPasses = 7; //Number of passes of cellular automata
    [SerializeField]
    private int NumAttempts = 1000; //Number of attempts to create a room
    [SerializeField]
    private int WidthCorridors = 3; //Width of corridors
    [SerializeField]
    private int MinRooms = 7; //Minimum number of rooms
    [SerializeField]
    private int MaxRooms = 9; //Maximum number of rooms
    private int AreaMin; //Minimum area of a room

    [SerializeField]
    private TilemapPainter tilemapPainter; //Tilemap painter to paint tiles

    [SerializeField]
    private Triangulator triangulator; //Triangulator to triangulate rooms
    [SerializeField]
    private GameObject Portal; //Portal prefab
    [SerializeField]
    private GameObject Spawner; //Spawner prefab
    public enum RoomType
    {
        Entrance,
        Exit,
        Enemy,
        Loot,
        StrongEnemy
    }

    List<Room> Rooms;
    //Start is called before the first frame update
    void Start()
    {
        CurrentLevel.Instance.SetEnvironment(new Environment(new int[]{0,1,2,3,4,5,6,7,8})); //Sets the enemy types that can be encountered
        AreaMin = (int)(MinWidth * MinHeight * 2.5); //Sets the minimum area of a room
        if (MinHeight > MaxHeight) //Validation of unity editor variables
        {
            throw new Exception("MinHeight cannot be greater than MaxHeight");
        }
        if (MinWidth > MaxWidth)
        {
            throw new Exception("MinWidth cannot be greater than MaxWidth");
        }
        if (NumPasses < 0)
        {
            throw new Exception("NumPasses cannot be less than 0");
        }
        if (Proportion < 0 || Proportion > 1)
        {
            throw new Exception("Proportion must be between 0 and 1");
        }
        if (WidthCorridors < 0)
        {
            throw new Exception("WidthCorridors cannot be less than 0");
        }
        runGeneration(); //Runs the dungeon generation
    }

    //Generates the dungeon fully
    public void runGeneration()
    {
        var grid = new Grid();
        int maxRooms = UnityEngine.Random.Range(MinRooms,MaxRooms); //Randomly sets number of rooms, as this number is highly likely to be reached
        int attempts = 0;
        while (attempts < NumAttempts && grid.Rooms.Count < maxRooms) //Attempts to create rooms
        {
            var room = new Room(MinWidth, MaxWidth, MinHeight, MaxHeight, Proportion, AreaMin, NumPasses); //Creates a room
            if (!room.centreIsFilled()) //If the centre of the room is not filled, the room is not valid
            {
                attempts++;
                continue;
            }
            grid.addRoom(room); //Adds the room to the grid
            attempts++;
        }
        grid.updateHashSetOfTiles(); //Updates the hashset of tiles
        triangulator.Initialise(grid.getCentrePoints()); //Initialises the triangulator with the room centrepoints
        Graph graph = new Graph();  //Initialises the graph
        triangulator.Triangulate(graph.AppendToGraph); //Triangulates the room centrepoints and adds the edges to the graph
        graph.PerformPrims(); //Performs Prim's algorithm on the graph
        graph.ReintroduceSomeRemovedEdges(); //Reintroduces some edges that were removed by Prim's algorithm
        grid.generateCorridors(graph); //Generates corridors
        var rectAllGrid = grid.getMaxGridSize(tiles); //Gets the maximum grid size
        rectAllGrid = grid.padGrid(rectAllGrid, 8); //Pads the grid with empty tiles, so edge of world cannot be seen
        var valueArray = generateValueArray(grid.Tiles, rectAllGrid); //Gets tile values to orient the 2.5D tiles
        //tilemapPainter.PaintFloorTiles(valueArray, new Vector2Int((int)rectAllGrid.x, (int)rectAllGrid.y)); //Paints the floor tiles
        //tilemapPainter.PaintObjectTiles(grid.Tiles); //Adds objects to the tilemap
        //setRoomTypes(grid); //Sets the room types
        //finishRoomAssignment(); //Finishes the room assignment
    }
    //Assigns room types
    private void setRoomTypes(Grid grid)
    {
        Debug.Log("Start room assignment");
        var tempRooms = grid.Rooms;
        var OutputRooms = new List<Room>();
        //First sets the entry and exit rooms
        var EntryRoom = tempRooms.Find(x => x.Rect.center == new Vector2Int(0,0));
        EntryRoom.Type = RoomType.Entrance;
        tempRooms.Remove(EntryRoom);
        var ExitRoom = tempRooms.OrderBy(x => x.Rect.center.magnitude).Last();
        ExitRoom.Type = RoomType.Exit;
        tempRooms.Remove(ExitRoom);
        OutputRooms.Add(EntryRoom);
        OutputRooms.Add(ExitRoom);
        int numEnemyRooms = 0;
        int desiredEnemyRooms = Math.Min(4, (int)(0.66*tempRooms.Count)); //Sets the minimum number of enemy rooms
        foreach (Room room in tempRooms)
        {
            if (numEnemyRooms < desiredEnemyRooms)
            {
                room.Type = RoomType.Enemy; //Sets the room type to enemy if the number of enemy rooms is less than minimum
                numEnemyRooms ++;
            }
            else //random assignment if enemy room quota reached
            {
                room.Type = (RoomType)UnityEngine.Random.Range(2,5); 
            }
            OutputRooms.Add(room);
        }
        foreach (Room room in OutputRooms)
        {
            Debug.Log("Room type:" + room.Type); //Debugging
        }
        Rooms = OutputRooms;
    }

    //Finishes the room assignment by acting upon the room roles
    private void finishRoomAssignment()
    {
        Vector3 startRoomLocation = Vector3.zero;
        foreach (Room room in Rooms)
        {
            Debug.Log("Room type:" + room.Type);
            Vector3 location = room.Rect.center; //as this must be filled in, this is where things are spawned
            EnemyTypeManager enemyTypeManager = GameObject.Find("EnemyTypeManager").GetComponent<EnemyTypeManager>();
            if (room.Type == RoomType.Entrance) //leaves this till the end
            {
                startRoomLocation = location;
            }
            else if (room.Type == RoomType.Exit) //Places portal at exit and sets it to be an exit, to prevent automatic deletion
            {
                GameObject portal = Instantiate(Portal, location, Quaternion.identity);
                Debug.Log("Placed portal, location: " + portal.transform.position.ToString());
                portal.GetComponent<Portal>().isExit = true;
            }
            else if (room.Type == RoomType.Enemy && room.Type == RoomType.StrongEnemy) //Places spawner object at enemy room, which spawns enemies
            {
                GameObject spawner = Instantiate(Spawner, location, Quaternion.identity);
                Spawner spawnerScript = spawner.GetComponent<Spawner>();
                //Sets the possible enemy types that could be spawned
                var PossibleEnemyTypes = CurrentLevel.Instance.Environment.PossibleEnemyTypes.Select(x => x.GetComponent<EnemyType>()).ToList();
                spawnerScript.PossibleEnemyTypes = PossibleEnemyTypes;
                spawnerScript.enemyTypeManager = enemyTypeManager;
                if (room.Type == RoomType.StrongEnemy) //Sets the spawner to spawn strong enemies if the room is a strong enemy room
                {
                    spawnerScript.isHardEnemy = true;
                }
                spawnerScript.SpawnEnemy(); //Triggers the spawner to spawn once fully initialised
            }
            else if (room.Type == RoomType.Loot) //Empty due to removal of feature, instead is an empty room
            {
                
            }
        }
        Instantiate(Portal, startRoomLocation, Quaternion.identity); //Places portal at start room
        Debug.Log("Created portal");
        Invoke("SpawnPlayer", 0.5f); //Spawns player after a delay
    }

    //Spawns the player
    private void SpawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f); //Makes the player visible
        player.transform.position = Vector3.zero;
        Player playerScript = player.GetComponent<Player>();
        playerScript.inBattle = false;
        playerScript.Turn(playerScript.calculateAngleTurned(0)); //Makes the player move forward out of the portal
        playerScript.Move();
    }

    //Generates the orientation values of each tile for 2.5D
    private ushort[,] generateValueArray(HashSet<Vector2Int> tiles, Rect grid)
    {
        var valueArray = new ushort[(int)grid.width, (int)grid.height]; //Initialises output array
        for (int x = 0; x < grid.width; x++)  //Iterates through each tile
        {
            for (int y = 0; y < grid.height; y++)
            {
                valueArray[x,y] = getValueFromAdjacents(tiles, x+(int)grid.x, y+(int)grid.y);
            }
        }
        return valueArray;
    }
    //Gets the orientation value of a tile based off neighbouring tiles
    private ushort getValueFromAdjacents(HashSet<Vector2Int> tiles, int x, int y)
    {
        if (tiles.Contains(new Vector2Int(x,y))) //If the tile is filled, it is a floor tile so does not need an orientation value
        {
            return 256;
        }
        ushort value = 0;
        //Gets the orientation value of wall depending on which neighboutring tiles are filled
        value += (ushort)(128 * getTileValue(tiles, x, y+1));
        value += (ushort)(64 * getTileValue(tiles, x+1, y+1));
        value += (ushort)(32 * getTileValue(tiles, x+1, y));
        value += (ushort)(16 * getTileValue(tiles, x+1, y-1));
        value += (ushort)(8 * getTileValue(tiles, x, y-1));
        value += (ushort)(4 * getTileValue(tiles, x-1, y-1));
        value += (ushort)(2 * getTileValue(tiles, x-1, y));
        value += getTileValue(tiles, x-1, y+1);
        return value;
    }
    //Returns one if tile is filled, 0 if not
    private ushort getTileValue(HashSet<Vector2Int> tiles, int x, int y)
    {
        if (tiles.Contains(new Vector2Int(x,y)))
        {
            return 1;
        }
        return 0;
    }
}
public class Grid
{
    public List<Room> Rooms; //List of rooms for the grid
    public HashSet<Vector2Int> Tiles;
    public Grid()
    {
        Rooms = new List<Room>();
        Tiles = new HashSet<Vector2Int>{};
    }
    //Checks if a rectangle overlaps with any existing rooms
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
    //Adds a room to the grid
    public bool addRoom(Room room)
    {
        if (room.IsEmpty) //Validation of room
        {
            return false;
        }
        //Makes the room a rectangle tight to the edges of the room
        room.makeRectangle();
        if (Rooms.Count == 0) //adds room to centre if it is the first room
        {
            room.Rect.center = new Vector2Int(0,0);
            Rooms.Add(room);
        }
        else
        {
            room.Rect.center = new Vector2Int(0,0);
            var angle = UnityEngine.Random.Range(0f, 360f) / 180f * Mathf.PI; //Randomly chooses an angle to move the room
            var movement = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * UnityEngine.Random.Range(2,11); //Randomly chooses a distance to move the room
            bool valid = false;
            while (!valid) //Moves the room further from centre until it does not overlap with any other rooms
            {
                room.Rect.center += movement;
                var tempRect = room.Rect;
                tempRect.center = new Vector2Int((int)tempRect.x, (int)tempRect.y);
                if (!doesOverlap(tempRect))
                {
                    valid = true;
                }
            }
            room.Rect.center = new Vector2Int((int)room.Rect.x, (int)room.Rect.y);
            Rooms.Add(room);
        }
        return true; 
    }
    //Updates the hashset of tiles
    public void updateHashSetOfTiles()
    {
        foreach (Room room in Rooms)
        {
            for (int x = 0; x < room.Rect.width; x++)
            {
                for (int y = 0; y < room.Rect.height; y++)
                {
                    if (room.Grid[x][y])
                    {
                        Tiles.Add(new Vector2Int(x+(int)room.Rect.x, y+(int)room.Rect.y));
                    }
                }
            }
        }
    }
    //Returns the centre points of all rooms
    public HashSet<Vector2Int> getCentrePoints()
    {
        var output = new HashSet<Vector2Int>();
        foreach (Room room in Rooms)
        {
            output.Add(new Vector2Int((int)room.Rect.center.x, (int)room.Rect.center.y));
        }
        return output;
    }
    public void generateCorridors(Graph graph)
    {
        var corridors = graph.SpanningTreeEdges.Select(x => (new Vector2Int(x.Points[0].x, x.Points[0].y), new Vector2Int(x.Points[1].x, x.Points[1].y))).ToHashSet();
        //Converts the edges of the graph to a hashset of tiles
        foreach (var corridor in corridors) //Iterates through all corridors to widen them
        {
            for (int i = 0; i < WidthCorridors; i++)
            {
                int offset = i == 0 ? 0 : i % 2 == 0 ? -((i+1)/2) : (i+1)/2;
                Tiles.UnionWith(lineGenerator(OffsetVector2Int(corridor.Item1, corridor.Item2, offset)));
            }
        }
    }
    //Wrapper for lineGenerator function to accept tuples
    private IEnumerable<Vector2Int> lineGenerator((Vector2Int ,Vector2Int) vectors)
    {
        return lineGenerator(vectors.Item1, vectors.Item2);
    }
    //Generator for an enumerable of tiles for a line between two given points, using discrete ray tracing algorithm
    private IEnumerable<Vector2Int> lineGenerator(Vector2Int point1, Vector2Int point2)
    {
        int x0 = point1.x;
        int x1 = point2.x;
        int y0 = point1.y;
        int y1 = point2.y;
        double tDeltaX; //Amount to move in x per unit t
        double tDeltaY; //Amount to move in y per unit t
        double tMaxX; //Amount of t to move before next vertical tile boundary
        double tMaxY; //Amount of t to move before next horizontal tile boundary
        var dx = x1 - x0;
        var dy = y1 - y0;
        dx = (dx > 0) ? 1 : (dx < 0) ? -1 : 0; //Gets the direction of the line in x and y
        dy = (dy > 0) ? 1 : (dy < 0) ? -1 : 0;
        x1 += dx;
        y1 += dy;
        if (dx != 0) 
        {
            tDeltaX = Math.Min((double)dx/(x1 - x0), double.MaxValue);
        }
        else
        {
            tDeltaX = double.MaxValue; //If dx is 0, set tDeltaX to be very large
        }
        if (dx > 0)
        {
            tMaxX = tDeltaX;
        }
        else
        {
            tMaxX = 0;
        }
        if (dy != 0)
        {
            tDeltaY = Math.Min((double)dy/(y1 - y0), double.MaxValue);
        }
        else
        {
            tDeltaY = double.MaxValue;
        }
        if (dy > 0)
        {
            tMaxY = tDeltaY;
        }
        else
        {
            tMaxY = 0;
        }
        int i = 0;
        while (true)
        {
            yield return new Vector2Int(x0, y0);
            if (tMaxX < tMaxY) //if closer to vertical boundary, move in x
            {
                tMaxX += tDeltaX;
                x0 += dx;
            }
            else //if closer to horizontal boundary, move in y
            {
                tMaxY += tDeltaY;
                y0 += dy;
            }
            if (tMaxX > 1 && tMaxY > 1) //if both tMaxX and tMaxY are greater than 1, the line has reached the end
            {
                yield break;
            }
            i ++;
            if (i > 10000) //If the line has not reached the end after 10000 iterations, it is likely in an infinite loop, for this implementation
            {
                Debug.Log("tDeltaX: " + tDeltaX.ToString() + ", tDeltaY: " + tDeltaY.ToString() + ", tMaxX: " + tMaxX.ToString() + ", tMaxY: " + tMaxY.ToString() + ", x0: " + x0.ToString() + ", y0: " + y0.ToString() + ", dx: " + dx.ToString() + ", dy: " + dy.ToString());
                throw new Exception("Infinite loop");
            }
        }
    }

    //Offsets a line by a given offset value, vector1 and vector2 are the start and end points of the line
    private(Vector2Int, Vector2Int) OffsetVector2Int(Vector2Int vector1, Vector2Int vector2, int offset)
    {
        if (Vector2.Angle(vector2-vector1, Vector2.right) < 45f) //If the line is horizontal, add offset to y
        {
            return (new Vector2Int(vector1.x, vector1.y + offset), new Vector2Int(vector2.x, vector2.y + offset));
        } //If the line is vertical, add offset to x
        return (new Vector2Int(vector1.x + offset, vector1.y), new Vector2Int(vector2.x + offset, vector2.y));
    }
    //Gets the maximum grid size
    public Rect getMaxGridSize()
    {
        int maxX = Tiles.Select(tile => tile.x).Max();
        int minX = Tiles.Select(tile => tile.x).Min();
        int maxY = Tiles.Select(tile => tile.y).Max();
        int minY = Tiles.Select(tile => tile.y).Min();
        return new Rect(minX, minY, maxX-minX, maxY-minY);
    }
    //Pads the grid with empty tiles
    public Rect padGrid(Rect grid, int padding)
    {
        return new Rect(grid.x - padding, grid.y - padding, grid.width + 2*padding, grid.height + 2*padding);
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
    public DungeonGen.RoomType Type;
    
    //Constructor for room, initialises with variables for room generation
    public Room(int minWidth, int maxWidth, int minHeight, int maxHeight, float proportion, int areaMin, int numPasses)
    {
        MinWidth = minWidth;
        MaxWidth = maxWidth;
        MinHeight = minHeight;
        MaxHeight = maxHeight;
        Proportion = proportion;
        AreaMin = areaMin;
        NumPasses = numPasses;
        var endLoop = false;
        int tryCount = 0;
        while (!endLoop && tryCount < 100) //Attempts to create a room
        {
            Width = UnityEngine.Random.Range(MinWidth, MaxWidth);
            Height = UnityEngine.Random.Range(MinHeight, MaxHeight);
            createRoom();
            for (int i = 0; i < NumPasses; i++) //Performs cellular automata passes
            {
                cellularAutomataPass();
            }
            if (getLargestRegion()) //Gets the largest region of the room while verifying it is large enough
            {
                makeRectangle(); //Makes the room a tight rectangle
                if (Rect.width >= MinWidth && Rect.height >= MinHeight) //Verifies the room meets the minimum size requirements
                {
                    getLargestRegion(target:false); //Fills in the largest region of the room
                    endLoop = true;
                }
            }
            tryCount++;
        }
        if (endLoop == false) //If the room cannot be created, it is set to be empty
        {
            Debug.LogWarning("Failed to create room");
            IsEmpty = true;
            Rect = new Rect(0,0,0,0);
        }
        setTightRectangleGrid(); 
    }
    //Checks if the centre of the room is filled
    public bool centreIsFilled()
    {
        return Grid[(int)Grid.Count/2][(int)Grid[0].Count/2];
    }
    //Creates a room
    private void createRoom()
    {
        Grid = new List<List<bool>>(); //Initialises the grid list
        for (int x = 0; x < Width; x++)
        {
            Grid.Add(Enumerable.Repeat(false, Height).ToList()); //fills with values indicating not filled
        }
        for (int x = 2; x < Width-2; x++) //with offsets to ensure cellular automata does not go out of bounds
        {
            for (int y = 2; y < Height-2; y++)
            {
                Grid[x][y] = UnityEngine.Random.Range(0f, 1f) < Proportion ? true : false; //Populates tiles randomly
            }   
        }
    }
    //Performs one pass in-place using cellular automata
    private void cellularAutomataPass()
    {
        var newGrid = new List<List<bool>>(); //Initialises new grid, to ensure cellular automata values are not overwritten
        for (int x = 0; x < Grid.Count; x++)
        {
            newGrid.Add(Enumerable.Repeat(false, Grid[0].Count).ToList()); //fills with values indicating not filled
        }
        for (int x = 1; x < Grid.Count-1; x++) //iterates with offsets to ensure cellular automata does not go out of bounds
        {
            for (int y = 1; y < Grid[0].Count-1; y++)
            {
                var count = 0;
                var adjacents = getAdjacents(x, y); //Gets all adjacent tiles
                foreach (Vector2Int adjacent in adjacents)
                {
                    if (Grid[adjacent.x][adjacent.y]) //Counts the number of adjacent tiles that are filled
                    {
                        count++;
                    }
                }
                newGrid[x][y] = count >= 4 ? true : false; //Sets the new value of the tile to true if the tile has 4 or more adjacents, else false
            }
        }
        copyToGrid(newGrid); //Copies the new grid to the old grid
    }
    //Gets all adjacent tiles to a given tile, with an option to include diagonals
    private List<Vector2Int> getAdjacents(int x, int y, bool diagonals = true)
    {
        var adjacents = new List<Vector2Int>();
        int minX = 0; //Specifies maximum and minimum values, to ensure there is not an out of bounds error
        int minY = 0;
        int maxX = Width - 1;
        int maxY = Height - 1;
        //Gets directly adjacent tiles
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
        if (diagonals) //Gets diagonal tiles if specified
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
    //Gets the largest region of the room, with an option to fill the largest region if target==true, else removes islands
    private bool getLargestRegion(bool target = true)
    {
        HashSet<Vector2Int> largestRegion = new HashSet<Vector2Int>();
        for (int x = 0; x < Width; x++) //Iterates through all tiles
        {
            for (int y = 0; y < Height; y++)
            {
                if (Grid[x][y] == target) //If the tile is the target value, it is a part of a target region to be examined
                {
                    HashSet<Vector2Int> newRegion = new HashSet<Vector2Int>(); //Sets up new region
                    Vector2Int tile = new Vector2Int(x, y);
                    var toBeFilled = new Queue<Vector2Int>(); //Initialises queue of tiles to be filled
                    toBeFilled.Enqueue(tile);
                    while (toBeFilled.TryDequeue(out tile)) //Continues until there are no more tiles in the region to be examined
                    {
                        if (!newRegion.Contains(tile)) //If the tile has not already been examined, it is added to the new region
                        {
                            newRegion.Add(tile);
                            Grid[tile.x][tile.y] = !target; //Tile is set to be the opposite value
                            var neighbours = getAdjacents(tile.x, tile.y, diagonals:false); //gets all immediate neighbours
                            foreach (Vector2Int neighbour in neighbours) //Adds neighbouts to queue if they are the target value and not already in the queue or new region
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
                    if (newRegion.Count > largestRegion.Count) //Overrides largest region if new region is larger
                    {
                        largestRegion.Clear();
                        largestRegion.UnionWith(newRegion);
                    }
                }
            }
        }
        foreach (Vector2Int tile in largestRegion) //fills in largest region
        {
            Grid[tile.x][tile.y] = target;
        }

        if (largestRegion.Count < AreaMin) //If the largest region is too small, the room is invalid
        {
            return false;
        }
        return true;
    }
    public void setTightRectangleGrid()
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
        Width = (int)Rect.width;
        Height = (int)Rect.height;
        copyToGrid(gridCopy);
        return;
    }
    //Copies room to grid
    private void copyToGrid(List<List<bool>> source)
    {
        Grid = new List<List<bool>>();
        for (int x = 0; x < source.Count; x++)
        {
            var temp = new List<bool>();
            for (int y = 0; y < source[0].Count; y++)
            {
                temp.Add(source[x][y]);
            }
            Grid.Add(temp);
        }
    }
    //Gets number of filled tiles in a list of tiles
    private int sum(List<bool> source)
    {
        int count = source.Where(x => x == true).Count();
        return count;
    }
    //Gets a row from a grid, source.Count
    private List<bool> getRow(List<List<bool>> source, int rowNumber)
    {
        return Enumerable.Range(0, Width).Select(x => source[x][rowNumber]).ToList();
    }
    //Makes the room a tight rectangle
    public void makeRectangle(int gap = 2)
    {
        int minX = 0;
        int minY = 0;
        int maxX = Width-1;
        int maxY = Height-1;
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
            Debug.LogWarning("Failed to make rectangle");
            Rect = new Rect(0,0,0,0);
            return;
        }
        Rect = new Rect(minX-gap, minY-gap, maxX - minX + 2*gap, maxY - minY+2*gap);
        return;
    }
}