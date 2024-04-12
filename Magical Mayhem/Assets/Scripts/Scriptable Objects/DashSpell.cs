using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dash Spell", menuName = "Game/Spells/Dash Spell")]
public class DashSpell : Spell
{

    [SerializeField,Range(1,20),Tooltip("the extra max movementSpeed you gain")] 
    private float MaxMoveSpeed = 10;

    
    [SerializeField, Range(0, 2f), Tooltip("A higher value will make the unit get up to maxSpeed quicker.")]
    private float acceleration = 1f;

    [SerializeField,Range(0,20), Tooltip("How far you characther can travel")]
    private float Range =5;

    [SerializeField, Tooltip("Your DashInstance")]
    DashInstance DashInstance;

    public float maxMoveSpeed => this.MaxMoveSpeed;

    public float range => this.Range;
    public DashInstance dashInstance => this.DashInstance;
    
     public override void Activate(UnitController owner, Vector3 target)
    {

        DashInstance d = Instantiate(dashInstance, owner.transform);
        d.Initialize(this,owner, target);
    }
}