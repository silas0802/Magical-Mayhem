using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class BlackHoleInstance : MonoBehaviour
{
    private float timeLeft;
    public float accteptingDistance=0.5f;
    
    BlackHoleSpell blackHoleSpell;
    UnitController owner;
    // Start is called before the first frame update
    public void Initialize(BlackHoleSpell blackHoleSpell,UnitController owner){
        this.blackHoleSpell=blackHoleSpell;
        this.owner=owner;

        timeLeft = blackHoleSpell.duration;

        Collider[] hits = Physics.OverlapSphere(transform.position, blackHoleSpell.areaSize);
        GetComponent<NetworkObject>().Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft-=Time.deltaTime;
        SuckingCommenced();
        if (timeLeft<0)
        {
            Destroy(gameObject);
            GetComponent<NetworkObject>().Despawn();
        }
    }

  

    public void SuckingCommenced(){

        Collider[] hits = Physics.OverlapSphere(transform.position, blackHoleSpell.areaSize);
        foreach (Collider victim in hits)
        {   
            UnitController hit = victim.GetComponent<UnitController>();
            if (hit!=null&&hit!=owner)
            {
                
            
            if ((transform.position-victim.transform.position).magnitude>accteptingDistance)
            {
               Vector3 suckSpeed =(transform.position-victim.transform.position).normalized*Time.deltaTime*(blackHoleSpell.suction*10)/((transform.position-victim.transform.position).magnitude*(transform.position-victim.transform.position).magnitude);
               Vector3 Cap = (transform.position-victim.transform.position).normalized*Time.deltaTime*(blackHoleSpell.suction*10)/accteptingDistance;
               if (suckSpeed.magnitude>Cap.magnitude)
               {
                victim.GetComponent<Rigidbody>().velocity+=Cap;
               }else
               {
                victim.GetComponent<Rigidbody>().velocity+=suckSpeed;
               }
               
               //victim.GetComponent<Rigidbody>().velocity+=(transform.position-victim.transform.position).normalized*Time.deltaTime*(blackHoleSpell.suction*10)/((transform.position-victim.transform.position).magnitude*(transform.position-victim.transform.position).magnitude);
            }
            }
            
        }
    }
}
