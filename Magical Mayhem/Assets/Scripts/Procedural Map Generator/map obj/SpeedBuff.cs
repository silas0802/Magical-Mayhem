using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public class SpeedBuff : NetworkBehaviour
{
    private float speed = 0;
    private float cooldown = 0;
    private float activetime = 3; 
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
            //other.gameObject.
        }
    }
}
