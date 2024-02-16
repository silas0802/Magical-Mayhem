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

    public void Initialize(ProjectileSpell spell, Vector3 target, UnitController owner)
    {
        this.spell = spell;
        this.owner = owner;
        startPos = transform.position;
        range = Mathf.Min((target-startPos).magnitude,spell.range);
        Vector3 direction = (target-startPos).normalized;
        

        GetComponent<NetworkObject>().Spawn(); //Start network syncronization
        GetComponent<Rigidbody>().velocity = direction*spell.speed;

    }
    

    // Update is called once per frame
    void Update()
    {
        if (IsServer && (startPos-transform.position).magnitude>range){
            spell.endEffect.Activate(owner, transform.position);
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
