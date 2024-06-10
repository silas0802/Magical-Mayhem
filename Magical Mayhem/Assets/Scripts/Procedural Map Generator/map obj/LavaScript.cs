using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour
{

    public void OnTriggerEnter(Collider player){
        if(player.gameObject.GetComponent<IDamagable>() != null){
            player.gameObject.GetComponent<UnitController>().SetInLavaBool(true);
        }
    }
    
}
