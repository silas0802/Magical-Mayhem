using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider player){
        if(player.gameObject.GetComponent<IDamagable>() != null){
            player.gameObject.GetComponent<UnitController>().SetInLavaBool(true);
        }
    }
    public void OnTriggerExit(Collider player){
        if(player.gameObject.GetComponent<IDamagable>() != null){
            player.gameObject.GetComponent<UnitController>().SetInLavaBool(false);
        }
    }
}
