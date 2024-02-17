using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
/// <summary>
/// Controls the movement of unit. - Silas Thule
/// </summary>
[RequireComponent(typeof(UnitController),typeof(Rigidbody))]
public class UnitMover : NetworkBehaviour
{
    [SerializeField, Tooltip("A value made for testing. Should be removed when movement system has been converted to use acceleration and friction")] 

    

    public Vector3 targetPosition {get; private set;}
    private Rigidbody rb;
    private UnitController controller;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<UnitController>();
    }
    

    /// <summary>
    /// Sets the velocity of the rigidbody based on the current position and target position. Can only be called from server.
    /// </summary>
    public void Move(){
        //Debug.Log((targetPosition-transform.position).normalized*moveSpeed);
        if ((targetPosition-transform.position).magnitude<controller.unitClass.acceptingDistance){
            if (rb.velocity.magnitude<controller.unitClass.maxSpeed+0.1f)
            {
                rb.velocity*=1-Time.deltaTime*10;   
            }
        }
        else{
            Vector3 inputtedVel = (targetPosition-transform.position).normalized*controller.unitClass.acceleration;
            Vector3 givenVel = rb.velocity+inputtedVel;
            if (givenVel.magnitude<rb.velocity.magnitude){
                rb.velocity = givenVel;
            }
            else if (rb.velocity.magnitude<controller.unitClass.maxSpeed){
                rb.velocity = givenVel.magnitude > controller.unitClass.maxSpeed ? givenVel.normalized*controller.unitClass.maxSpeed : givenVel;
            }
        }
        
    }
    private void Update(){
        if (IsServer){
            rb.velocity-=rb.velocity.normalized*controller.unitClass.friction*Time.deltaTime;
        }
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
