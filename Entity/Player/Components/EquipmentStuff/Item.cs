using System;
using UnityEngine;

public class Item : Entity
{
    [SerializeField] private Consumable consumable;
    [SerializeField] private Equippable equippable;

    public Consumable Consumable { get => consumable; }
    public Equippable Equippable { get => equippable; }

    private void OnValidate()
    {
        if (GetComponent<Consumable>())
        {                   //Checks if the item is a consumable 
            consumable = GetComponent<Consumable>();
        }
    }

    private void Start() => AddToGameManager();          //Since items are a type of entity 

    public override EntityState SaveState() => new ItemState( //Saves all of the information about this current entity(item)
        name: name,
        blocksMovement: BlocksMovement,
        isVisible: MapManager.instance.VisibleTiles.Contains(MapManager.instance.FloorMap.WorldToCell(transform.position)),
        position: transform.position,
        parent: transform.parent != null ? transform.parent.gameObject.name : ""
      );

    public void LoadState(ItemState state)
    {        //Loading in the item 
        if (!state.IsVisible)
        {                              //If it is not visible
            GetComponent<SpriteRenderer>().enabled = false;    //Then it disables sprite renderer
        }

        if (state.Parent is not "")
        {                        //If the parent is not empty
            GameObject parent = GameObject.Find(state.Parent); //Finds the parent
            parent.GetComponent<Inventory>().Add(this);        //And adds this item to the inventory

            if (equippable is not null && state.Name.Contains("(E)"))
            {                                       //Checks and see if it was equipped before or not
                parent.GetComponent<Equipment>().EquipToSlot(equippable.EquipmentType.ToString(), this, false); //Equips it if it was equipped
            }
        }

        transform.position = state.Position;                //Sets position as original
    }
}

[System.Serializable]
public class ItemState : EntityState
{          //Information about the current item
    [SerializeField] private string parent;

    public string Parent { get => parent; set => parent = value; }

    public ItemState(EntityType type = EntityType.Item, string name = "", bool blocksMovement = false, bool isVisible = false, Vector3 position = new Vector3(),
     string parent = "") : base(type, name, blocksMovement, isVisible, position)
    {
        this.parent = parent;
    }
}