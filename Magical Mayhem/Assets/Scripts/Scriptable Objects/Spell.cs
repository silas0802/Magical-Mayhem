using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : Buyable
{
    [SerializeField, Tooltip("What kind of function the spell serves. (e.g: An attack is offensive)")] 
    private SpellUseType UseType;

    [SerializeField, Tooltip("The element of the spell.")] 
    private SpellElementType ElementType;

    [SerializeField, Tooltip("Set to true if this is a compnent of another spell. (e.g: The explosion for a projectile spell).")] 
    private bool IsComponentSpell = false;

    [SerializeField, Range(0,20), Tooltip("The duration you have to wait before, you can use the spell again")] 
    private float Cooldown = 3;

    [SerializeField, Range(0,3), Tooltip("The amount of time in between the use of a spell and the spells effect taking place.")] 
    private float CastTime = 0.5f;
    [SerializeField, Tooltip("If true, the player can walk while casting.")]
    private bool CanMoveWhileCasting = false;

    public SpellUseType useType => this.UseType;
    public SpellElementType elementType => this.ElementType;
    public bool isComponentSpell => this.IsComponentSpell;
    public float cooldown => this.Cooldown;
    public float castTime => this.CastTime;
    public bool canMoveWhileCasting => this.CanMoveWhileCasting;

    public abstract void Activate(UnitController owner, Vector3 target);
}

public enum SpellUseType
{
    None,
    Offensive,
    Defensive,
    Movement,
    Utility,
}
public enum SpellElementType
{
    None,
    Arcane,
    Fire,
    Water,
    Ground,
}
