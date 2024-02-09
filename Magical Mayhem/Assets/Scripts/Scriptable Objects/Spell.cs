using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : ScriptableObject
{
    [TextArea(5,10)] public string Description = "This is a spell";
    [Range(1,20)] public int Price = 10;
    [Range(0,20)] public float Cooldown = 3;

    [Range(0,3)] public float CastTime = 0.5f;


    public abstract void Activate(UnitController owner, Vector3 target);

}
