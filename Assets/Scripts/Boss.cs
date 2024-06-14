using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Boss Settings")]
    public float attackInterval = 2.0f; // Time between attacks
    public float attackDuration = 1.0f; // Duration of each attack
    public float windupDuration = 0.5f; // Duration of windup before each attack
    public GameObject attackArea; // Area of effect for boss attacks
    public float moveSpeed = 2.0f; // Speed at which the boss moves
    public float detectionRange = 10.0f; // Range to detect the player
    public float attackRange = 1.5f; // Range to start attacking the player

    [Header("Animation Speeds")]
    [SerializeField] private float idleAnimSpeed = 1f;
    [SerializeField] private float runAnimSpeed = 1f;
    [SerializeField] private float attackAnimSpeed = 1f;
    [SerializeField] private float windupAnimSpeed = 1f;
    [SerializeField] private float dieAnimSpeed = 1f;
    [SerializeField] private float takeDamageAnimSpeed = 1f;

    private Transform player;
    private bool isAttacking = false;
    private Animator animator;

    private BossStates bossState = BossStates.Idle;
    private string currentState;
    private HealthAndPosture healthAndPosture;
    [SerializeField] private GameObject swordClash;

    void Start()
    {
        healthAndPosture = GetComponent<HealthAndPosture>();
        attackArea = transform.GetChild(0).gameObject;
        attackArea.SetActive(false); // Ensure the attack area is initially inactive

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the Boss!");
        }

        // Assuming player is tagged as "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Start the attack pattern coroutine
        StartCoroutine(BossBehavior());
    }

    void Update()
    {
        if (healthAndPosture.die)
        {
            ChangeAnimationState("WindUp", windupAnimSpeed);
            swordClash.SetActive(true);
            Destroy(this);
            return;
        }
        if (isAttacking) return;

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                StartRandomAttack();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                ChasePlayer();
            }
            else
            {
                Idle();
            }
        }
    }

    private void FixedUpdate()
    {
        if (healthAndPosture.takeDamage)
        {
            ChangeAnimationState("TakeDamage", takeDamageAnimSpeed);
        }
    }

    IEnumerator BossBehavior()
    {
        while (!healthAndPosture.die) // Infinite loop for continuous behavior
        {
            if (!isAttacking)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer <= attackRange)
                {
                    yield return StartCoroutine(StartWindup()); // Start windup animation
                    yield return new WaitForSeconds(windupDuration);
                    StartRandomAttack();
                    yield return new WaitForSeconds(attackDuration);
                    EndAttack();
                }
            }
            yield return new WaitForSeconds(attackInterval);
        }
    }

    IEnumerator StartWindup()
    {
        ChangeAnimationState("WindUp", windupAnimSpeed);
        yield return null; // Ensure the windup animation starts before returning
    }

    private void StartRandomAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        FacePlayer();

        // Start windup animation
        StartCoroutine(StartWindup());

        // Choose a random attack after windup
        StartCoroutine(StartRandomAttackAfterWindup());
    }

    private IEnumerator StartRandomAttackAfterWindup()
    {
        // Wait for the windup duration before choosing the attack
        yield return new WaitForSeconds(windupDuration);

        // Choose a random attack
        int randomAttack = Random.Range(1, 3); // Generates a random number between 1 and 3
        string attackAnimation = "Attack" + randomAttack;

        ChangeAnimationState(attackAnimation, attackAnimSpeed);

        if (attackArea != null)
        {
            attackArea.SetActive(true);
        }

        Debug.Log("Boss starts attacking with: " + attackAnimation);

        // Schedule the end of the attack after the duration
        Invoke("EndAttack", attackDuration);
    }


    private void EndAttack()
    {
        if (!isAttacking) return;

        isAttacking = false;

        if (attackArea != null)
        {
            attackArea.SetActive(false);
        }

        ChangeAnimationState("Idle", idleAnimSpeed);
        Debug.Log("Boss stops attacking!");
    }

    private void ChasePlayer()
    {
        if (isAttacking) return;

        ChangeAnimationState("Run", runAnimSpeed);

        FacePlayer();

        // Get current position
        Vector3 currentPosition = transform.position;

        // Determine direction towards player on the X-axis
        float directionX = player.position.x - currentPosition.x;
        float movementX = moveSpeed * Time.deltaTime;

        // Move only in the X direction
        if (Mathf.Abs(directionX) > movementX) // Move only if the distance is greater than movement step
        {
            currentPosition.x += Mathf.Sign(directionX) * movementX;
        }
        else
        {
            currentPosition.x = player.position.x; // Snap to player's X position if close enough
        }

        // Update the position of the object
        transform.position = currentPosition;
    }


    private void Idle()
    {
        ChangeAnimationState("Idle", idleAnimSpeed);
    }

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Face right
        }
        else if (player.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Face left
        }
    }

    private void ChangeAnimationState(string newState, float animSpeed)
    {
        if (currentState == newState) return;

        animator.Play(newState);
        animator.speed = animSpeed;
        currentState = newState;
    }

    private enum BossStates
    {
        Idle,
        Run,
        Attack,
        Die,
        WindUp,
        TakeDamage,
    }
}
