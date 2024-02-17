using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  - Silas Thule
/// </summary>
[CreateAssetMenu(fileName = "New Explosion Spell", menuName = "Game/Spells/Explosion Spell")]
public class ExplosionSpell : Spell
{
    [SerializeField, Range(0, 100)] 
    private int Damage = 20;

    [SerializeField,Range(0,3),Tooltip("The time before the damage effect is applied after spawning the explosion.")] 
    private float Delay = 0f;

    [SerializeField,Range(0,3), Tooltip("The time the explosion effect will last after applying damage.")] 
    private float LifeTime = 1f;

    [SerializeField,Range(0,3), Tooltip("The radius of the damage application of the explosion")] 
    private float Radius = 0.5f;

    [SerializeField, Tooltip("If unchecked, the explosion will occur on the owners position (Self Casting).")] 
    private bool ExplodeAtTarget = true;

    [SerializeField] private ExplosionInstance Explosion;

    public int damage => this.Damage;
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

