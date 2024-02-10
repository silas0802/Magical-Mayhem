using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Spell", menuName = "Game/Spells/Projectile Spell")]

public class ProjectileSpell : Spell
{
    [SerializeField,Range(0,30)] private float Range = 10;
    [SerializeField,Range(0,3)] private float TriggerRadius = 0.5f;
    [SerializeField] private ProjectileInstance Projectile;
    [SerializeField] private ExplosionSpell EndEffect;

    public float range => this.Range;
    public float triggerRadius => this.TriggerRadius;
    public ProjectileInstance projectile => this.Projectile;
    public ExplosionSpell endEffect => this.EndEffect;

    public override void Activate(UnitController owner, Vector3 target)
    {
        ProjectileInstance clone = Instantiate(Projectile, target, Quaternion.identity);
        clone.Initialize();
    }
}
