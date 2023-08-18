using UnityEngine;

/// <summary>
/// A generic class to represent players, enemies, items, etc.
/// </summary>
public class Entity : MonoBehaviour
{
    [SerializeField] private string entityId;

    [SerializeField] private bool blocksMovement;        //Only for players and enemies
    [SerializeField] private EntityType type;

    public EntityType Type { get => type; set => type = value; }
    public string EntityID { get => entityId; set => entityId = value; }
    public bool BlocksMovement { get => blocksMovement; set => blocksMovement = value; }

    public virtual void AddToGameManager()
    {
        if (GetComponent<Player>())
        {
            GameManager.instance.InsertEntity(this, 0);      //Adds players as first entity                             
        }
        else
        {
            GameManager.instance.AddEntity(this);            //Adds all the rest of entities 
        }
    }

    private void Awake()
    {
        GenerateGuid();
    }

    private void GenerateGuid()
    {
        entityId = System.Guid.NewGuid().ToString();
    }

    public virtual EntityState SaveState() => new EntityState();
}

