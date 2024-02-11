using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Class", menuName = "Game/Class")]
public class UnitClass : ScriptableObject
{
    [Header("Combat Statistics")]
    [SerializeField, Tooltip("These are the spell types that the class may equip")] private SpellType[] AllowedSpells = new SpellType[6];

    [Header("Movement Statistics")]

    [SerializeField,Range(0,5f),Tooltip("A higher value will make the unit get up to maxSpeed quicker.")]
    private float Acceleration = 2.5f;

    [SerializeField,Range(0, 5f),Tooltip("A higher value will slow down the unit quicker.")]
    private float Friction = 2f;

    [SerializeField,Range(0, 10f),Tooltip("The max speed the unit can go by walking. This can be exceeded through knockbacks.")]
    private float MaxSpeed = 5;

    public SpellType[] allowedSpells => this.AllowedSpells;
    public float acceleration => this.Acceleration;
    public float friction => this.Friction;
    public float maxSpeed => this.MaxSpeed;


    //private void OnValidate()
    //{
    //    if (AllowedSpells.Length != 6)
    //    {
    //        AllowedSpells = new SpellType[6];
    //    }
    //}
}
