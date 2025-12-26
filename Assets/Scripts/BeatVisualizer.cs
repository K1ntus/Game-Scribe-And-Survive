using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual debug component that pulses on each beat to help visualize rhythm timing.
/// Attach to a UI Image (circle recommended) for visual feedback.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class BeatVisualizer : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float pulseScale = 1.5f;
    [SerializeField] private float pulseDuration = 0.2f;
    [SerializeField] private AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Color Settings")]
    [SerializeField] private bool changeColorOnBeat = true;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color beatColor = Color.cyan;

    private RectTransform rectTransform;
    private Image image;
    private Coroutine pulseCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        if (image != null)
        {
            image.color = normalColor;
        }
    }

    private void OnEnable()
    {
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat += OnBeat;
        }
    }

    private void OnDisable()
    {
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat -= OnBeat;
        }
    }

    private void Start()
    {
        // Subscribe to conductor events if not already subscribed
        if (Conductor.Instance != null && Conductor.Instance.OnBeat != null)
        {
            // Unsubscribe first to avoid double subscription
            Conductor.Instance.OnBeat -= OnBeat;
            Conductor.Instance.OnBeat += OnBeat;
        }
    }

    private void OnBeat(int beatCount)
    {
        // Stop any existing pulse
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }

        // Start new pulse
        pulseCoroutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float elapsed = 0f;

        // Change color if enabled
        if (changeColorOnBeat && image != null)
        {
            image.color = beatColor;
        }

        // Animate scale
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;
            float curveValue = pulseCurve.Evaluate(t);

            // Pulse out then back in
            float scale = Mathf.Lerp(pulseScale, normalScale, curveValue);
            rectTransform.localScale = Vector3.one * scale;

            yield return null;
        }

        // Ensure we end at normal scale
        rectTransform.localScale = Vector3.one * normalScale;

        // Return to normal color
        if (changeColorOnBeat && image != null)
        {
            image.color = normalColor;
        }

        pulseCoroutine = null;
    }
}
