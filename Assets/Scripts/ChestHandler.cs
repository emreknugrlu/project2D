using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestHandler : MonoBehaviour
{
    
    
    private SpriteRenderer spriteRenderer; // Add this line
    public float disappearDelay = 1f; // Delay before the chest starts to disappear
    public float disappearDuration = 1f; // Duration over which the chest disappears

    private void Start()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>(); // Add this line
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Chest chest = GetComponent<Chest>();
            if (chest != null)
            {
                chest.Open();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Chest chest = GetComponent<Chest>();
            if (chest != null)
            {
                chest.Close();
                StartCoroutine(DisappearOverTime());
            }
        }
    }

    private IEnumerator DisappearOverTime()
    {
        yield return new WaitForSeconds(disappearDelay);

        float startTime = Time.time;
        while (Time.time < startTime + disappearDuration)
        {
            float t = (Time.time - startTime) / disappearDuration;
            spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
