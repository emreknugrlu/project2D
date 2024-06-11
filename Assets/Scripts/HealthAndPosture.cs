using System.Collections;
using UnityEngine;

public class HealthAndPosture : MonoBehaviour
{
    [SerializeField] private int health = 100;
    private int MAX_HEALTH = 100;

    [Header("Posture Settings")]
    [SerializeField] private int posture = 0;
    [SerializeField] private float freezeDuration = 3.0f; // Duration to freeze the enemy
    private int MAX_POSTURE = 100;
    public static bool isPostureBroken = false;
    public static bool isStunned = false;
    public static bool gotParried = false;

    private Rigidbody2D rb2D; // Assuming 2D, adjust if using 3D

    [Header("State Settings")]
    public bool blocking = false;
    public bool parrying = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Damage(10);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakePostureDamage(20);
        }
    }

    public void Damage(int amount)
    {
        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative Damage");
        }

        if (blocking || parrying)
        {
            Debug.Log("Damage blocked or parried!");
            return;
        }

        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative healing");
        }

        if (health + amount > MAX_HEALTH)
        {
            health = MAX_HEALTH;
        }
        else
        {
            health += amount;
        }
    }

    public void GetParried()
    {
        gotParried = true;
        TakePostureDamage(20);
    }

    public void TakePostureDamage(int amount)
    {
        if (isPostureBroken) return; // Prevent additional posture damage when broken

        posture += amount;

        if (posture >= MAX_POSTURE)
        {
            posture = MAX_POSTURE; // Cap posture at max
            BreakPosture();
        }
        else
        {
            if (!isStunned)
            {
                StunObject();
            }
        }
    }

    private void BreakPosture()
    {
        isPostureBroken = true;
        Debug.Log("I am Paralyzed!");

        // Freeze the object
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void StunObject()
    {
        isStunned = true;
        Debug.Log("I got stunned!");

        // Freeze the object
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        StartCoroutine(StunRecovery());
    }

    private IEnumerator StunRecovery()
    {
        yield return new WaitForSeconds(freezeDuration);

        // Unfreeze the object
        if (rb2D != null)
        {
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        isStunned = false;

        Debug.Log("Stun ended");
    }

    private void Die()
    {
        Debug.Log("I am Dead!");
        Destroy(gameObject);
    }

    // Method to check if the player is blocking or parrying
    public bool IsBlockingOrParrying()
    {
        return blocking || parrying;
    }
}
