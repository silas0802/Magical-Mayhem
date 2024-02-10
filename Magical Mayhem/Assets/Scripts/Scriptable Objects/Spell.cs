using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : ScriptableObject
{
    [TextArea(5,10)] public string Description = "This is a spell";

    public SpellType Type;

    [SerializeField,Range(1,20), Tooltip("The amount of gold required to purchase this spell")] 
    private int Price = 10;

    [Range(0,20), Tooltip("The duration you have to wait before, you can use the spell again")] 
    private float Cooldown = 3;

    [Range(0,3), Tooltip("The amount of time in between the use of a spell and the spells effect taking place.")] 
    private float CastTime = 0.5f;

    public int price => this.Price;
    public float cooldown => this.Cooldown;
    public float castTime => this.CastTime;

    public abstract void Activate(UnitController owner, Vector3 target);
}

public enum SpellType
{
    Offensive,
    Defensive,
    Movement,
    Utility,
}
