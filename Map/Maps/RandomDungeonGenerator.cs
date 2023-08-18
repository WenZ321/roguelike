using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using SysRandom = System.Random;
using UnityRandom = UnityEngine.Random;

sealed class RandomDungeonGenerator
{

    private List<Tuple<int, int>> maxItemsByFloor = new List<Tuple<int, int>> {           //First number being the floor number
    new Tuple<int, int>(1, 1),                                                          //Second number being the amount of items or monsters
    new Tuple<int, int>(4, 2),
    new Tuple<int, int>(7, 3),
    new Tuple<int, int>(10, 4),
  };
    private List<Tuple<int, int>> maxMonstersByFloor = new List<Tuple<int, int>> {
    new Tuple<int, int>(1, 2),
    new Tuple<int, int>(4, 3),
    new Tuple<int, int>(6, 5),
    new Tuple<int, int>(8, 7),
    new Tuple<int, int>(10, 10),
  };

    private List<Tuple<int, string, int>> itemChances = new List<Tuple<int, string, int>> {       //Floor number, name of entity, chances of the entity 
    new Tuple<int, string, int>(0, "Potion of Health", 35),
    new Tuple<int, string, int>(2, "Sword", 5),
    new Tuple<int, string, int>(4, "Chain Mail", 15),
  };

    private List<Tuple<int, string, int>> monsterChances = new List<Tuple<int, string, int>> {
    new Tuple<int, string, int>(1, "Orc", 80),
    new Tuple<int, string, int>(3, "Troll", 15),
    new Tuple<int, string, int>(5, "Troll", 30),
    new Tuple<int, string, int>(7, "Troll", 60),
  };

    private int GetMaxValueForFloor(List<Tuple<int, int>> values, int floor)
    {             //Gets the max amount of entity for the current floor 
        int currentValue = 0;

        foreach (Tuple<int, int> value in values)
        {
            if (floor >= value.Item1)
            {
                currentValue = value.Item2;
            }
        }

        return currentValue;
    }

    private List<string> GetEntitiesAtRandom(List<Tuple<int, string, int>> chances, int numberOfEntities, int floor)
    {     //Spawns the entities at random given the chances 
        List<string> entities = new List<string>();
        List<int> weightedChances = new List<int>();

        foreach (Tuple<int, string, int> chance in chances)
        {
            if (floor >= chance.Item1)
            {
                entities.Add(chance.Item2);
                weightedChances.Add(chance.Item3);
            }
        }

        SysRandom rnd = new SysRandom();
        List<string> chosenEntities = rnd.Choices(entities, weightedChances, numberOfEntities);

        return chosenEntities;                                                                                             //returns a list with the names of the entities 
    }


    /// <summary>
    /// Generate a new dungeon map.
    /// </summary>

