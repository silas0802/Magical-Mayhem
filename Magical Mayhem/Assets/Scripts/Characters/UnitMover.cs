using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnitMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5;
    private Vector3 targetPosition;
    private Rigidbody rb;

    void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    

    public void Move(){
        Debug.Log((targetPosition-transform.position).normalized*moveSpeed);
        rb.velocity = (targetPosition-transform.position).normalized*moveSpeed;
    }
    public void SetTargetPosition(Vector3 targetPosition){
        this.targetPosition = targetPosition;
    }
}
