using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LavaTileScript : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Lava";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
