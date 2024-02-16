using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  - Silas Thule
/// </summary>
[CreateAssetMenu(fileName = "New Explosion Spell", menuName = "Game/Spells/Explosion Spell")]
public class ExplosionSpell : Spell
{
    [SerializeField,Range(0,3)] private float Delay = 0f;
    [SerializeField,Range(0,3)] private float LifeTime = 1f;

    [SerializeField,Range(0,3)] private float Radius = 0.5f;
    [SerializeField] private bool ExplodeAtTarget = true;
    [SerializeField] private ExplosionInstance Explosion;

    public float delay => this.Delay;
    public float lifeTime => this.LifeTime;
    public float radius => this.Radius;

    public bool explodeAtTarget => this.ExplodeAtTarget;
    public ExplosionInstance explosion => this.Explosion;
    public override void Activate(UnitController owner, Vector3 target)
    {
        ExplosionInstance clone = Instantiate(explosion,ExplodeAtTarget ? target : owner.transform.position, Quaternion.identity);
        clone.Initialize(this, owner);
    }
}

