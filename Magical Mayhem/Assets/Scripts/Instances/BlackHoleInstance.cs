using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class BlackHoleInstance : MonoBehaviour
{
    private float timeLeft;
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

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {   
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, blackHoleSpell.areaSize);
       
    }
    #endif

    public void SuckingCommenced(){

        Collider[] hits = Physics.OverlapSphere(transform.position, blackHoleSpell.areaSize);
        foreach (Collider victim in hits)
        {
            
        }
    }
}
