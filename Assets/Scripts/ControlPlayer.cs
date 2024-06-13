using System;
using System.Collections;
using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    float horizontalAxis = 0;
    private bool jumpKey = false;
    public bool onGround = false;

    [Header("Movement Values")]
    public float moveSpeed = 5.5f;
    public float jumpSpeed = 8.25f;

    [Header("Animation values")]
    public PlayerStates playerStates = PlayerStates.Idle;
    [SerializeField] private float idleAnimSpeed = 1f;
    [SerializeField] private float runAnimSpeed = 1f;
    [SerializeField] private float jumpAnimSpeed = 1f;
    [SerializeField] private float fallAnimSpeed = 1f;
    [SerializeField] private float attackAnimSpeed = 1f;
    [SerializeField] private float brokenPostureAnimSpeed = 1f;
    [SerializeField] private float stunnedAnimSpeed = 1f;
    [SerializeField] private float blockingAnimSpeed = 1f;
    [SerializeField] private float gotParriedAnimSpeed = 1f;
    [SerializeField] private float dieAnimSpeed = 1f;
    [SerializeField] private float takeDamageAnimSpeed = 1f;

    [Header("Components")]
    public Rigidbody2D rb;
    public BoxCollider2D groundDetector;
    public Animator animator;

    private PlayerAttack playerAttack;
    private HealthAndPosture healthAndPosture;

    private void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        healthAndPosture = GetComponent<HealthAndPosture>();
    }

    void Update()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        jumpKey = Input.GetKeyDown(KeyCode.W);

        if (horizontalAxis < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (horizontalAxis > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        if (onGround && jumpKey)
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalAxis * moveSpeed, rb.velocity.y);
        onGround = groundDetector.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (healthAndPosture.isPostureBroken)
        {
            playerStates = PlayerStates.BrokenPosture;
            ChangeAnimationState("BrokenPosture", brokenPostureAnimSpeed);
        }
        else if (healthAndPosture.gotParried)
        {
            playerStates = PlayerStates.GotParried;
            ChangeAnimationState("GotParried", gotParriedAnimSpeed);
        }

        else if (healthAndPosture.die)
        {
            playerStates = PlayerStates.Die;
            ChangeAnimationState("Die", dieAnimSpeed);
        }

        else if (healthAndPosture.isStunned)
        {
            playerStates = PlayerStates.Stunned;
            ChangeAnimationState("Stunned", stunnedAnimSpeed);
        }

        else if (healthAndPosture.takeDamage)
        {
            playerStates = PlayerStates.TakeDamage;
            ChangeAnimationState("TakeDamage", takeDamageAnimSpeed);
        }

        else if (playerAttack.blocking)
        {
            playerStates = PlayerStates.Blocking;
            ChangeAnimationState("Blocking", blockingAnimSpeed);
        }
        else if (playerAttack.attacking)
        {
            playerStates = PlayerStates.Attack;
            ChangeAnimationState("Attack", attackAnimSpeed);
        }
        else if (onGround)
        {
            if (Mathf.Abs(rb.velocity.x) < Mathf.Epsilon)
            {
                playerStates = PlayerStates.Idle;
                ChangeAnimationState("Idle", idleAnimSpeed);
            }
            else
            {
                playerStates = PlayerStates.Run;
                ChangeAnimationState("Run", runAnimSpeed);
            }
        }
        else
        {
            playerStates = PlayerStates.Jump;
            if (rb.velocity.y > 0)
            {
                ChangeAnimationState("Jump", jumpAnimSpeed);
            }
            else
            {
                ChangeAnimationState("Fall", fallAnimSpeed);
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
    
    public void StopPlayer()
    {
        rb.velocity = Vector2.zero;
        horizontalAxis = 0;
        playerStates = PlayerStates.Idle;
        ChangeAnimationState("Idle", idleAnimSpeed);
        this.enabled = false;
    }

    public enum PlayerStates
    {
        Idle,
        Run,
        Jump,
        Attack,
        BrokenPosture,
        GotParried,
        Stunned,
        Blocking,
        Die,
        TakeDamage,
    }
}
