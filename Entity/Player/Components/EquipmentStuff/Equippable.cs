using UnityEngine;

[RequireComponent(typeof(Item))]
public class Equippable : MonoBehaviour
{
    [SerializeField] private EquipmentType equipmentType;     //If it is armor or weapon
    [SerializeField] private int powerBonus = 0;
    [SerializeField] private int defenseBonus = 0;

    //Gets these values from the euippable items 
    public EquipmentType EquipmentType { get => equipmentType; set => equipmentType = value; }
    public int PowerBonus { get => powerBonus; set => powerBonus = value; }
    public int DefenseBonus { get => defenseBonus; set => defenseBonus = value; }
}