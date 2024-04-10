using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TileScript : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Floor";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
