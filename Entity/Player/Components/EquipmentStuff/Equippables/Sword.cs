sealed class Sword : Equippable
{               //A type of equippable
    public Sword()
    {
        EquipmentType = EquipmentType.Weapon;
        PowerBonus = 4;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {                                      //If gameObject has a parent(which is the player)
            gameObject.transform.parent.GetComponent<Equipment>().Weapon = this;  //Sets their weapon equipment as this 
        }
    }
}