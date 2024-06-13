using UnityEngine;
using UnityEngine.Audio;

public class PlayerRespawn : MonoBehaviour
{
    private Transform latestCheckpoint;
    private HealthAndPosture healthAndPosture; // Reference to HealthAndPosture script
    private Animator animator;  // Reference to Animator component
    private AudioManager audioManager;
    public AudioMixer audioMixer;

    void Start()
    {
        // Optionally, set an initial checkpoint at the player's starting position
        latestCheckpoint = transform;
        healthAndPosture = GetComponent<HealthAndPosture>();  // Get references to scripts
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Method to set a new checkpoint
    public void SetCheckpoint(Transform checkpoint)
    {
        latestCheckpoint = checkpoint;
        Debug.Log("Checkpoint set at: " + checkpoint.position);
    }

    // Method to respawn the player at the latest checkpoint
    public void Respawn()
    {
        if (latestCheckpoint != null)
        {
            Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            transform.position = latestCheckpoint.position;
            Debug.Log("Player respawned at: " + latestCheckpoint.position);
            // Reset health and animation state
            if (healthAndPosture != null)
            {
                healthAndPosture.ResetHealthAndPosture();
            }
            if (animator != null)
            {
                animator.Play("Idle");
            }

            if (audioManager != null)
            {
                audioManager.PlaySFX("respawn", 0.1f);
            }
        }
        else
        {
            Debug.LogWarning("No checkpoint set. Player respawn failed.");
        }
    }
}
