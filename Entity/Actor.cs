using System.Collections.Generic;
using UnityEngine;

public class Actor : Entity
{
    [SerializeField] private bool isAlive = true; //read-only
    [SerializeField] private int fieldOfViewRange = 8;
    [SerializeField] private List<Vector3Int> fieldOfView = new List<Vector3Int>();
    [SerializeField] private Inventory inventory;
    [SerializeField] private Equipment equipment;
    [SerializeField] private AI aI;
    [SerializeField] private Fighter fighter;
    [SerializeField] private Level level;
    AdamMilVisibility algorithm;

    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public List<Vector3Int> FieldOfView { get => fieldOfView; }
    public Inventory Inventory { get => inventory; }
    public Equipment Equipment { get => equipment; }
    public AI AI { get => aI; set => aI = value; }
    public Fighter Fighter { get => fighter; set => fighter = value; }
    public Level Level { get => level; set => level = value; }

    private void OnValidate()
    {
        if (GetComponent<Inventory>())
        {
            inventory = GetComponent<Inventory>();
        }

        if (GetComponent<AI>())
        {
            aI = GetComponent<AI>();
        }

        if (GetComponent<Fighter>())
        {
            fighter = GetComponent<Fighter>();
        }

        if (GetComponent<Level>())
        {
            level = GetComponent<Level>();
        }

        if (GetComponent<Equipment>())
        {
            equipment = GetComponent<Equipment>();
        }
    }

    private void Start()
    {
        AddToGameManager();

        if (isAlive)
        {                          //Sets of the POV of mobs if it is alive
            algorithm = new AdamMilVisibility();
            UpdateFieldOfView();
        }
        else if (fighter != null)
        {           //If is it not alive and there is a fighter component, kill it
            fighter.Die();
        }
    }

    public override void AddToGameManager()
    {
        base.AddToGameManager();

        if (GetComponent<Player>())
        {
            GameManager.instance.InsertActor(this, 0);
        }
        else
        {
            GameManager.instance.AddActor(this);
        }
    }

    public void UpdateFieldOfView()
    {
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(transform.position);     //Gets current position

        fieldOfView.Clear();                                                                        //Clears POV
        algorithm.Compute(gridPosition, fieldOfViewRange, fieldOfView);                             //Creates new POV

        if (GetComponent<Player>())
        {
            MapManager.instance.UpdateFogMap(fieldOfView);                                            //Updates the fog map if it is a player
            MapManager.instance.SetEntitiesVisibilities();                                            //Sets the visibilities of entities
        }
    }

    public override EntityState SaveState() => new ActorState(
      name: name,
      blocksMovement: BlocksMovement,
      isAlive: IsAlive,
      isVisible: MapManager.instance.VisibleTiles.Contains(MapManager.instance.FloorMap.WorldToCell(transform.position)),
      position: transform.position,
      currentAI: aI != null ? AI.SaveState() : null,
      fighterState: fighter != null ? fighter.SaveState() : null,
      levelState: level != null && GetComponent<Player>() ? level.SaveState() : null
    );

    public void LoadState(ActorState state)
    {
        transform.position = state.Position;
        isAlive = state.IsAlive;

        if (!IsAlive)
        {
            GameManager.instance.RemoveActor(this);
        }

        if (!state.IsVisible)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        if (state.CurrentAI != null)
        {
            if (state.CurrentAI.Type == "HostileEnemy")
            {
                aI = GetComponent<HostileEnemy>();
            }
        }

        if (state.FighterState != null)
        {
            fighter.LoadState(state.FighterState);
        }

        if (state.LevelState != null)
        {
            level.LoadState(state.LevelState);
        }
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
     bool isAlive = true, AIState currentAI = null, FighterState fighterState = null, LevelState levelState = null) : base(type, name, blocksMovement, isVisible, position)
    {
        this.isAlive = isAlive;
        this.currentAI = currentAI;
        this.fighterState = fighterState;
        this.levelState = levelState;
    }
}