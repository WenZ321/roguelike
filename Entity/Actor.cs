using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Actor : Entity
{
    [SerializeField] private bool isAlive = true;
    [SerializeField] private List<Vector3Int> fieldOfView = new List<Vector3Int>();
    [SerializeField] private Inventory inventory;
    [SerializeField] private Equipment equipment;
    [SerializeField] private AI aI;
    [SerializeField] private Fighter fighter;
    [SerializeField] private Level level;
    [SerializeField] private Vector3Int position;

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
        UpdateAIFieldOfView();
        if (!isAlive && fighter != null)
        {           //If is it not alive and there is a fighter component, kill it
            fighter.Die();
        }
    }

    private void UpdateAIFieldOfView()
    {
        if (GetComponent<AI>())
        {
            foreach(RectangularRoom room in MapManager.instance.Rooms)
            {
                position = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
                if (room.X < position.x && (room.X + room.Width) > position.x && room.Y < position.y && (room.Y + room.Height) > position.y)
                {
                    for (int x = room.X; x < room.X + room.Width; x++)
                    {
                        for (int y = room.Y; y < room.Y + room.Height; y++)
                        {
                            fieldOfView.Add(new Vector3Int(x, y));
                        }
                    }
                }
            }
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

    public override EntityState SaveState() => new ActorState(
      name: name,
      blocksMovement: BlocksMovement,
      isAlive: IsAlive,
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

