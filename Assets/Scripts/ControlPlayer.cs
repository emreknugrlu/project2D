using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class ControlPlayer : MonoBehaviour
{
    float horizontalAxis = 0;
    private bool jumpKey = false;
    public bool onGround = false;

    [Header("Movement Values")]
    public float moveSpeed = 5.5f;
    public float jumpSpeed = 8.25f;
    private float walkSoundCooldown = 0.3f;
    private float nextWalkSoundTime = 0f;
    private bool hasAttacked = false;  // Flag to track if attack sound has played
    private bool hasBlocked = false;   // Flag to track if block sound has played
    private bool hasJumped = false;

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
    private GameObject HP;
    public Rigidbody2D rb;
    public BoxCollider2D groundDetector;
    public Animator animator;
    private AudioManager audioManager;
    public AudioMixer audioMixer;

    private PlayerAttack playerAttack;
    private HealthAndPosture healthAndPosture;

    private void Start()
    {
        HP = transform.GetChild(3).gameObject;
        playerAttack = GetComponent<PlayerAttack>();
        healthAndPosture = GetComponent<HealthAndPosture>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Update()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        jumpKey = Input.GetKeyDown(KeyCode.W);

        if (horizontalAxis < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            HP.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (horizontalAxis > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            HP.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (onGround && jumpKey && !hasJumped) // Check if onGround before jumping
        {
            hasJumped = true;
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            audioManager.PlaySFX("jump");
        }
        else if (onGround)
        {
            hasJumped = false;
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
        // SFX for Specific States (with checks to play once per action)
        switch (playerStates)
        {
            case PlayerStates.Blocking:
                if (!hasBlocked)
                {
                    audioManager.PlaySFX("block", 0.1f);
                    hasBlocked = true;
                }
                break;
            case PlayerStates.Attack:
                if (!hasAttacked)
                {
                    audioManager.PlaySFX("attack", 0.1f);
                    hasAttacked = true;
                }
                break;
            case PlayerStates.Idle: // Reset flags when transitioning to Idle
            case PlayerStates.Run:
                hasAttacked = false;
                hasBlocked = false;
                break;
        }
        // Sound Effects Logic with Cooldown
        if (onGround && jumpKey)
        {
            audioManager.PlaySFX("jump");
        }
        // Walk Sound (only if enough time has passed)
        else if (onGround && Mathf.Abs(rb.velocity.x) > Mathf.Epsilon && Time.time >= nextWalkSoundTime)
        {
            audioManager.PlaySFX("walk");
            nextWalkSoundTime = Time.time + walkSoundCooldown;
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
