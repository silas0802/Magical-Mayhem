using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnitMover : NetworkBehaviour
{
    [SerializeField, Tooltip("A value made for testing. Should be removed")] 
    private float moveSpeed = 5;       //Made for testing and should be removed when movement gets converted to use acceleration and friction

    

    private Vector3 targetPosition;
    private Rigidbody rb;

    void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    


    public void Move(){
        Debug.Log((targetPosition-transform.position).normalized*moveSpeed);
        rb.velocity = (targetPosition-transform.position).normalized*moveSpeed;
    }

    [ServerRpc]
    public void MoveServerRPC()
    {
        Move();
    }
    [ServerRpc]
    public void SetTargetPositionServerRPC(Vector3 targetPosition)
    {
        SetTargetPosition(targetPosition);
    }
    public void SetTargetPosition(Vector3 targetPosition){
        this.targetPosition = targetPosition;
    }
}
