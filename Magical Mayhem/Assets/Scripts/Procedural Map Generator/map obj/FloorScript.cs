using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    // Start is called before the first frame update
    
    public void OnTriggerEnter(Collider player){
            if(player.gameObject.GetComponent<IDamagable>() != null){
                player.gameObject.GetComponent<UnitController>().SetInLavaBool(false);
            }
        }
}
