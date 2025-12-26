using UnityEngine;

/// <summary>
/// Makes a sprite quad always face the camera for 2.5D effect.
/// Attach to a child object with a Quad mesh renderer.
/// </summary>
public class BillboardSprite : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool freezeXRotation = true;
    [SerializeField] private bool freezeZRotation = true;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("BillboardSprite: No main camera found!");
        }
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
        {
            return;
        }

        // Make the sprite face the camera
        if (freezeXRotation && freezeZRotation)
        {
            // Only rotate on Y axis (typical for 2.5D games)
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            directionToCamera.y = 0; // Keep sprite upright
            if (directionToCamera != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(-directionToCamera);
            }
        }
        else
        {
            // Full billboard effect
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
