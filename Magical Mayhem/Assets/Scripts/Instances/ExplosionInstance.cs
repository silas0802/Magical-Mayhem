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
                Collider[] hits = Physics.OverlapSphere(transform.position, spell.radius);

            }
            else if (timer < -spell.lifeTime){
                GetComponent<NetworkObject>().Despawn();
                Destroy(gameObject);
            }
        }
    }
}
