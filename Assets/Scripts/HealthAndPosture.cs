using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class HealthAndPosture : MonoBehaviour
{
    [SerializeField] private int health = 100;
    private int MAX_HEALTH = 100;

    public Image healthBar;
    public Image postureBar;

    [Header("Posture Settings")]
    [SerializeField] private int posture = 0;
    [SerializeField] private float parriedDuration = 0.2f;
    [SerializeField] private float damageDuration = 0.6f;
    [SerializeField] private float freezeDuration = 3.0f;// Duration to freeze the enemy
    private int MAX_POSTURE = 100;
    public bool isPostureBroken = false;
    public bool isStunned = false;
    public bool gotParried = false;
    public bool die = false;
    public bool takeDamage = false;

    private Rigidbody2D rb2D;
    private Animator animator; // Animator component
    private AudioManager audioManager;
    public AudioMixer audioMixer;

    [Header("State Settings")]
    public bool blocking = false;
    public bool parrying = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component is missing!");
        }
        audioManager = FindObjectOfType<AudioManager>(); // Get reference to AudioManager
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

        healthBar.fillAmount = health / 100f;
        postureBar.fillAmount = posture / 100f;
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
        takeDamage = true;
        StartCoroutine(DamageRecovery());

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
        takeDamage = true;
        StartCoroutine(DamageRecovery());
        TakePostureDamage(20);
    }

    public void TakePostureDamage(int amount)
    {
        if (isPostureBroken) return; // Prevent additional posture damage when broken

        takeDamage = true;
        StartCoroutine(DamageRecovery());

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
        isPostureBroken = false; // Reset posture broken state

        Debug.Log("Stun ended");
    }

    private IEnumerator DamageRecovery()
    {
        yield return new WaitForSeconds(damageDuration);

        takeDamage = false;
        Debug.Log("takeDamage ended");
    }

    private IEnumerator ParryRecovery()
    {
        yield return new WaitForSeconds(parriedDuration);

        gotParried = false;

        Debug.Log("isParried ended");
    }

    private void Die()
    {
        Debug.Log("I am Dead!");
        die = true;

        // Play the death sound effect
        if (audioManager != null)
        {
            audioManager.PlaySFX("death", 0.1f);
        }

        // Find the PlayerRespawn component and trigger respawn
        PlayerRespawn playerRespawn = GetComponent<PlayerRespawn>();
        if (playerRespawn != null)
        {
            playerRespawn.Respawn();
        }
        else
        {
            Debug.LogWarning("PlayerRespawn component not found!");
        }
    }
    public void ResetHealthAndPosture()
    {
        health = 100;
        posture = 0;

        die = false;
    }

    // Method to check if the player is blocking or parrying
    public bool IsBlockingOrParrying()
    {
        return blocking || parrying;
    }
}