using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Teleport Spell", menuName = "Game/Spells/Teleport Spell")]

public class TeleportSpell : Spell
{
    [Range(0,30)] public float Range = 10f;
    public GameObject TeleportVFX;
    public override void Activate(UnitController owner, Vector3 target)
    {
        throw new System.NotImplementedException();
    }
}
