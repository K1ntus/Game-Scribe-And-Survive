using System;
using UnityEngine;

/// <summary>
/// Singleton Conductor class that acts as the global rhythm clock.
/// Tracks song position using AudioSettings.dspTime for precision timing.
/// </summary>
public class Conductor : MonoBehaviour
{
    public static Conductor Instance { get; private set; }

    [Header("Rhythm Settings")]
    [SerializeField] private float bpm = 120f;
    [SerializeField] private AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    // Timing variables
    private float secPerBeat;
    private double dspSongTime;
    private float songPosition;
    private int beatCount;
    private float lastBeatTime;

    // Events
    public event Action<int> OnBeat;

    // Public properties
    public float BPM => bpm;
    public float SecPerBeat => secPerBeat;
    public float SongPosition => songPosition;
    public int BeatCount => beatCount;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Calculate seconds per beat
        secPerBeat = 60f / bpm;
    }

    private void Start()
    {
        // Record the time when the audio starts
        if (audioSource != null && audioSource.clip != null)
        {
            dspSongTime = AudioSettings.dspTime;
            audioSource.Play();
        }
        else
        {
            // If no audio source, still track time for rhythm validation
            dspSongTime = AudioSettings.dspTime;
            Debug.LogWarning("Conductor: No AudioSource assigned. Running in silent mode.");
        }

        lastBeatTime = 0f;
        beatCount = 0;
    }

    private void Update()
    {
        // Track song position based on DSP time for precision
        if (audioSource != null && audioSource.isPlaying)
        {
            songPosition = (float)(AudioSettings.dspTime - dspSongTime);
        }
        else
        {
            // Even without audio, maintain timing
            songPosition = (float)(AudioSettings.dspTime - dspSongTime);
        }

        // Check if we've crossed a beat boundary
        float currentBeat = songPosition / secPerBeat;
        int currentBeatInt = Mathf.FloorToInt(currentBeat);

        if (currentBeatInt > beatCount)
        {
            beatCount = currentBeatInt;
            lastBeatTime = songPosition;
            OnBeat?.Invoke(beatCount);

            if (showDebugInfo)
            {
                Debug.Log($"Beat {beatCount} at {songPosition:F3}s");
            }
        }
    }

    /// <summary>
    /// Gets the time until the next beat in seconds.
    /// </summary>
    public float GetTimeToNextBeat()
    {
        float nextBeatTime = (beatCount + 1) * secPerBeat;
        return nextBeatTime - songPosition;
    }

    /// <summary>
    /// Gets the time since the last beat in seconds.
    /// </summary>
    public float GetTimeSinceLastBeat()
    {
        return songPosition - lastBeatTime;
    }

    /// <summary>
    /// Gets the distance to the nearest beat (positive = ahead, negative = behind).
    /// </summary>
    public float GetDistanceToNearestBeat()
    {
        float timeSinceLastBeat = GetTimeSinceLastBeat();
        float timeToNextBeat = GetTimeToNextBeat();

        // Return the smaller absolute distance, with appropriate sign
        if (timeSinceLastBeat < timeToNextBeat)
        {
            return -timeSinceLastBeat; // Negative means past the beat
        }
        else
        {
            return timeToNextBeat; // Positive means before the beat
        }
    }

    private void OnGUI()
    {
        if (showDebugInfo)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"BPM: {bpm}");
            GUI.Label(new Rect(10, 30, 300, 20), $"Song Position: {songPosition:F3}s");
            GUI.Label(new Rect(10, 50, 300, 20), $"Beat Count: {beatCount}");
            GUI.Label(new Rect(10, 70, 300, 20), $"Time to Next Beat: {GetTimeToNextBeat():F3}s");
        }
    }
}
