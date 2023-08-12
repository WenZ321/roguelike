using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Consumable : MonoBehaviour
{
    public virtual bool Activate(Actor actor) => false;

    public void Consume(Actor consumer)
    {
        if (consumer.GetComponent<Inventory>().SelectedConsumable == this)
        {                                                                           //The consumable script is on every consumable so if the selected consumable is whichever one
            consumer.GetComponent<Inventory>().SelectedConsumable = null;             //Unselects the consumable
        }

        consumer.Inventory.Items.Remove(GetComponent<Item>());                      //Removes the consumable(item), therefore consuming
        GameManager.instance.RemoveEntity(GetComponent<Item>());                    //Removes it from the entity list
        Destroy(gameObject);                                                        //And deletes it as well
    }
}