using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject attackArea;
    private GameObject blockShield;

    public bool attacking = false;
    public bool blocking = false;
    public bool parrying = false;

    [SerializeField] private float timeToAttack = 0.65f;
    private float attackTimer = 0f;

    [SerializeField] private float parryWindow = 0.5f; // Parry window is 0.5 seconds
    private float parryTimer = 0f;

    private HealthAndPosture healthAndPosture;

    // Start is called before the first frame update
    void Start()
    {
        attackArea = transform.GetChild(0).gameObject;
        attackArea.SetActive(false); // Ensure the attack area is initially inactive

        blockShield = transform.GetChild(1).gameObject;
        blockShield.SetActive(false);

        // Get the HealthAndPosture component from the current game object
        healthAndPosture = GetComponent<HealthAndPosture>();

        if (healthAndPosture == null)
        {
            Debug.LogError("HealthAndPosture component is missing!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Attack action
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        // Block action
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartBlocking();
        }

        // Check if the player is still holding the block key
        if (blocking && Input.GetKey(KeyCode.LeftControl))
        {
            HandleBlocking();
        }
        else if (blocking && Input.GetKeyUp(KeyCode.LeftControl))
        {
            EndBlocking();
        }

        // Handle the attacking state
        if (attacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= timeToAttack)
            {
                EndAttack();
            }
        }
    }

    public void Attack()
    {
        if (!attacking && !blocking && !parrying)
        {
            attacking = true;
            attackArea.SetActive(true); // Activate the attack area

            // Reset timers
            attackTimer = 0f;
        }
    }

    private void EndAttack()
    {
        attacking = false;
        attackArea.SetActive(false); // Deactivate the attack area

        // Reset attack timer
        attackTimer = 0f;
    }

    private void StartBlocking()
    {
        if (!attacking)
        {
            blocking = true;
            blockShield.SetActive(true);
            parrying = true; // Parrying is initially true

            // Link blocking state with the HealthAndPosture component
            healthAndPosture.blocking = true;
            healthAndPosture.parrying = true;

            // Reset parry timer
            parryTimer = 0f;
        }
    }

    private void HandleBlocking()
    {
        if (parrying)
        {
            parryTimer += Time.deltaTime;

            // End the parry state after the parry window
            if (parryTimer >= parryWindow)
            {
                parrying = false;
                healthAndPosture.parrying = false;
            }
        }
    }

    private void EndBlocking()
    {
        blocking = false;
        blockShield.SetActive(false);
        parrying = false; // Ensure parry is false when blocking ends

        // Reset block and parry states in HealthAndPosture
        healthAndPosture.blocking = false;
        healthAndPosture.parrying = false;

        // Reset parry timer
        parryTimer = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (parrying && collision.CompareTag("Enemy"))
        {
            // Assuming the enemy has a HealthAndPosture component
            HealthAndPosture enemyHealthAndPosture = collision.GetComponent<HealthAndPosture>();
            if (enemyHealthAndPosture != null)
            {
                enemyHealthAndPosture.GetParried();
            }
        }
    }
}
