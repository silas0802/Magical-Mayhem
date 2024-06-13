using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Barrier Spell", menuName ="Game/Spells/Barrier Spell")]
public class BarrierSpell : Spell
{
    [SerializeField] private float Duration;
    [SerializeField] private BarrierInstance Instance;

    public float duration => this.Duration;
    public override void Activate(UnitController owner, Vector3 target)
    {
        owner.hasBarrier = true;
        BarrierInstance inst = Instantiate(Instance, owner.transform.position, Quaternion.identity);
        inst.InitializeSpell(this, owner);
        owner.StartCoroutine(Barrier(owner));
    }
    IEnumerator Barrier(UnitController owner)
    {
        yield return new WaitForSeconds(Duration);
        owner.hasBarrier = false;
    }
    
}
