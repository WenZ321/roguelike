using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Consumable : MonoBehaviour
{
    public virtual bool Activate(Actor actor) => false;
    public virtual bool Cast(Actor actor, Actor target) => false;
    public virtual bool Cast(Actor actor, List<Actor> targets) => false;

    public void Consume(Actor consumer)
    {
        if (consumer.GetComponent<Inventory>().SelectedConsumable == this)   //Checks if the selected consumable is this consumable
        {
            consumer.GetComponent<Inventory>().SelectedConsumable = null;    //Unselects the selected consumable
        }

        consumer.Inventory.Items.Remove(GetComponent<Item>());               //Removes item from inventory
        GameManager.instance.RemoveEntity(GetComponent<Item>());             //Removes item from the entity list
        Destroy(gameObject);                                                 //Destroys it
    }
}