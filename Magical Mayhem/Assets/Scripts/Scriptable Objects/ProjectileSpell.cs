using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Spell", menuName = "Game/Spells/Projectile Spell")]

public class ProjectileSpell : Spell
{
    [Range(0,30)] public float Range = 10;
    [Range(0,3)] public float TriggerRadius = 0.5f;
    public ProjectileInstance Projectile;
    public ExplosionSpell EndEffect;
    public override void Activate(UnitController owner, Vector3 target)
    {
        throw new System.NotImplementedException();
    }
}
