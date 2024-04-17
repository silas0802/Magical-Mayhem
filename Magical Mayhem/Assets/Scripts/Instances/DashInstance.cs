using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DashInstance : NetworkBehaviour
{
     DashSpell dashSpell;
     UnitController owner;
     Vector3 target;
     Vector3 origin;
     Vector3 direction;

     float acceptingDistance = 0.3f;
    public void Initialize(DashSpell dashSpell,UnitController owner,Vector3 target){
        this.dashSpell = dashSpell;
        this.owner = owner;
        this.target = target;  
        owner.unitMover.canMove = false;
        origin = owner.transform.position;
        direction = (target-origin).normalized*dashSpell.maxMoveSpeed;
        owner.GetComponent<Rigidbody>().velocity = direction;
        GetComponent<NetworkObject>().Spawn();
        transform.parent = owner.transform;
        
    }
    
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {   owner.GetComponent<Rigidbody>().velocity = direction;
        
         if ((origin-owner.transform.position).magnitude>dashSpell.range||(target - owner.transform.position).magnitude<acceptingDistance  )
        {
            owner.GetComponent<Rigidbody>().velocity = Vector3.zero;
            owner.unitMover.canMove=true; 
            Debug.Log(owner.unitMover.canMove); 
            owner.unitMover.ReachTarget();
            Destroy(gameObject);
            GetComponent<NetworkObject>().Despawn();
        }
        
    }
}
