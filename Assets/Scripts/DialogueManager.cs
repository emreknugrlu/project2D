using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	public TextMeshProUGUI nameText;
	public TextMeshProUGUI dialogueText;

	public Animator boxAnimator;
	public Animator portraitAnimator;
    private AudioManager audioManager;
    public AudioMixer audioMixer;

    private GameObject player;

	private Queue<string> sentences;
	[SerializeField]private float letterSpeed=1f;

	// Use this for initialization
	void Start () {
		sentences = new Queue<string>();
		player = GameObject.FindWithTag("Player");
        audioManager = FindObjectOfType<AudioManager>();
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) ||
		    Input.GetKeyDown(KeyCode.Return) || 
		    Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			DisplayNextSentence();
		}
	}

	public void StartDialogue (Dialogue dialogue)
	{
		player.GetComponent<ControlPlayer>().StopPlayer();
		boxAnimator.SetBool("isOpen", true);
		portraitAnimator.SetBool("isOpen", true);

		nameText.SetText(dialogue.name);

		sentences.Clear();

		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence ()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence (string sentence)
	{

		dialogueText.SetText("");
        int letterCounter = 0; // Counter to track displayed letters
        const int lettersPerSound = 3; // Play sound every 3 letters
        foreach (char letter in sentence)
		{
			dialogueText.SetText(dialogueText.text + letter);
            letterCounter++;

            //Play the sound effect for each letter
            if (letterCounter % lettersPerSound == 0 && audioManager != null)
            {
                audioManager.PlaySFX("button", 0.2f);
            }

            yield return new WaitForSeconds(0.4f/(Mathf.Abs(letterSpeed) + Mathf.Epsilon));
        }
	}

	void EndDialogue()
	{
		player.GetComponent<ControlPlayer>().enabled = true;
		boxAnimator.SetBool("isOpen", false);
		portraitAnimator.SetBool("isOpen", false);
	}

}
