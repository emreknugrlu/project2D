using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiderHandler : MonoBehaviour
{
    [Header("Components")]
    public TilemapRenderer tilemapRenderer;
    // Start is called before the first frame update
    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered");
        other.CompareTag("Player");
        if (other.CompareTag("Player"))
            tilemapRenderer.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exitted");
        if (other.CompareTag("Player"))
            tilemapRenderer.enabled = true;
    }
}
