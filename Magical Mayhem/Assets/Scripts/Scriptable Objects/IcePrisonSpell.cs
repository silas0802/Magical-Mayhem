using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

using UnityEngine;
[CreateAssetMenu(fileName = "New IcePrisonSpell", menuName = "Game/Spells/IcePrisonSpell")]
public class IcePrisonSpell : Spell
{

    [SerializeField,Range(1,5),Tooltip("the radius of the area")]
    private float Radius = 4f;

    [SerializeField,Range(1,5),Tooltip("the duration that the enemies are stuck")]
    private float RootDuration= 4f;

    [SerializeField,Range(1,5),Tooltip("Damage deal")]
    private int Damage =5;

    [SerializeField,Range(1,5),Tooltip("The second it takes from the initial shown area until the blast hits")]
    private float ActivationTime =1f;

    [SerializeField,Range(1,10),Tooltip("The range the the person can cast it from ")]
    private float Range =4f;

    [SerializeField,Tooltip("IcePrisonInstance")] private IcePrisonInstance IcePrisonInstance;
    public float radius =>this.Radius;
    public float rootDuration => this.RootDuration;
    public float activationTime => this.ActivationTime;
    public int damage => this.Damage;

    public float range => this.Range;
    public IcePrisonInstance icePrisonInstance=> this.IcePrisonInstance;
    public override void Activate(UnitController owner, Vector3 target)
    {
        if ((target-owner.transform.position).magnitude > range)
        {
            IcePrisonInstance iPS = Instantiate(IcePrisonInstance,(target-owner.transform.position).normalized*range,quaternion.identity);
            iPS.Initialize(this,owner,(target-owner.transform.position).normalized*range);
        }else{
            IcePrisonInstance iPS = Instantiate(IcePrisonInstance,target,quaternion.identity);
            iPS.Initialize(this,owner,target);
        }
        
    }

    
}
