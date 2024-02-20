using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Multi Cast Spell", menuName = "Game/Spells/Multi Cast")]
public class MultiCastSpell : Spell
{
    [SerializeField] private Spell SpellToCast;
    [SerializeField, Range(0, 20), Tooltip("How many times the spell that will be cast.")] 
    private int Amount = 2;

    [SerializeField, Range(0,180), Tooltip("How many angles the projectiles might deviate from target by.")] 
    private float Spray = 0;

    [SerializeField, Range(0,0.5f), Tooltip("Time between each consecutive cast.")] 
    private float TimeInterval = 0;

    public Spell spellToCast => this.SpellToCast;
    public int amount => this.Amount;
    public float spray => this.Spray;
    public float timeInterval => this.TimeInterval;
    public override void Activate(UnitController owner, Vector3 target)
    {
        owner.StartCoroutine(CastWithDelay(owner, target));
    }
    IEnumerator CastWithDelay(UnitController owner, Vector3 target)
    {
        for (int i = 0; i < this.amount; i++)
        {
            Vector3 dir = target - owner.transform.position;
            float rotation = Random.Range(-spray, spray);
            dir = Quaternion.AngleAxis(rotation, Vector3.up) * dir;
            spellToCast.Activate(owner, dir);
            yield return new WaitForSeconds(timeInterval);
        }
    }
}


