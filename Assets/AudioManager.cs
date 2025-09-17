using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager Instance { get; private set; }

    [Header("Music Settings")]
    public AudioClip[] musicClips;           // Array of music tracks
    public float fadeDuration = 1f;         // Duration for fade transitions

    private AudioSource audioSource;         // Primary AudioSource
    private int currentTrackIndex = -1;      // Currently playing track index

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Add and configure AudioSource component
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Plays the music track at the specified index with a fade transition.
    /// </summary>
    /// <param name="index">Index of the music track in the array.</param>
    public void PlayMusic(int index)
    {
        if (index < 0 || index >= musicClips.Length)
        {
            Debug.LogError("AudioManager: PlayMusic index out of range.");
            return;
        }

        if (currentTrackIndex == index)
        {
            Debug.Log("AudioManager: Requested track is already playing.");
            return;
        }

        StartCoroutine(FadeToTrack(index));
    }

    /// <summary>
    /// Plays a random music track from the array with a fade transition.
    /// </summary>
    public void PlayRandomMusic()
    {
        if (musicClips.Length == 0)
        {
            Debug.LogError("AudioManager: No music clips assigned.");
            return;
        }

        int randomIndex = Random.Range(0, musicClips.Length);

        // Ensure a different track is selected
        if (musicClips.Length > 1)
        {
            while (randomIndex == currentTrackIndex)
            {
                randomIndex = Random.Range(0, musicClips.Length);
            }
        }

        PlayMusic(randomIndex);
    }

    /// <summary>
    /// Stops the current music with a fade-out effect.
    /// </summary>
    public void StopMusic()
    {
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Coroutine to fade to a new track.
    /// </summary>
    /// <param name="newIndex">Index of the new track.</param>
    private IEnumerator FadeToTrack(int newIndex)
    {
        // Fade out current music
        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut());
        }

        // Assign and play new track
        audioSource.clip = musicClips[newIndex];
        audioSource.Play();
        currentTrackIndex = newIndex;

        // Fade in new music
        yield return StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Coroutine to fade in the music.
    /// </summary>
    private IEnumerator FadeIn()
    {
        audioSource.volume = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = 1f;
    }

    /// <summary>
    /// Coroutine to fade out the music.
    /// </summary>
    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    /// <summary>
    /// Optionally, call this to initialize the first track.
    /// </summary>
    private void Start()
    {
        if (musicClips.Length > 0)
        {
            //PlayMusic(0); // Start with the first track
        }
    }
}
