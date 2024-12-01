using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D body;
    
    [Header("Movement Stats")]
    [SerializeField, Range(0f, 20f)][Tooltip("Maximum movement speed")] public float maxSpeed = 10f;
    
    [Header("Calculations")]
    public float directionX;
    private Vector2 desiredVelocity;
    public Vector2 velocity;
  
    
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        directionX = Input.GetAxisRaw("Horizontal");
        
        // flipping the sprites direction
        if (directionX != 0)
        {
            // using a ternary operator to flip the local scale
            transform.localScale = new Vector3(directionX > 0 ? 1 : -1, 1, 1);
        }

        desiredVelocity = new Vector2(directionX, 0f) * maxSpeed;
    }

    private void FixedUpdate()
    {
        // grab a reference to the current velocity
        velocity = body.velocity;
        
        // change the reference velocity's x component
        velocity.x = desiredVelocity.x;
        
        // update the velocity
        body.velocity = velocity;


    }
}
