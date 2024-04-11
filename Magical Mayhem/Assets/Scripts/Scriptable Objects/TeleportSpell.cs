using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Teleport Spell", menuName = "Game/Spells/Teleport Spell")]

public class TeleportSpell : Spell
{
    [SerializeField,Range(0,30)] private float Range = 10f;
    [SerializeField,Range(0,5)] private float VFXLifetime = 1;
    [SerializeField] private TeleportInstance TeleportVFX;

    public float vfxLifetime => VFXLifetime;
    public float range => this.Range;
    public TeleportInstance teleportVFX => this.TeleportVFX;
    public override void Activate(UnitController owner, Vector3 target)
    {
        TeleportInstance n1 = Instantiate(teleportVFX, owner.transform.position, Quaternion.identity);
        n1.InitializeSpell(this);
        Vector3 distanceVec = target - owner.transform.position;
        if (distanceVec.magnitude > range){
            owner.transform.position += distanceVec.normalized*range;
        }
        else{
            owner.transform.position = target;
        }
        TeleportInstance n2 = Instantiate(teleportVFX, owner.transform.position, Quaternion.identity);
        n2.InitializeSpell(this);
    }
}
