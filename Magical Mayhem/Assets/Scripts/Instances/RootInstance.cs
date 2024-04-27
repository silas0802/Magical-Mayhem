using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RootInstance : NetworkBehaviour
{
    
    float timeLeftofRoot;
    UnitController victim;
    // Start is called before the first frame update
   public void Initialize(UnitController victim,float time){
        this.timeLeftofRoot=time;
        this.victim=victim;

        GetComponent<NetworkObject>().Spawn();

   }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            
        
            timeLeftofRoot-=Time.deltaTime;
            if (timeLeftofRoot<0)
            {
                
                Destroy(gameObject);
                GetComponent<NetworkObject>().Despawn();
                victim.unitMover.canMove=true;
            }
        }
        
    }
}
