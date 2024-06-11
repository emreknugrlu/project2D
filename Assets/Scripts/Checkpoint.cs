using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn playerRespawn = other.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                playerRespawn.SetCheckpoint(transform.parent.transform);
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
