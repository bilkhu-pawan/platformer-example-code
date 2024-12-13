// Adapted from Platformer ToolKit: https://gmtk.itch.io/platformer-toolkit

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterGround : MonoBehaviour
{
    private bool onGround;
    [Header("Collider Settings")]
    
    [SerializeField][Tooltip("Length of the ground-checking collider")] private float distance = 0.95f;

    [Header("Layer Masks")]
    
    [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.Raycast(transform.position, Vector2.down, distance, groundLayer);
    }
    private void OnDrawGizmos()
    {
        //Draw the ground colliders on screen for debug purposes
        if (onGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distance);
    }
    
    public bool GetOnGround()
    {
        return onGround;
    }
}
