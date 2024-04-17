using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
[CreateAssetMenu(fileName = "New BlackHole Spell", menuName = "Game/Spells/BlackHole Spell")]
public class BlackHoleSpell : Spell
{
    
    

    // Start is called before the first frame update
    [SerializeField,Range(1,10),Tooltip("The size of the black hole")] 
    private float AreaSize = 10;

    [SerializeField,Range(1,10),Tooltip("the duration the spell will stay")] 
    private float Duration = 10;

    [SerializeField,Range(1,5),Tooltip("How fast player caught in it will be sucked into the middle")] 
    private float Suction = 10;

    [SerializeField,Range(1,10),Tooltip("how far away from target the player can cast the spell")] 
    private float Range = 10;

    [SerializeField] private BlackHoleInstance BlackHoleInstance;

    public float areaSize => this.AreaSize;
    public float duration => this.Duration;
    public float suction => this.Suction;

    public float range => this.Range;
    public BlackHoleInstance blackHoleInstance => this.BlackHoleInstance;

    // Update is called once per frame
    

    public override void Activate(UnitController owner, Vector3 target)
    {
        BlackHoleInstance bli = Instantiate(BlackHoleInstance,target,quaternion.identity);
        bli.Initialize(this, owner);
    }
}
