using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Teleport Spell", menuName = "Game/Spells/Teleport Spell")]

public class TeleportSpell : Spell
{
    [SerializeField,Range(0,30)] private float Range = 10f;
    [SerializeField] private GameObject TeleportVFX;

    public float range => this.Range;
    public GameObject teleportVFX => this.TeleportVFX;
    public override void Activate(UnitController owner, Vector3 target)
    {
        throw new System.NotImplementedException();
    }
}
