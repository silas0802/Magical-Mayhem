using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Explosion Spell", menuName = "Game/Spells/Explosion Spell")]
public class ExplosionSpell : Spell
{
    [Range(0,3)] public float Radius = 0.5f;
    public ExplosionInstance Explosion;
    public override void Activate(UnitController owner, Vector3 target)
    {
        throw new System.NotImplementedException();
    }
}

