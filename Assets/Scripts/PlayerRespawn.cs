using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Transform latestCheckpoint;

    public Vector3 respawnOffset = new Vector3(0f, 1f, 0f); // Adjust y value as needed

    void Start()
    {
        latestCheckpoint = transform; // Set initial checkpoint at the start
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        latestCheckpoint = checkpoint;
        Debug.Log("Checkpoint set at: " + checkpoint.position);
    }

    public void Respawn()
    {
        if (latestCheckpoint != null)
        {
            Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Reset the player's velocity
            }

            Vector3 adjustedRespawnPosition = latestCheckpoint.position + respawnOffset;

            // Optional Raycast Check:
            RaycastHit2D hit = Physics2D.Raycast(adjustedRespawnPosition, Vector2.down);
            if (hit.collider != null)
            {
                adjustedRespawnPosition.y = hit.point.y + respawnOffset.y; // Respawn just above the ground
            }

            transform.position = adjustedRespawnPosition;
            Debug.Log("Player respawned at: " + transform.position);
        }
        else
        {
            Debug.LogWarning("No checkpoint set. Player respawn failed.");
        }
    }
}
