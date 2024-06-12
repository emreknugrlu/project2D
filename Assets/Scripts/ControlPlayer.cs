using System;
using System.Collections;
using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    private float horizontalAxis = 0;
    private bool jumpKey = false;
    public bool onGround = false;

    [Header("Movement Values")]
    public float moveSpeed = 5.5f;
    public float jumpSpeed = 8.25f;

    [Header("Animation Values")]
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
    private bool wasOnGround = false;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        healthAndPosture = GetComponent<HealthAndPosture>();
    }

    void Update()
    {
        // Get player inputs
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        jumpKey = Input.GetKeyDown(KeyCode.W);

        // Flip the player based on direction
        if (horizontalAxis < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (horizontalAxis > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        // Handle jump
        if (onGround && jumpKey)
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            audioManager.PlaySFX("jump");
        }
    }

    private void FixedUpdate()
    {
        // Update velocity for horizontal movement
        rb.velocity = new Vector2(horizontalAxis * moveSpeed, rb.velocity.y);
        onGround = groundDetector.IsTouchingLayers(LayerMask.GetMask("Ground"));

        // Sound effects
        HandleSoundEffects();

        // Check if player is grounded
        wasOnGround = onGround;

        // Update player states
        UpdatePlayerState();
    }

    private void HandleSoundEffects()
    {
        if (!wasOnGround && onGround)
        {
            audioManager.PlaySFX("land");
        }
        else if (onGround && Mathf.Abs(rb.velocity.x) > Mathf.Epsilon)
        {
            audioManager.PlaySFX("walk");
        }
        else if (!onGround && rb.velocity.y < 0)
        {
            audioManager.PlaySFX("fall");
        }
    }

    private void UpdatePlayerState()
    {
        if (HealthAndPosture.isPostureBroken)
        {
            playerStates = PlayerStates.BrokenPosture;
            ChangeAnimationState("BrokenPosture", brokenPostureAnimSpeed);
        }
        else if (HealthAndPosture.gotParried)
        {
            playerStates = PlayerStates.GotParried;
            ChangeAnimationState("GotParried", gotParriedAnimSpeed);
        }
        else if (HealthAndPosture.die)
        {
            playerStates = PlayerStates.Die;
            ChangeAnimationState("Die", dieAnimSpeed);
        }
        else if (HealthAndPosture.isStunned)
        {
            playerStates = PlayerStates.Stunned;
            ChangeAnimationState("Stunned", stunnedAnimSpeed);
        }
        else if (HealthAndPosture.takeDamage)
        {
            playerStates = PlayerStates.TakeDamage;
            ChangeAnimationState("TakeDamage", takeDamageAnimSpeed);
        }
        else if (playerAttack.blocking)
        {
            playerStates = PlayerStates.Blocking;
            ChangeAnimationState("Blocking", blockingAnimSpeed);
            audioManager.PlaySFX("block");
        }
        else if (playerAttack.attacking)
        {
            playerStates = PlayerStates.Attack;
            ChangeAnimationState("Attack", attackAnimSpeed);
            audioManager.PlaySFX("attack");
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
