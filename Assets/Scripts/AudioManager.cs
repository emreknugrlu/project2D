using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Sources ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    // Store audio clips directly in the script for better organization
    [Header("---------- Sound Effects ----------")]
    public AudioClip background;
    public AudioClip checkpoint;
    public AudioClip attack;
    public AudioClip parry;
    public AudioClip jump;
    public AudioClip walk;
    public AudioClip block;
    public AudioClip land;

    private void Start()
    {
        // Ensure both AudioSource and the AudioClip are assigned
        if (musicSource != null && background != null)
        {
            musicSource.clip = background;
            musicSource.Play();
            musicSource.loop = true;
        }
        else
        {
            Debug.LogWarning("Missing Music Source or Background Music Clip in AudioManager.");
        }
    }

    public void PlaySFX(string soundName)
    {
        AudioClip clipToPlay = null;

        // Switch statement to determine the AudioClip based on the soundName
        switch (soundName)
        {
            case "checkpoint":
                clipToPlay = checkpoint;
                break;
            case "attack":
                clipToPlay = attack;
                break;
            case "parry":
                clipToPlay = parry;
                break;
            case "jump":
                clipToPlay = jump;
                break;
            case "walk":
                clipToPlay = walk;
                break;
            case "block":
                clipToPlay = block;
                break;
            case "land":
                clipToPlay = land;
                break;
            default:
                Debug.LogWarning("Sound effect not found: " + soundName);
                break;
        }

        // Play the AudioClip if found
        if (clipToPlay != null && SFXSource != null)
        {
            SFXSource.PlayOneShot(clipToPlay);
        }
        else
        {
            Debug.LogWarning("Missing SFX Source or AudioClip in AudioManager.PlaySFX()");
        }
    }
}
