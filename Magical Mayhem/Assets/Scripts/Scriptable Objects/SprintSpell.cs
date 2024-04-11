using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dash Spell", menuName = "Game/Spells/Sprint Spell")]
public class SprintSpell : Spell
{

    [SerializeField,Range(1,20),Tooltip("the extra max movementSpeed you gain")] 
    private float MaxMoveSpeed = 10;

    
    [SerializeField, Range(0, 2f), Tooltip("A higher value will make the unit get up to maxSpeed quicker.")]
    private float acceleration = 1f;


    public float maxMoveSpeed => this.MaxMoveSpeed;

    
     public override void Activate(UnitController owner, Vector3 target)
    {
        Vector3 origin = owner.transform.position;
        
        owner.GetComponent<Rigidbody>().velocity = new Vector3(1,1,1);

        owner.unitMover.canMove = false;
        Vector3 direction = (target-origin).normalized*acceleration;
        
            
        
        
        
    }
}