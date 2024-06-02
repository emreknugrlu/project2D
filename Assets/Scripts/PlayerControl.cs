using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    [Header("Animation values")]
    public PlayerStates playerStates = PlayerStates.Idle;
    [SerializeField] float idleAnimSpeed = .12f;
    [SerializeField] float runAnimSpeed = .12f;
    [SerializeField] float jumpAnimSpeed = .12f;
    [SerializeField] float fallAnimSpeed = .12f;

    [Space(4)]
    [Header("Components")]
    public Rigidbody2D rb;
    public BoxCollider2D groundDetector;
    public Animator animator;
    

    // Update is called once per frame
    void Update()
    {
        
        
        
        horizontalAxis = Input.GetAxis("Horizontal");

        jumpKey = Input.GetKeyDown(KeyCode.W);

        if (horizontalAxis < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Sola gidiyorsa 0 derece
        }
        else if (horizontalAxis > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Sağa gidiyorsa 180 derece
        }

        if (onGround && jumpKey)
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string RUN = "Run";
    const string JUMP = "Jump";
    const string FALL = "Fall";
    
    
    private void FixedUpdate()
    {
        rb.velocity = new(horizontalAxis * moveSpeed, rb.velocity.y);
        onGround = groundDetector.IsTouchingLayers(LayerMask.GetMask("Ground"));
        
        if (onGround)
        {
            if (Mathf.Abs(rb.velocity.x) < Mathf.Epsilon)
            {
                playerStates = PlayerStates.Idle;
                ChangeAnimationState(IDLE, idleAnimSpeed);
            }
            else
            {
                playerStates = PlayerStates.Run;
                ChangeAnimationState(RUN, runAnimSpeed);

            }
        }
        else
        {
            playerStates = PlayerStates.Jump;
            if (rb.velocity.y > 0)
            {
                ChangeAnimationState(JUMP, jumpAnimSpeed);
            }
            else
            {
                ChangeAnimationState(FALL, fallAnimSpeed);
            }
        }
    }
    
    private string currentState;
    public void ChangeAnimationState(string newState, float animSpeed)
    {
        if (currentState == newState) return;

        animator.Play(newState);
        animator.speed = animSpeed;
        currentState = newState;
    }
    public enum PlayerStates
    {
        Idle,
        Run,
        Jump,
        Dead,
    }
}
