using UnityEngine;

/// <summary>
/// Static helper class for validating rhythm-based inputs.
/// Checks if player input occurs within the acceptable timing window of a beat.
/// </summary>
public static class RhythmValidator
{
    // Default tolerance window in seconds (±0.15s as per spec)
    private const float DEFAULT_TOLERANCE = 0.15f;

    /// <summary>
    /// Checks if the current input timing is within the acceptable window of a beat.
    /// </summary>
    /// <param name="toleranceSeconds">The acceptable timing window in seconds (default ±0.15s)</param>
    /// <returns>True if input is on beat, false otherwise</returns>
    public static bool IsInputOnBeat(float toleranceSeconds = DEFAULT_TOLERANCE)
    {
        if (Conductor.Instance == null)
        {
            Debug.LogWarning("RhythmValidator: Conductor instance not found!");
            return false;
        }

        // Get the distance to the nearest beat
        float distanceToNearestBeat = Mathf.Abs(Conductor.Instance.GetDistanceToNearestBeat());

        // Check if within tolerance window
        bool isOnBeat = distanceToNearestBeat <= toleranceSeconds;

        return isOnBeat;
    }

    /// <summary>
    /// Gets the timing accuracy of the current input relative to the nearest beat.
    /// Returns a value between 0 (perfect) and 1 (at edge of tolerance).
    /// Values > 1 indicate the input is outside the timing window.
    /// </summary>
    public static float GetTimingAccuracy(float toleranceSeconds = DEFAULT_TOLERANCE)
    {
        if (Conductor.Instance == null)
        {
            return 1f;
        }

        float distanceToNearestBeat = Mathf.Abs(Conductor.Instance.GetDistanceToNearestBeat());
        return distanceToNearestBeat / toleranceSeconds;
    }

    /// <summary>
    /// Gets a timing grade (Perfect, Good, Ok, Miss) based on timing accuracy.
    /// </summary>
    public static string GetTimingGrade(float toleranceSeconds = DEFAULT_TOLERANCE)
    {
        float accuracy = GetTimingAccuracy(toleranceSeconds);

        if (accuracy <= 0.3f)
            return "Perfect!";
        else if (accuracy <= 0.6f)
            return "Good";
        else if (accuracy <= 1.0f)
            return "Ok";
        else
            return "Miss";
    }
}
