using UnityEngine;

[RequireComponent(typeof(Item))]
public class Healing : Consumable
{
    [SerializeField] private int amount = 4;        //Can change this amt depending on how much it heals

    public int Amount { get => amount; }

    public override bool Activate(Actor consumer)   //Self explanantory 
    {
        int amountRecovered = consumer.GetComponent<Fighter>().Heal(amount);    

        if (amountRecovered > 0)
        {
            UIManager.instance.AddMessage($"You consume the {name}, and recover {amountRecovered} HP!", "#00FF00");
            Consume(consumer);
            return true;
        }
        else
        {
            UIManager.instance.AddMessage("Your health is already full.", "#808080");
            return false;
        }
    }
}