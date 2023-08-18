using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Item : Entity
{
    private ItemState _itemState;

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
        position: transform.position,
        parent: transform.parent != null ? transform.parent.gameObject.name : ""
      );

    public void LoadState(ItemState state)
    {       

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

