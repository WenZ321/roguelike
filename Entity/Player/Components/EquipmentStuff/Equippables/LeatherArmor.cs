sealed class LeatherArmor : Equippable
{                //A type of equippable
    public LeatherArmor()
    {
        EquipmentType = EquipmentType.Armor;
        DefenseBonus = 1;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {                                      //If gameObject has a parent(which is the player)
            gameObject.transform.parent.GetComponent<Equipment>().Armor = this;   //Sets their armor equipment as this 
        }
    }
}