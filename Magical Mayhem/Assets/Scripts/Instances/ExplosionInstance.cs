using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
/// <summary>
///  - Silas Thule
/// </summary>
public class ExplosionInstance : NetworkBehaviour
{
    ExplosionSpell spell;
    UnitController owner;
    float timer;
    bool hasTriggered = false;

    public void Initialize(ExplosionSpell spell,UnitController owner)
    {
        this.spell = spell;
        this.owner = owner;
        timer = spell.delay;

        GetComponent<NetworkObject>().Spawn(); //Start network syncronization
    }
    // Update is called once per frame
    void Update()
    {
        if (IsServer){
            timer -= Time.deltaTime;
            if (timer<0&& !hasTriggered)
            {
                hasTriggered = true;
                Detonate();
            }
            if (timer < -spell.lifeTime){
                GetComponent<NetworkObject>().Despawn();
                Destroy(gameObject);
            }
        }
    }

    void Detonate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, spell.radius);
        
        foreach (Collider hit in hits)
        {
            if (hit.GetComponent<UnitController>() == owner)
            {
                continue;
            }
            
            hit.GetComponent<IDamagable>()?.ModifyHealth(owner,-spell.damage);
            Vector3 dir = hit.transform.position-transform.position;
            dir = new Vector3(dir.x,0,dir.z);
            hit.GetComponent<UnitController>()?.unitMover.ApplyKnockBack(dir.normalized * spell.knockback);

        }
    }
}
