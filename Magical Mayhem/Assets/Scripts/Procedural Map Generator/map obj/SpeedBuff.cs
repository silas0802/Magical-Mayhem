using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

public class SpeedBuff : NetworkBehaviour
{
    private readonly float speed = 2.5f;
    private readonly float acceleration = 1.5f;
    private readonly int cd = 7;
    private readonly int activetime = 2; 
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

    private IEnumerator SetMaxSpeedback(float speed, int activetime, Collider player){
        yield return new WaitForSeconds(activetime);
        player.GetComponent<UnitMover>().BuffSpeed(-speed, -acceleration);
    }

    private void OnTriggerEnter(Collider other){
       if(IsServer){
            if(other != null){
                NowYouDontClientRPC();
                other.GetComponent<UnitMover>().BuffSpeed(speed, acceleration);
                StartCoroutine(Cooldown(cd));
                StartCoroutine(SetMaxSpeedback(speed, activetime, other));
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
