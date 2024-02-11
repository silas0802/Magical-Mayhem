using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Explosion Spell", menuName = "Game/Spells/Explosion Spell")]
public class ExplosionSpell : Spell
{
    [SerializeField,Range(0,3)] private float Radius = 0.5f;
    [SerializeField] private ExplosionInstance Explosion;

    public float radius => this.Radius;
    public ExplosionInstance explosion => this.Explosion;
    public override void Activate(UnitController owner, Vector3 target)
    {
        throw new System.NotImplementedException();
    }
}

