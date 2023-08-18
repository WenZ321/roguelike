using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameState
{
    [SerializeField] private List<EntityState> entities;

    public List<EntityState> Entities { get => entities; set => entities = value; }

    public GameState(List<EntityState> entities)
    {
        this.entities = entities;
    }
}

[System.Serializable]
public class MapState
{
    [SerializeField] private string mapName;
    [SerializeField] private Dictionary<Vector3, TileData> storedTiles;
    [SerializeField] private List<RectangularRoom> storedRooms;
    public string MapName { get => MapName; set => MapName = value; }
    public Dictionary<Vector3, TileData> StoredTiles { get => storedTiles; set => storedTiles = value; }
    public List<RectangularRoom> StoredRooms { get => storedRooms; set => storedRooms = value; }

    public MapState(string name, Dictionary<Vector3Int, TileData> tiles, List<RectangularRoom> rooms)
    {
        mapName = name;
        storedTiles = tiles.ToDictionary(x => (Vector3)x.Key, x => x.Value);
        storedRooms = rooms;
    }
}

[System.Serializable]
public class EntityState
{                           //All the info about the current entity 
    [SerializeField] private EntityType type;
    [SerializeField] private string name;
    [SerializeField] private bool blocksMovement, isVisible;
    [SerializeField] private Vector3 position;

    public EntityType Type { get => type; set => type = value; }
    public string Name { get => name; set => name = value; }
    public bool BlocksMovement { get => blocksMovement; set => blocksMovement = value; }
    public Vector3 Position { get => position; set => position = value; }

    public EntityState(EntityType type = EntityType.Other, string name = "", bool blocksMovement = false, Vector3 position = new Vector3())
    {
        this.type = type;
        this.name = name;
        this.blocksMovement = blocksMovement;
        this.position = position;
    }
}

[System.Serializable]
public class ActorState : EntityState
{
    [SerializeField] private bool isAlive;
    [SerializeField] private AIState currentAI;
    [SerializeField] private FighterState fighterState;
    [SerializeField] private LevelState levelState;

    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public AIState CurrentAI { get => currentAI; set => currentAI = value; }
    public FighterState FighterState { get => fighterState; set => fighterState = value; }
    public LevelState LevelState { get => levelState; set => levelState = value; }

    public ActorState(EntityType type = EntityType.Actor, string name = "", bool blocksMovement = false, bool isVisible = false, Vector3 position = new Vector3(),
     bool isAlive = true, AIState currentAI = null, FighterState fighterState = null, LevelState levelState = null) : base(type, name, blocksMovement, position)
    {
        this.isAlive = isAlive;
        this.currentAI = currentAI;
        this.fighterState = fighterState;
        this.levelState = levelState;
    }
}

public class FighterState
{
    [SerializeField] private int maxHp, hp, defense, power;
    [SerializeField] private string target;

    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Hp { get => hp; set => hp = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Power { get => power; set => power = value; }
    public string Target { get => target; set => target = value; }

    public FighterState(int maxHp, int hp, int defense, int power, string target)
    {
        this.maxHp = maxHp;
        this.hp = hp;
        this.defense = defense;
        this.power = power;
        this.target = target;
    }
}

[System.Serializable]
public class ItemState : EntityState
{          //Information about the current item
    [SerializeField] private string parent;

    public string Parent { get => parent; set => parent = value; }

    public ItemState(EntityType type = EntityType.Item, string name = "", bool blocksMovement = false, Vector3 position = new Vector3(),
     string parent = "") : base(type, name, blocksMovement, position)
    {
        this.parent = parent;
    }
}

public class LevelState
{
    [SerializeField] private int currentLevel = 1, currentXp, xpToNextLevel;

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public int CurrentXp { get => currentXp; set => currentXp = value; }
    public int XpToNextLevel { get => xpToNextLevel; set => xpToNextLevel = value; }

    public LevelState(int currentLevel, int currentXp, int xpToNextLevel)
    {
        this.currentLevel = currentLevel;
        this.currentXp = currentXp;
        this.xpToNextLevel = xpToNextLevel;
    }
}
