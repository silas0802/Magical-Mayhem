using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthBuff : NetworkBehaviour
{   
    private readonly int health = 10;
    private readonly int cd = 7;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private IEnumerator Cooldown(int cd){
        yield return new WaitForSeconds(cd);
        NowYouSeeMeClientRPC();
    }

    private void OnTriggerEnter(Collider other){
        if(IsServer){
            IDamagable player = other.gameObject.GetComponent<IDamagable>();
            if(player != null){
                NowYouDontClientRPC();
                player.ModifyHealth(other.GetComponent<UnitController>(), health);
                StartCoroutine(Cooldown(cd));
            }
        }
    }

    [ClientRpc]
    private void NowYouDontClientRPC(){
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
    }
    [ClientRpc]
    private void NowYouSeeMeClientRPC(){
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
    }
}
