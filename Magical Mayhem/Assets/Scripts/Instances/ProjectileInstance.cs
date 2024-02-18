using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
/// <summary>
///  - Silas Thule
/// </summary>
public class ProjectileInstance : NetworkBehaviour
{
    UnitController owner;
    ProjectileSpell spell;
    Vector3 startPos;
    float range;
    Rigidbody rb;
    float unitTimer;
    UnitController unitTarget;

    public void Initialize(ProjectileSpell spell, Vector3 target, UnitController owner)
    {
        this.spell = spell;
        this.owner = owner;
        startPos = transform.position;
        Vector3 direction = (target-startPos).normalized;
        direction.y = 0f;
        rb = GetComponent<Rigidbody>();
        if (spell.homingForce > 0)
        {
            range = spell.range;
        }
        else
        {
            range = Mathf.Min((target - startPos).magnitude, spell.range);
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

    private void Detonate()
    {
        spell.endEffect.Activate(owner, transform.position);
        GetComponent<NetworkObject>().Despawn();
    }
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
}
