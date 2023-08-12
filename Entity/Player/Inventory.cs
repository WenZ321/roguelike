using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 0;                        //Max amount of items you can hold
    [SerializeField] private Consumable selectedConsumable = null;    //If there is a consumable selected or not 
    [SerializeField] private List<Item> items = new List<Item>();
    public int Capacity { get => capacity; }
    public Consumable SelectedConsumable { get => selectedConsumable; set => selectedConsumable = value; }
    public List<Item> Items { get => items; }

    public void Add(Item item)
    {
        items.Add(item);                            //Adds to inventory
        item.transform.SetParent(transform);        //Sets the parent of the item's transform to this transform
        GameManager.instance.RemoveEntity(item);    //Removes it from entity list since it is in your inventory now
    }

    public void Drop(Item item)
    {
        items.Remove(item);                         //Removes from inventory 
        item.transform.SetParent(null);
        GameManager.instance.AddEntity(item);       //Adds it to entity list
        UIManager.instance.AddMessage($"You dropped the {item.name}.", "#FF0000");
    }
}