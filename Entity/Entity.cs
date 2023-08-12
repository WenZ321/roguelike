using UnityEngine;

/// <summary>
/// A generic class to represent players, enemies, items, etc.
/// </summary>
public class Entity : MonoBehaviour
{
    [SerializeField] private bool blocksMovement;        //Only for players and enemies

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
    public virtual EntityState SaveState() => new EntityState();
}

[System.Serializable]
public class EntityState
{                           //All the info about the current entity 
    public enum EntityType
    {                   //Type of entity 
        Actor,
        Item,
        Other
    }
    [SerializeField] private EntityType type;
    [SerializeField] private string name;
    [SerializeField] private bool blocksMovement, isVisible;
    [SerializeField] private Vector3 position;

    public EntityType Type { get => type; set => type = value; }
    public string Name { get => name; set => name = value; }
    public bool BlocksMovement { get => blocksMovement; set => blocksMovement = value; }
    public bool IsVisible { get => isVisible; set => isVisible = value; }
    public Vector3 Position { get => position; set => position = value; }

    public EntityState(EntityType type = EntityType.Other, string name = "", bool blocksMovement = false, bool isVisible = false, Vector3 position = new Vector3())
    {
        this.type = type;
        this.name = name;
        this.blocksMovement = blocksMovement;
        this.isVisible = isVisible;
        this.position = position;
    }
}