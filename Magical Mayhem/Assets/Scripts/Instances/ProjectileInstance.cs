using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
/// <summary>
///  - Silas Thule
/// </summary>
public class ProjectileInstance : NetworkBehaviour
{
    public UnitController owner { get; private set; }
    ProjectileSpell spell;
    Vector3 startPos;
    float range;
    Rigidbody rb;
    float unitTimer;
    UnitController unitTarget;
    bool isThreat = false;

    /// <summary>
    /// Initialize the projectile with inputted information.
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="target"></param>
    /// <param name="owner"></param>
    public void Initialize(ProjectileSpell spell, Vector3 target, UnitController owner)
    {
        this.spell = spell;
        this.owner = owner;
        startPos = transform.position;
        Vector3 direction = (target-startPos).normalized;
        direction.y = 0f;
        rb = GetComponent<Rigidbody>();
        GetComponent<SphereCollider>().radius = spell.triggerRadius;
        if (spell.homingForce > 0)
        {
            range = spell.range;
        }
        else
        {
            if (spell.mustFlyMaxDistance)
            {
                range = spell.range;
            }
            else
            {
                range = Mathf.Min((target - startPos).magnitude, spell.range);
            }
        }



        GetComponent<NetworkObject>().Spawn(); //Start network syncronization
        rb.velocity = direction*spell.speed;

    }
    

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (spell.homingForce == 0 && (startPos - transform.position).magnitude > range) //if homing is disabled and projectile has travelled too far.
            {
                Detonate();
            }
            else if (spell.homingForce > 0) //if homing
            {
                Homing();
            }
        }
        
    }
    /// <summary>
    /// Calls its endEffect Activation and Despawns this object. Server Only.
    /// </summary>
    private void Detonate()
    {
        spell.endEffect.Activate(owner, transform.position);
        GetComponent<NetworkObject>().Despawn();
    }
    /// <summary>
    /// Is called in update if homing is enabled.
    /// </summary>
    private void Homing()
    {
        unitTimer-=Time.deltaTime;
        range -= Time.deltaTime;
        if (unitTimer < 0)  //scan for nearest enemy
        {
            unitTarget = RoundManager.instance.FindNearestUnit(transform.position,owner);
            unitTimer = 0.25f;
            
        }
        if (unitTarget != null) //Math to figure out trajectory
        {
            Vector3 dir = (unitTarget.transform.position - transform.position).normalized * Time.deltaTime * spell.homingForce;
            dir.y = 0;
            dir = dir + rb.velocity;
            if (dir.magnitude > spell.speed)
            {
                dir = dir.normalized*spell.speed;
            }
            rb.velocity = dir;
        }
        
        if (range < 0)
        {
            Detonate();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            IDamagable hit = other.gameObject.GetComponent<IDamagable>();
            if (hit != null)
            {
                if (hit != (IDamagable)owner)
                {
                    Detonate();
                }
            }
        }
    }
    #if UNITY_EDITOR
    void OnDrawGizmos() 
    {   
        Handles.DrawWireDisc(transform.position, Vector3.up, spell.triggerRadius * 2);

        if (isThreat) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position,rb.velocity.normalized * 2);
        } 
        else 
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, rb.velocity.normalized * 2);
        }
    }
    #endif
}

