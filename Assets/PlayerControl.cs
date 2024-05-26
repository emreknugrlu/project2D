using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    
    float horizontalAxis = 0;
    private bool jumpKey = false;
    public bool onGround = false;
    
    
    [Header("Movement Values")]
    public float moveSpeed = 1f;
    public float jumpSpeed = 3.2f;

    [Space(4)]
    [Header("Components")]
    public Rigidbody2D rb;
    public BoxCollider2D groundDetector;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalAxis = Input.GetAxis("Horizontal");

        jumpKey = Input.GetKeyDown(KeyCode.W);

        if (onGround && jumpKey)
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new(horizontalAxis * moveSpeed, rb.velocity.y);
        onGround = groundDetector.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }
}
