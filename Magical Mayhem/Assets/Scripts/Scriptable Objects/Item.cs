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

[SerializeField,Tooltip("Defines whether the item have elementBost or not")]
    private bool HasElementBoost=true;

public int damage => this.Damage;
public int health => this.Health;
public int cDReduction => this.CDReduction;

public float elementBoostPercent => ElementBoostPercent;
public bool hasElementBoost => HasElementBoost;

 public enum ItemType{
    Defensive, Offensive, Utility
 }

 public enum ElementBoost{
    Ice, Fire, Arcane
 }

}
