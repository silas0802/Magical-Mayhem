using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class BlackHoleInstance : MonoBehaviour
{
    private float timeLeft;
    public float accteptingDistance=0.3f;
    BlackHoleSpell blackHoleSpell;
    // Start is called before the first frame update
    public void Initialize(BlackHoleSpell blackHoleSpell,UnitController owner){
        this.blackHoleSpell=blackHoleSpell;


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
            if (victim.GetComponent<UnitController>()!=null)
            {
                
            
            if ((transform.position-victim.transform.position).magnitude>accteptingDistance)
            {
                victim.GetComponent<Rigidbody>().velocity+=(transform.position-victim.transform.position).normalized*blackHoleSpell.suction*Time.deltaTime;
            }
            }
            
        }
    }
}
