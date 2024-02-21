using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class Item : Buyable
{
  



[SerializeField, Range(100,1000), Tooltip("increases damage of your spells")]
    private int Damage =100;


[SerializeField, Range(100,1000),Tooltip("Health of your charater")]
    private int Health = 1000;

[SerializeField, Range(1,15),Tooltip("Decreases cooldown of spells")]
    private int CDReduction = 5;

[SerializeField, Range(1,20), Tooltip("Boost the percentage damage of the element bost")]
    private float ElementBoostPercent = 5F;



[SerializeField,Tooltip("What itemType this item has")]
    private ItemType ItemType;

[SerializeField,Tooltip("what elementBoost this item has")]
    private SpellElementType ItemElement;

public int damage => this.Damage;
public int health => this.Health;
public int cDReduction => this.CDReduction;

public float elementBoostPercent => this.ElementBoostPercent;

public ItemType itemType => this.itemType;
public SpellElementType itemElement => this.itemElement;

 

 

}

public enum ItemType
{
    Defensive, Offensive, Utility
 }