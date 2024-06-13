using System;
using UnityEngine;
using UnityEngine.Audio;

public class Checkpoint : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;
    private bool isChecked = false;

    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isChecked)
        {
            isChecked = true;
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            PlayerRespawn playerRespawn = other.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                playerRespawn.SetCheckpoint(transform.parent.transform);
            }
            if (dialogueTrigger != null)
            {
                dialogueTrigger.TriggerDialogue();
            }
        }
    }
}
