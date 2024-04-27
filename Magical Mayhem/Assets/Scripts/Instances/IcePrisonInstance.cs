using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class IcePrisonInstance : NetworkBehaviour
{
    [SerializeField,Tooltip("Explosion")] private ParticleSystem Explosion;
    [SerializeField,Tooltip("RootInstance")] private RootInstance Rootinstance;
    IcePrisonSpell iPS;
    UnitController owner;
    Vector3 target;
    bool explosionOccured=false;
    float timeRemainAfterImpact=1;
    float timeRemaining;
    public ParticleSystem explosion => Explosion;
    public RootInstance rootInstance=> Rootinstance;
    ParticleSystem a;
    public void Initialize(IcePrisonSpell iPS,UnitController owner, Vector3 target){
        this.iPS = iPS;
        this.owner = owner;
        this.target = target;
        timeRemaining = iPS.activationTime;

        GetComponent<NetworkObject>().Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            
        
            timeRemaining-=Time.deltaTime;
            if (timeRemaining < 0&&!explosionOccured)
            {
                a = Instantiate(explosion,target,quaternion.identity);
               a.GetComponent<NetworkObject>().Spawn();
                explosionOccured=true;
                IceExplosion();

                
            }
            if (explosionOccured)
            {
                timeRemainAfterImpact-=Time.deltaTime;
            }
            if (timeRemainAfterImpact<0)
            {
                Destroy(a);
                Destroy(gameObject);
                GetComponent<NetworkObject>().Despawn();
                a.GetComponent<NetworkObject>().Despawn();
            }
        }
        
    }
    void OnDrawGizmos(){

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target,iPS.radius);
    

    }
    private void IceExplosion(){

        Collider[] hits = Physics.OverlapSphere(target, iPS.radius);
        
        foreach ( Collider rootMen in hits)
        {
            UnitController hit = rootMen.GetComponent<UnitController>();
            if (hit!=null&&hit!=owner)
            {   
                hit.unitMover.canMove=false;
                rootMen.GetComponent<IDamagable>().ModifyHealth(owner,-iPS.damage);
                //call root instance with a unitcontroller, position and time rooted i guess
                RootInstance rI = Instantiate(Rootinstance,hit.transform.position,Quaternion.identity);
                rI.Initialize(hit,iPS.rootDuration);
                
            }            
        }
    }

}
