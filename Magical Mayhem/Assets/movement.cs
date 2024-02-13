using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool inJump = false;
    private int numberOfJumps = 0;
    Vector2 inputDir;
    Rigidbody rig;
    // Start is called before the first frame update
    void Start()
    {
        rig=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfJumps==0)
        {
            rig.AddForce(new Vector3(inputDir.x,0,inputDir.y)*moveSpeed*Time.deltaTime);    
        }
        
        
    }
    public void OnMove(InputValue dir){
        inputDir = dir.Get<Vector2>();
    }

    public void OnJump(){
        
        if (numberOfJumps<2)
        {
            numberOfJumps++;
            rig.AddForce(new Vector3(0,300,0));    
            
        }
        

    }
    void OnCollisionEnter(){
        numberOfJumps=0;
    }
}
