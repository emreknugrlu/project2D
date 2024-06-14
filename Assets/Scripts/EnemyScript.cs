using System.Collections;
using UnityEngine;
using static ControlPlayer;

public class EnemyScript : MonoBehaviour
{
    // Public Variables (Adjustable in the Inspector)
    public GameObject pointA; // Patrol point A
    public GameObject pointB; // Patrol point B
    public GameObject player; // Reference to the player
    public float speed = 2f; // Movement speed
    public float upperYAxisTolerance = 4.0f; // Vertical tolerance for player detection
    public float lowerYAxisTolerance = 0.5f; // Vertical tolerance for player detection
    public float windupDuration = 1f; // Duration of windup before each attack

    public enum EnemyStates
    {
        Idle,
        Windup,
        Walk,
        Run,
        Attack,
        Death,
    }
    public EnemyStates enemyStates = EnemyStates.Idle;
    [SerializeField] float runAnimSpeed = 1.5f;
    [SerializeField] float walkAnimSpeed = 1f;
    [SerializeField] float attackAnimSpeed = 1f;
    [SerializeField] float windupAnimSpeed = 1f;


    // Private Variables
    [SerializeField] private Animator animator;
    [SerializeField] private float timeToAttack = 0.65f;
    [SerializeField] private float attackInteval = 1f;
    private Rigidbody2D rb;
    private HealthAndPosture healthAndPosture;
    private Transform currentPoint;
    private GameObject attackArea;
    private bool playerInPatrolArea = false;
    private bool isChasingPlayer = false;
    public float attackDistance = 2.0f; // Distance at which the enemy attacks
    public bool attacking = false;
    private bool canAttack = true;
    private float attackTimer = 0f;
    private float waitingToAttack = 0f;
    private string currentState = IDLE;

    // Animation State Constants
    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string RUN = "Run";
    public const string Death = "Death";
    public const string ATTACK = "Attack";
    public const string WINDUP = "Windup";

    
    IEnumerator DoAttackAfterWindup()
    {
        yield return new WaitForSeconds(windupDuration);
        enemyStates = EnemyStates.Attack;
        ChangeAnimationState(ATTACK, attackAnimSpeed);
        Attack();
    }
    IEnumerator StartWindup()
    {
        enemyStates = EnemyStates.Windup;
        ChangeAnimationState(WINDUP, windupAnimSpeed);
        StartCoroutine(DoAttackAfterWindup());
        yield return null;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentPoint = pointB.transform;
        ChangeAnimationState(IDLE);
        attackArea = transform.GetChild(0).gameObject;
        attackArea.SetActive(false); // Ensure the attack area is initially inactive
        healthAndPosture = GetComponent<HealthAndPosture>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        playerInPatrolArea = IsPlayerInPatrolArea();
        bool shouldChase = playerInPatrolArea && distanceToPlayer <= 10f;
        bool shouldAttack = distanceToPlayer <= attackDistance; // New condition for attack

        if (healthAndPosture.die)
        {
            ChangeAnimationState(Death);
            Destroy(transform.GetChild(1).gameObject);
            Destroy(transform.GetChild(0).gameObject);
            Destroy(this);
            return;
        }
        else if (shouldAttack)
        {
            // Stop movement and attack
            rb.velocity = Vector2.zero;
            FlipTowards(player.transform.position);
            if (canAttack)
                StartCoroutine(StartWindup());
        }
        else if (shouldChase)
        {
            // Chase the player
            isChasingPlayer = true;
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
            enemyStates = EnemyStates.Run;
            ChangeAnimationState(RUN, runAnimSpeed);
            FlipTowards(player.transform.position);
        }
        else
        {
            // Patrol between points
            isChasingPlayer = false;
            rb.velocity = (currentPoint == pointB.transform) ? new Vector2(speed, 0f) : new Vector2(-speed, 0f);

            // Switch direction at patrol points
            if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
            {
                currentPoint = (currentPoint == pointA.transform) ? pointB.transform : pointA.transform;
                FlipTowards(currentPoint.position);
            }

            // Walk while patrolling
            enemyStates = EnemyStates.Walk;
            ChangeAnimationState(WALK, walkAnimSpeed);
        }

        // Handle the attacking state
        if (attacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= timeToAttack)
            {
                EndAttack();
            }
        }else
        {
            waitingToAttack += Time.deltaTime;
            
            if (waitingToAttack >= attackInteval)
            {
                canAttack = true;
            }
        }
    }

    // Function to flip the sprite horizontally based on target position
    private void FlipTowards(Vector3 target)
    {
        if ((target.x < transform.position.x && transform.localScale.x > 0) || (target.x > transform.position.x && transform.localScale.x < 0))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
    public void Attack()
    {
        if (!attacking)
        {
            attacking = true;
            canAttack = false;
            attackArea.SetActive(true); // Activate the attack area

            // Reset timers
            attackTimer = 0f;
        }
    }
    private void EndAttack()
    {
        attacking = false;
        attackArea.SetActive(false); // Deactivate the attack area
        enemyStates = EnemyStates.Idle;
        ChangeAnimationState(IDLE);

        // Reset attack attributes
        attackTimer = 0f;
        waitingToAttack = 0f;
        canAttack = false;
    }

    // Function to check if the player is within the patrol area
    private bool IsPlayerInPatrolArea()
    {
        float minX = Mathf.Min(pointA.transform.position.x, pointB.transform.position.x);
        float maxX = Mathf.Max(pointA.transform.position.x, pointB.transform.position.x);
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;
        float enemyY = transform.position.y;

        return playerX > minX && playerX < maxX &&
        playerY <= enemyY + upperYAxisTolerance && playerY >= enemyY - lowerYAxisTolerance;
    }

    // Function to draw patrol path gizmos (visible in Scene view)
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
            Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
        }
    }

    // Function to change animation state and prevent unnecessary calls
    public void ChangeAnimationState(string newState, float animSpeed = 1f)
    {
        if (currentState == newState)
            return;

        animator.Play(newState);
        animator.speed = animSpeed;
        currentState = newState;
    }
}
