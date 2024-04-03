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
    [Header("Movement Statistics")]

    [SerializeField, Range(0, 2f), Tooltip("A higher value will make the unit get up to maxSpeed quicker.")]
    private float acceleration = 1f;

    [SerializeField, Range(0, 5f), Tooltip("A higher value will slow down the unit quicker.")]
    private float frictionFlat = 2f;

    [SerializeField, Range(0, 0.6f), Tooltip("A higher value will slow down the unit quicker.")]
    private float frictionMult = 0.1f;

    [SerializeField, Range(0, 10f), Tooltip("The max speed the unit can go by walking. This can be exceeded through knockbacks.")]
    private float maxSpeed = 5f;

    [SerializeField, Range(0, 1f), Tooltip("The distance from target position that the unit accepts as close enough")]
    private float acceptingDistance = 0.1f;
    [SerializeField, Range(0, 30f), Tooltip("A higher value makes the unit stop quicker when within acceptingDistance")]
    private float slowDownMult = 10f;

    [SerializeField]public bool canMove = true;


    


    public Vector3 targetPosition {get; private set;}
    private Rigidbody rb;
    private UnitController controller;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<UnitController>();
    }
    

    /// <summary>
    /// Sets the velocity of the rigidbody based on the current velocity, position and target position. Can only be called from server.
    /// </summary>
    public void Move(){
        if (controller.isDead) return;
        //Debug.Log((targetPosition-transform.position).normalized*moveSpeed);
        if ((targetPosition-transform.position).magnitude<acceptingDistance||!canMove){
            if (rb.velocity.magnitude<maxSpeed +0.1f)
            {
                rb.velocity*=1-Time.deltaTime*slowDownMult;   
            }
        }
        else{
            Vector3 inputtedVel = (targetPosition-transform.position).normalized*acceleration;
            Vector3 givenVel = rb.velocity+inputtedVel;
            if (givenVel.magnitude<rb.velocity.magnitude){
                rb.velocity = givenVel;
            }
            else if (rb.velocity.magnitude<maxSpeed){
                rb.velocity = givenVel.magnitude > maxSpeed ? givenVel.normalized*maxSpeed : givenVel;
            }
        }
        transform.position = new Vector3 (transform.position.x,0,transform.position.z);
        
    }
    private void Update(){
        if (controller.IsClient && controller.IsLocalPlayer)   //Ask server to move your unit if you are client
        {
            controller.unitMover.MoveServerRPC();
        }
        if (IsServer){
            rb.velocity*=1-frictionMult;
            rb.velocity-=rb.velocity.normalized*frictionFlat *Time.deltaTime;
        }
    }
    /// <summary>
    /// Requests server to move this character for client.
    /// </summary>
    [ServerRpc]
    public void MoveServerRPC()
    {
        Move();
    }

    /// <summary>
    /// Sets target position on server side (as its the server that needs to know where you want to go)
    /// </summary>
    /// <param name="targetPosition"></param>
    [ServerRpc]
    public void SetTargetPositionServerRPC(Vector3 targetPosition)
    {
        SetTargetPosition(targetPosition);
    }
    /// <summary>
    /// Sets the local target position of the unit to the given Vector3.
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetTargetPosition(Vector3 targetPosition){
        this.targetPosition = targetPosition;
    }
}
