using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Equipment : MonoBehaviour
{
    [SerializeField] private Equippable weapon;
    [SerializeField] private Equippable armor;

    public Equippable Weapon { get => weapon; set => weapon = value; }
    public Equippable Armor { get => armor; set => armor = value; }

    public int DefenseBonus()
    {
        int bonus = 0;

        if (weapon is not null && weapon.DefenseBonus > 0)
        {        //Adds weapon defense bonus to the total defense bonus
            bonus += weapon.DefenseBonus;
        }

        if (armor is not null && armor.DefenseBonus > 0)
        {          //Adds armor defense bonus to the total defense bonus
            bonus += armor.DefenseBonus;
        }
        return bonus;
    }

    public int PowerBonus()
    {
        int bonus = 0;

        if (weapon is not null && weapon.PowerBonus > 0)
        {          //Adds weapon power bonus to the total power bonus
            bonus += weapon.PowerBonus;
        }

        if (armor is not null && armor.PowerBonus > 0)
        {            //Adds armor power bonus to the total power bonus
            bonus += armor.PowerBonus;
        }

        return bonus;
    }

    public bool ItemIsEquipped(Item item)
    {
        if (item.Equippable is null)
        {              //Checks if item is equippable or not 
            return false;
        }

        return item.Equippable == weapon || item.Equippable == armor;
    }

    public void UnequipMessage(string name)
    {     //Name explains it
        UIManager.instance.AddMessage($"You remove the {name}.", "#da8ee7");
    }

    public void EquipMessage(string name)
    {       //Name explains it
        UIManager.instance.AddMessage($"You equip the {name}.", "#a000c8");
    }

    public void EquipToSlot(string slot, Item item, bool addMessage)
    {
        Equippable currentItem = slot == "Weapon" ? weapon : armor;         //Checks if the item is a weapon or armor

        if (currentItem is not null)
        {  //Not sure about this
            UnequipFromSlot(slot, addMessage);
        }

        if (slot == "Weapon")
        {     //Sets the equippable slot in the item menu as a weapon or armor 
            weapon = item.Equippable;
        }
        else
        {
            armor = item.Equippable;
        }

        if (addMessage)
        {
            EquipMessage(item.name);
        }

        item.name = $"{item.name} (E)"; //(E) indicates it is equipped
    }

    public void UnequipFromSlot(string slot, bool addMessage)
    {
        Equippable currentItem = slot == "Weapon" ? weapon : armor;
        currentItem.name = currentItem.name.Replace(" (E)", "");

        if (addMessage)
        {
            UnequipMessage(currentItem.name);
        }

        if (slot == "Weapon")
        {     //Removes the item from either weapon slot or armor slot 
            weapon = null;
        }
        else
        {
            armor = null;
        }
    }

    public void ToggleEquip(Item equippableItem, bool addMessage = true)
    {        //Toggles between equipping and unequipping the item 
        string slot = equippableItem.Equippable.EquipmentType == EquipmentType.Weapon ? "Weapon" : "Armor";     //Slot is weapon slot or armor slot 

        if (ItemIsEquipped(equippableItem))
        {
            UnequipFromSlot(slot, addMessage);
        }
        else
        {
            EquipToSlot(slot, equippableItem, addMessage);
        }
    }
}
