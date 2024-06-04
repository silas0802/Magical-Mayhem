using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
[CreateAssetMenu(fileName = "New LaserBeam Spell", menuName = "Game/Spells/LaserBeam Spell")]
public class LaserBeamSpell : Spell
{
    

    // Start is called before the first frame update
    [SerializeField,Range(1,20),Tooltip("The lenght of the beam")] 
    private float Lenght = 4;

    [SerializeField,Range(1,5),Tooltip("the width of the beam")]
    private float Width = 4f;

    [SerializeField,Range(1,5),Tooltip("The duration of the spell")]
    private float Duration = 10f;

    [SerializeField,Range(1,5),Tooltip("Damage per second")]
    private int DamagePerSecond = 10;

    [SerializeField,Range(1,5),Tooltip("How often it damages, the smaller the number here the less time it damages")]
    private float Tickrate = 10f;

    [SerializeField] private LaserBeamInstance LaserBeamInstance;

       
    public float lenght => this.Lenght;
    public float width => this.Width;
    public float duration => this.Duration;
    public int damagePerSecond => this.DamagePerSecond;

    public float tickrate => this.Tickrate;
    public LaserBeamInstance laserBeamInstance => this.LaserBeamInstance;


    public override void Activate(UnitController owner, Vector3 target)
    {
        LaserBeamInstance lb = Instantiate(laserBeamInstance, owner.transform.position,quaternion.identity);
        lb.Initialize(this, owner,target);
    }
}
