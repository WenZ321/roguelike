using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityRandom = UnityEngine.Random;

public class FirstSceneMap : MonoBehaviour
{
    public static FirstSceneMap Instance;

    private int roomNum = 3;
    private List<Vector2Int> coords = new List<Vector2Int>() {new Vector2Int(-3,-3), new Vector2Int(-7, 18), new Vector2Int(-5, 40)};
    private List<Vector2Int> size = new List<Vector2Int>() { new Vector2Int(7, 7), new Vector2Int(11, 11), new Vector2Int(9, 9)};

    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap fogMap;

    [Header("Features")]
    [SerializeField] private List<RectangularRoom> rooms;
    [SerializeField] private List<Vector3Int> visibleTiles;
    [SerializeField] private Dictionary<Vector3Int, TileData> tiles;
    private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

    public Tilemap FloorMap { get => floorMap; }
    public Tilemap ObstacleMap { get => obstacleMap; }
    public Tilemap FogMap { get => fogMap; }
    public List<Vector3Int> VisibleTiles { get => visibleTiles; }

    private void GenerateDungeon()
    {

        for (int i = 0; i < roomNum; i++)
        {
            int roomWidth = size[i].x;
            int roomHeight = size[i].y;
            int roomX = coords[i].x;
            int roomY = coords[i].y;

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);

            for (int x = roomX; x < roomX + roomWidth; x++)
            {
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
        }
    }

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

}
