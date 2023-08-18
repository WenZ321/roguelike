using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour, IDataPersistence
{
    public static MapManager instance;

    [SerializeField] private string mapID;
    [SerializeField] private string sceneName;
    [SerializeField] private int currentFloor = 0;
    MapState _mapState;

    [Header("Map Settings")]
    [SerializeField] private int width = 175;
    [SerializeField] private int height = 175;
    [SerializeField] private int roomMaxSize = 25;
    [SerializeField] private int roomMinSize = 20;
    [SerializeField] private int maxRooms = 30;

    [Header("Tiles")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase upStairsTile;
    [SerializeField] private TileBase downStairsTile;
    [SerializeField] private TileBase doorTile;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap doorMap;

    [Header("Features")]
    [SerializeField] private List<RectangularRoom> rooms;
    [SerializeField] private Dictionary<Vector3Int, TileData> tiles;
    private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

    
    public TileBase FloorTile { get => floorTile; }
    public TileBase WallTile { get => wallTile; }
    public TileBase UpStairsTile { get => upStairsTile; }
    public TileBase DownStairsTile { get => downStairsTile; }
    public TileBase DoorTile { get => doorTile; }
    public Tilemap FloorMap { get => floorMap; }
    public Tilemap ObstacleMap { get => obstacleMap; }
    public Tilemap DoorMap { get => doorMap; } 
    public List<RectangularRoom> Rooms { get => rooms; }
    public Dictionary<Vector2Int, Node> Nodes { get => nodes; set => nodes = value; }
    public int CurrentFloor { get => currentFloor; set => currentFloor = value; }
    public string SceneName { get => sceneName ; set => sceneName = value; }    
    public string MapID { get => mapID; set => mapID = value; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        sceneName = SceneManager.GetActiveScene().name; 
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "RandomDungeon")
        {
            if(_mapState != null)
            {
                LoadState(_mapState);
            }
            else
            {
                GenerateRandomDungeon(true);
            }
        }

        LoadState(_mapState);
    }
    

    private void Start()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.orthographicSize = 5;
    }

    public void GenerateRandomDungeon(bool isNewGame = false)
    {
        /*
        if (floorMap.cellBounds.size.x > 0) 
        {
            Reset();
        }
        else
        {
            rooms = new List<RectangularRoom>();
            tiles = new Dictionary<Vector3Int, TileData>();
        }
        */
        rooms = new List<RectangularRoom>();
        tiles = new Dictionary<Vector3Int, TileData>();

        RandomDungeonGenerator RandomDungeonGenerator = new RandomDungeonGenerator();
        RandomDungeonGenerator.GenerateRandomDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, rooms, isNewGame);

        AddTileMapToDictionary(floorMap);
        AddTileMapToDictionary(obstacleMap);
        AddTileMapToDictionary(doorMap);
    }


    ///<summary>Return True if x and y are inside of the bounds of this map. </summary>
    public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    public GameObject CreateEntity(string entity, Vector2 position)
    {
        // Debug.Log($"Entity: {entity}");
        GameObject entityObject = Instantiate(Resources.Load<GameObject>($"{entity}"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
        entityObject.name = entity;
        return entityObject;
    }


    private void AddTileMapToDictionary(Tilemap tilemap)
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        { //Looping through every position in the tilemap
            if (!tilemap.HasTile(pos))
            {                                      //If no tile, goes to next loop
                continue;
            }

            TileData tile = new TileData(
              name: tilemap.GetTile(pos).name,
              isExplored: false,
              isVisible: false
            );

            tiles.Add(pos, tile);
        }
    }



    private void Reset()
    {
        rooms.Clear();
        tiles.Clear();
        nodes.Clear();

        floorMap.ClearAllTiles();
        obstacleMap.ClearAllTiles();
        doorMap.ClearAllTiles();
    }

    public void LoadState(MapState mapState)
    {
        if (floorMap.cellBounds.size.x > 0)
        {
            Reset();
        }

        rooms = mapState.StoredRooms;
        tiles = mapState.StoredTiles.ToDictionary(x => new Vector3Int((int)x.Key.x, (int)x.Key.y, (int)x.Key.z), x => x.Value);

        foreach (Vector3Int pos in tiles.Keys)
        {
            if (tiles[pos].Name == floorTile.name)
            {
                floorMap.SetTile(pos, floorTile);
            }
            else if (tiles[pos].Name == wallTile.name)
            {
                obstacleMap.SetTile(pos, wallTile);
            }
            else if (tiles[pos].Name == upStairsTile.name)
            {
                floorMap.SetTile(pos, upStairsTile);
            }
            else if (tiles[pos].Name == downStairsTile.name)
            {
                floorMap.SetTile(pos, downStairsTile);
            }
            else if (tiles[pos].Name == doorTile.name)
            {
                doorMap.SetTile(pos, doorTile);
            }
        }
    }

    public void LoadData(GameData data)
    {
        string mapid;
        data.savedScenes.TryGetValue(sceneName, out mapid);
        if (mapid != null)
        {
            mapid = MapID;
            data.mapState.TryGetValue(MapID, out _mapState);
        }
        else
        {

        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.savedScenes.ContainsKey(MapID))
        {
            data.savedScenes.Remove(MapID);
        }
        else
        {
            data.savedScenes.Add(SceneName, MapID);
        }
        if (data.mapState.ContainsKey(MapID))
        {
            data.mapState.Remove(MapID);
        }
        MapState _mapstate = new MapState(sceneName, tiles, rooms);
        data.mapState.Add(MapID, _mapstate);
    }
}

