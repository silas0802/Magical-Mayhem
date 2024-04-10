using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthBuff : NetworkBehaviour
{   
    private int health = 10;
    private float cooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cooldown <= 0){
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<BoxCollider>().enabled = true;
        }   
        cooldown -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other){
        if(IsServer){
            IDamagable player = other.gameObject.GetComponent<IDamagable>();
            if(player != null){
                cooldown = 7f;
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().enabled = false;
                player.ModifyHealth(other.GetComponent<UnitController>(), health);
            }
        }
    }
}
