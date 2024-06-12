using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Transform latestCheckpoint;

    void Start()
    {
        // Optionally, set an initial checkpoint at the player's starting position
        latestCheckpoint = transform;
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
        }
        else
        {
            Debug.LogWarning("No checkpoint set. Player respawn failed.");
        }
    }
}
