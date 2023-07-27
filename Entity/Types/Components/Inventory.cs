using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 0;
    [SerializeField] private Consumable selectedConsumable = null;
    [SerializeField] private List<Item> items = new List<Item>();
    public int Capacity { get => capacity; }
    public Consumable SelectedConsumable { get => selectedConsumable; set => selectedConsumable = value; }
    public List<Item> Items { get => items; }

    public void Add(Item item)      //Adds item, similiar concept to dropping items.
    {
        items.Add(item);
        item.transform.SetParent(transform);
        GameManager.instance.RemoveEntity(item);
    }

    public void Drop(Item item)    //Drops item      
    {
        items.Remove(item);        //Removes from your inventory and adds it to the entity list
        item.transform.SetParent(null);     //I think this puts the item to where you dropped it
        GameManager.instance.AddEntity(item);
        UIManager.instance.AddMessage($"You dropped the {item.name}.", "#FF0000");
    }
}