    public void GenerateRandomDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, List<RectangularRoom> rooms, bool isNewGame)
    {
        List<Vector2Int> RoomCoords = new List<Vector2Int>();
        HelperFunctions.GenerateCoords(RoomCoords, mapWidth, mapHeight, maxRooms, roomMaxSize);


        // Generate the rooms.
        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {                                                                       //Generates each room first, not the entire dungeon 
            int roomWidth = UnityRandom.Range(roomMinSize, roomMaxSize);        //Random width
            int roomHeight = UnityRandom.Range(roomMinSize, roomMaxSize);       //Random Height


            int roomX = RoomCoords[roomNum].x;                                  //X and Y value for the bottom left corner of each rooms
            int roomY = RoomCoords[roomNum].y;

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);   //Saves this current room being generated as a variable 

            //Check if this room intersects with any other rooms
            if (newRoom.Overlaps(rooms))
            {
                continue;
            }
            //If there are no intersections then the room is valid.

            //Dig out this rooms inner area and builds the walls.
            for (int x = roomX; x < roomX + roomWidth; x++)
            {                 //Loops thru the dimensions using the x + width and y + length
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {  //roomX is the bottom left corner of room, this places walls at the perimeter of the room
                        if (SetWallTileIfEmpty(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y));
                    }
                }
            }

            if (rooms.Count != 0)
            {
                //Dig out a tunnel between this room and the previous one.
                TunnelBetween(rooms[rooms.Count - 1], newRoom);
            }

            PlaceEntities(newRoom, MapManager.instance.CurrentFloor);       //Place entities at the current room generated 

            rooms.Add(newRoom);                                              //Adds the generated room to the list of rooms 
        }

        //Add the stairs to the last room.
        MapManager.instance.FloorMap.SetTile((Vector3Int)rooms[rooms.Count - 1].RandomPoint(), MapManager.instance.DownStairsTile);     //Takes a random point 

        //Add the player to the first room.
        Vector3Int playerPos = (Vector3Int)rooms[0].RandomPoint();

        while (GameManager.instance.GetActorAtLocation(playerPos) is not null)
        {    //Generates a new playerPos if playerPos is null
            playerPos = (Vector3Int)rooms[0].RandomPoint();
        }

        MapManager.instance.FloorMap.SetTile(playerPos, MapManager.instance.UpStairsTile); //Upstairs is going to previous level while downstairs is going to next level 

        if (!isNewGame)
        {
            GameManager.instance.Actors[0].transform.position = new Vector3(playerPos.x + 0.5f, playerPos.y + 0.5f, 0);       //If it is not a new game, places player at the playerPos
        }
        else
        {
            GameObject player = MapManager.instance.CreateEntity("Player", (Vector2Int)playerPos);                            //Else, creates a new player
            Actor playerActor = player.GetComponent<Actor>();

            Item starterWeapon = MapManager.instance.CreateEntity("Dagger", (Vector2Int)playerPos).GetComponent<Item>();
            Item starterArmor = MapManager.instance.CreateEntity("Leather Armor", (Vector2Int)playerPos).GetComponent<Item>();

            playerActor.Inventory.Add(starterWeapon);
            playerActor.Inventory.Add(starterArmor);                                                                          //Spawns the player in with the starter gear

            playerActor.Equipment.EquipToSlot("Weapon", starterWeapon, false);                                                //Equips items
            playerActor.Equipment.EquipToSlot("Armor", starterArmor, false);
        }
    }

    /// <summary>
    /// Return an L-shaped tunnel between these two points using Bresenham lines.
    /// </summary>
    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
    {   //Helper function used to generate tunnels between rooms
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (UnityRandom.value < 0.5f)
        {
            //Move horizontally, then vertically.
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            //Move vertically, then horizontally.
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        //Generate the coordinates for this tunnel.
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();

        HelperFunctions.GenerateTunnel(oldRoomCenter, tunnelCorner, tunnelCoords);
        HelperFunctions.GenerateTunnel(tunnelCorner, newRoomCenter, tunnelCoords);

        //Set the tiles for this tunnel.
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            //Set the wall tiles around this tile to be walls.
            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (SetWallTileIfEmpty(new Vector3Int(x, y)))
                    {
                        continue;
                    }
                }
            }
        }
    }

    private bool SetWallTileIfEmpty(Vector3Int pos)
    {
        if (MapManager.instance.FloorMap.GetTile(pos))
        {                                //If no tile at the position
            return true;
        }
        else
        {
            MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.WallTile);   //Places wall tiles
            return false;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {                                       //Places a tile at position even if it already has a tile
        if (MapManager.instance.ObstacleMap.GetTile(pos))
        {
            MapManager.instance.ObstacleMap.SetTile(pos, null);
        }
        MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);
    }

    private void PlaceEntities(RectangularRoom newRoom, int floorNumber)
    {
        int numberOfMonsters = UnityRandom.Range(0, GetMaxValueForFloor(maxMonstersByFloor, floorNumber) + 1);
        int numberOfItems = UnityRandom.Range(0, GetMaxValueForFloor(maxItemsByFloor, floorNumber) + 1);

        List<string> monsterNames = GetEntitiesAtRandom(monsterChances, numberOfMonsters, floorNumber);
        List<string> itemNames = GetEntitiesAtRandom(itemChances, numberOfItems, floorNumber);

        List<string> entityNames = monsterNames.Concat(itemNames).ToList();                                 //Creates a big list with the monsters and items 

        foreach (string entityName in entityNames)
        {
            Vector3Int entityPos = (Vector3Int)newRoom.RandomPoint();

            while (GameManager.instance.GetActorAtLocation(entityPos) is not null)
            {                          //Checks if there is location or not at the entity 
                entityPos = (Vector3Int)newRoom.RandomPoint();
            }

            MapManager.instance.CreateEntity(entityName, (Vector2Int)entityPos);                              //Creates entity at that position 
        }
    }
}