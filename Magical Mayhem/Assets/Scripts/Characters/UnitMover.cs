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

    [SerializeField, Range(0, 20f), Tooltip("How quickly the body rotates")]
    private float rotationSpeed = 1f;

    [SerializeField, Range(0, 20f), Tooltip("How quickly the animation changes")]
    private float animationLerpSpeed = 4f;

    [SerializeField, Range(0, 10f), Tooltip("A higher value will make the unit get up to maxSpeed quicker.")]
    private float acceleration = 1f;

    [SerializeField, Range(0, 10f), Tooltip("A higher value will make the unit slow down quicker when reaching it's target.")]
    private float decceleration = 1f;

    //[SerializeField, Range(0, 5f), Tooltip("A higher value will slow down the unit quicker.")]
    //private float frictionFlat = 2f;

    //[SerializeField, Range(0, 20f), Tooltip("A higher value will slow down the unit quicker.")]
    //private float frictionMult = 0.1f;

    [SerializeField, Range(0, 10f), Tooltip("The max speed the unit can go by walking. This can be exceeded through knockbacks.")]
    private float maxSpeed = 5f;

    [SerializeField, Range(0, 1f), Tooltip("The distance from target position that the unit accepts as close enough")]
    private float acceptingDistance = 0.1f;
    [SerializeField, Range(0, 30f), Tooltip("A higher value makes the unit stop quicker when within acceptingDistance")]
    private float knockBackDecceleration = 2f;

    [SerializeField] public bool canMove = true;
    [SerializeField] private bool hasReached = true;
    [SerializeField] private bool isKnockedBack = false;




    public Vector3 targetPosition {get; private set;}
    private Rigidbody rb;
    private UnitController controller;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<UnitController>();
    }
    

    /// <summary>
    /// Sets the velocity of the rigidbody based on the current velocity, position and target position. Also handles rotation. Can only be called from server.
    /// </summary>
    public void Move()
    {
        if (!IsServer) { throw new NotServerException("Can't Apply Knockback if not Server"); }
        if (controller.isDead) { rb.velocity = Vector3.zero; return; } //Cant move if dead
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;
        direction = direction.normalized;
        float currentVel = rb.velocity.magnitude;
        if (distance < acceptingDistance)
        {
            ReachTarget();
        }
        if (!isKnockedBack)
        {
            //Normal Movement
            if (!hasReached)    
            {
                //walking
                rb.velocity = Vector3.Lerp(rb.velocity, direction * maxSpeed, acceleration / 60);
            }
            else
            {
                //slowing down
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, decceleration / 60);
            }
        }
        else
        {
            //Knockback Movement
            if (currentVel < 1f)
            {
                isKnockedBack = false;
            }
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, knockBackDecceleration / 60);
            
        }
        



        transform.position = new Vector3(transform.position.x, 0, transform.position.z); //clamp y position to 0
        HandleRotation();
        HandleWalkingAnimation();
        
    }
    private void FixedUpdate(){
        //if (controller.IsClient && controller.IsLocalPlayer)   //Ask server to move your unit if you are client
        //{
        //    controller.unitMover.MoveServerRPC();
        //}
        if (IsServer){
            Move();
        }
        
    }
    /// <summary>
    /// Applies Knockback to unit. Must only be called from server
    /// </summary>
    public void ApplyKnockBack(Vector3 forceVector)
    {
        if (!IsServer) { throw new NotServerException("Can't Apply Knockback if not Server"); }
        rb.velocity += forceVector;
        //rb.AddForce(ForceVector,ForceMode.Impulse);
        isKnockedBack = true;
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
    public void SetTargetPosition(Vector3 targetPosition)
    {
        if (!IsServer) { throw new NotServerException("Can't Apply Knockback if not Server"); }
        this.targetPosition = targetPosition;
        hasReached = false;
    }
    private void HandleWalkingAnimation()
    {
        if (rb.velocity.magnitude > 0.1f) // if moving do walking animation
        {
            float lerpedVal = Mathf.Lerp(controller.animator.GetFloat("MovementValue"), 0.5f, Time.deltaTime * animationLerpSpeed);
            controller.animator.SetFloat("MovementValue", lerpedVal);
        }
        else // if not moving do idle animation
        {
            float lerpedVal = Mathf.Lerp(controller.animator.GetFloat("MovementValue"), 0f, Time.deltaTime * animationLerpSpeed);
            controller.animator.SetFloat("MovementValue", lerpedVal);
        }
    }
    private void HandleRotation()
    {
        if (targetPosition - transform.position != Vector3.zero && canMove && !hasReached) //Handles rotation
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
    public void ReachTarget()
    {
        hasReached = true;
    }
}
