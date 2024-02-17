using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Class", menuName = "Game/Class")]
public class UnitClass : ScriptableObject
{
    [Header("Combat Statistics")]
    [SerializeField, Tooltip("These are the spell types that the class may equip")] private SpellType[] AllowedSpells = new SpellType[6];

    [SerializeField, Tooltip("Do you actually need this explained????")] private int MaxHealth;

    public SpellType[] allowedSpells => this.AllowedSpells;
    public int maxHealth => this.MaxHealth;

    //private void OnValidate()
    //{
    //    if (AllowedSpells.Length != 6)
    //    {
    //        AllowedSpells = new SpellType[6];
    //    }
    //}
}
