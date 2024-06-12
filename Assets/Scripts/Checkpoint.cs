using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private AudioManager audioManager; // Reference to your AudioManager

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        // Check if AudioManager was found
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

            PlayerRespawn playerRespawn = other.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                playerRespawn.SetCheckpoint(transform.parent.transform);
            }

            // Play checkpoint sound effect
            if (audioManager != null)
            {
                audioManager.PlaySFX("checkpoint"); // Pass the string "checkpoint"
            }
        }
    }
}