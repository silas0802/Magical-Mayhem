using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Unity.Netcode;
public class CameraBehaviour : NetworkBehaviour
{
   
    public static CameraBehaviour instance;
    UnitController controller; 
    Vector3 offset;
    Quaternion rotation;

    public void ConnectPlayer(UnitController controller){
        this.controller = controller;
    }

    void Awake(){
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position;
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller is not null)
        {
            transform.position = controller.transform.position+offset;    
        }
        
    }
}
