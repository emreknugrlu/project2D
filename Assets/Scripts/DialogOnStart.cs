using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<DialogueTrigger>().TriggerDialogue();
        Destroy(gameObject);
    }
}
