using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody RB;
    Vector2 inputDir;
    public float moveSpeed = 5f;
    public float moveForce = 100f;
    public bool usingForce = true;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (usingForce)
        {
            RB.AddForce(new Vector3(inputDir.x, 0, inputDir.y)*Time.deltaTime*moveForce);
        }
        else
        {
            RB.velocity = new Vector3(inputDir.x, RB.velocity.y, inputDir.y)*moveSpeed;
        }
    }
    public void OnMove(InputValue dir)
    {
        inputDir = dir.Get<Vector2>();
    }
}
