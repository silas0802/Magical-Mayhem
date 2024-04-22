using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBeamInstance : NetworkBehaviour
{   
    LaserBeamSpell laser;
    UnitController owner;
    Vector3 target;
    float timeLeft;
    // Start is called before the first frame update
    public void Initialize(LaserBeamSpell laser,UnitController owner,Vector3 target){
        this.laser = laser;
        this.owner=owner;
        this.target=target;
        timeLeft = laser.duration;
        Vector3 box = new Vector3(laser.width, laser.lenght,2);
        //Collider[] hits = Physics.OverlapBox(owner.transform.position, box, owner.transform.rotation);
        //Collider[] hits = Physics.SphereCast(owner.transform.position,laser.width,(target-owner.transform.position).normalized);
        GetComponent<NetworkObject>().Spawn();
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            timeLeft-=Time.deltaTime;
            if (timeLeft<0)
            {
                Destroy(gameObject);
                GetComponent<NetworkObject>().Despawn();
            }

            AttackRay();
        }
    }
    public void OnDrawGizmos(){
    
    }

    private void AttackRay(){
        bool validTarget;
        Vector3 newTarget = HelperClass.GetMousePosInWorld(out validTarget);
        RaycastHit[] rayHits = Physics.SphereCastAll(owner.transform.position,laser.width,(newTarget-owner.transform.position).normalized,laser.lenght);
        transform.rotation = Quaternion.LookRotation((newTarget-owner.transform.position).normalized);
        //RaycastHit[] rayHits = Physics.SphereCastAll(owner.transform.position,laser.width,(target-owner.transform.position).normalized,laser.lenght);
        //transform.rotation = Quaternion.LookRotation((target-owner.transform.position).normalized);
        Collider[] colliderHits = ConvertRayToCollider(rayHits);
        DealDamage(colliderHits);
        

    }
    
    
    

    private void DealDamage(Collider[] colliderHits){
        foreach (Collider victim in colliderHits)
        {
            UnitController hit = victim.GetComponent<UnitController>();
            if (hit!=null&&hit!=owner)
            {

                victim.GetComponent<IDamagable>().ModifyHealth(owner,-laser.damagePerSecond);
            }
            
        }
    }

    private Collider[] ConvertRayToCollider(RaycastHit[] hits){
        
        Collider[] colliders = new Collider[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            colliders[i] = hits[i].collider;
        }
        return colliders;
    }
}
