using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// - Silas Thule
/// </summary>
[CreateAssetMenu(fileName = "New Projectile Spell", menuName = "Game/Spells/Projectile Spell")]

public class ProjectileSpell : Spell
{
    [SerializeField] private ProjectileInstance Projectile;

    [SerializeField, Tooltip("What effect that happens when the projectile hits")]
    private ExplosionSpell EndEffect;

    [SerializeField,Range(1,30),Tooltip("How fast the projectile moves")] 
    private float Speed = 10;

    [SerializeField,Range(0,30), Tooltip("How far the projectile can travel. If homing is active this range is now the projectiles lifespan.")] 
    private float Range = 10;

    [SerializeField, Tooltip("If checked, then the projectile will always try to travel it's full distance")]
    private bool MustFlyMaxDistance = false;

    [SerializeField,Range(0,3), Tooltip("How far away from the projectile a target can be before it explodes")] 
    private float TriggerRadius = 0.5f;

    [SerializeField, Range(0, 100), Tooltip("A value of zero disables homing.")]
    private float HomingForce = 0f;

    public float speed => this.Speed;
    public float range => this.Range;
    public bool mustFlyMaxDistance => this.MustFlyMaxDistance;
    public float triggerRadius => this.TriggerRadius;
    public float homingForce => this.HomingForce;
    public ProjectileInstance projectile => this.Projectile;
    public ExplosionSpell endEffect => this.EndEffect;

    public override void Activate(UnitController owner, Vector3 target)
    {
        Vector3 origin = owner.transform.position;
        origin.y = 1;
        target.y = 1;
        ProjectileInstance clone = Instantiate(Projectile, origin, Quaternion.identity);
        clone.Initialize(this, target,owner);
    }
}
