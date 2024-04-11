using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Teleport Spell", menuName = "Game/Spells/Teleport Spell")]

public class TeleportSpell : Spell
{
    [SerializeField,Range(0,30)] private float Range = 10f;
    [SerializeField] private NetworkObject TeleportVFX;

    public float range => this.Range;
    public NetworkObject teleportVFX => this.TeleportVFX;
    public override void Activate(UnitController owner, Vector3 target)
    {
        NetworkObject n1 = Instantiate(teleportVFX, owner.transform.position, Quaternion.identity);
        NetworkObject n2 = Instantiate(teleportVFX, target, Quaternion.identity);
        n1.Spawn();
        n2.Spawn();
        Vector3 distanceVec = target - owner.transform.position;
        if (distanceVec.magnitude > range){
            owner.transform.position = owner.transform.position + distanceVec.normalized*range;
        }
        else{
            owner.transform.position = owner.transform.position + distanceVec;
        }
        owner.transform.position = target;
    }
}
