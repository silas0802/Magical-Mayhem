using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rewind Spell", menuName = "Game/Spells/Rewind Spell")]
public class TimeRewindSpell : Spell
{
    
    [SerializeField, Range(0, 5), Tooltip("How much time needs to pass before rewind takes place")] private float Delay;
    public override void Activate(UnitController owner, Vector3 target)
    {
        owner.StartCoroutine(Rewind(owner, owner.GetHealth(),owner.transform.position));
    }

    IEnumerator Rewind(UnitController owner, int startHP, Vector3 startPos)
    {
        yield return new WaitForSeconds(Delay);
        if (owner != null)
        {
            owner.transform.position = startPos;
            owner.ModifyHealth(owner, startHP - owner.GetHealth());
        }

    }

    
}
