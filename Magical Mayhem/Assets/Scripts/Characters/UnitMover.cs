using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
/// <summary>
/// Controls the movement of unit. - Silas Thule
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class UnitMover : NetworkBehaviour
{
    [SerializeField, Tooltip("A value made for testing. Should be removed when movement system has been converted to use acceleration and friction")] 
    private float moveSpeed = 5;       //Made for testing and should be removed when movement gets converted to use acceleration and friction

    

    private Vector3 targetPosition;
    private Rigidbody rb;

    void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    

    /// <summary>
    /// Sets the velocity of the rigidbody based on the current position and target position. Can only be called from server.
    /// </summary>
    public void Move(){
        //Debug.Log((targetPosition-transform.position).normalized*moveSpeed);
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
    /// <summary>
    /// Sets the target position of the unit to the given Vector3. Can only be called from server.
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetTargetPosition(Vector3 targetPosition){
        this.targetPosition = targetPosition;
    }
}
