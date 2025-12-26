using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player controller that only accepts movement inputs on beat.
/// Off-beat inputs trigger a stumble penalty.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveDuration = 0.2f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Stumble Settings")]
    [SerializeField] private float stumbleDuration = 0.5f;

    [Header("Audio Feedback")]
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failureSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    // Components
    private Rigidbody rb;
    private Animator animator;

    // State
    private bool isMoving = false;
    private bool isStumbling = false;
    private Vector2 moveInput;

    // Input Actions (using new Input System)
    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(); // Optional: will be null if no Animator exists

        // Set up Rigidbody for 3D physics
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Set up audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound
        }
    }

    private void OnEnable()
    {
        // Set up Input System if PlayerInput component exists
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
        }
    }

    private void Update()
    {
        // Don't accept input while stumbling or moving
        if (isStumbling || isMoving)
        {
            return;
        }

        // Get input
        Vector2 input = Vector2.zero;
        
        if (playerInput != null && moveAction != null)
        {
            input = moveAction.ReadValue<Vector2>();
        }
        else
        {
            // Fallback to legacy input if new Input System not set up
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        // Check for movement input
        if (input.magnitude > 0.1f)
        {
            // Normalize input for consistent movement speed
            input.Normalize();

            // Validate rhythm timing
            bool isOnBeat = RhythmValidator.IsInputOnBeat();

            if (isOnBeat)
            {
                // Success - perform movement
                OnSuccessfulInput(input);
            }
            else
            {
                // Failure - stumble
                OnFailedInput();
            }
        }
    }

    private void OnSuccessfulInput(Vector2 input)
    {
        if (showDebugInfo)
        {
            string grade = RhythmValidator.GetTimingGrade();
            Debug.Log($"PlayerController: Movement SUCCESS! Timing: {grade}");
        }

        // Play success sound
        PlaySound(successSound);

        // Convert 2D input to 3D movement direction
        Vector3 moveDirection = new Vector3(input.x, 0, input.y).normalized;

        // Start movement coroutine
        StartCoroutine(MoveRoutine(moveDirection));

        // Trigger animation if available
        if (animator != null)
        {
            animator.SetTrigger("Move");
        }
    }

    private void OnFailedInput()
    {
        if (showDebugInfo)
        {
            Debug.Log("PlayerController: Movement FAILED! Off-beat input - Stumbling!");
        }

        // Play failure sound
        PlaySound(failureSound);

        // Trigger stumble
        StartCoroutine(StumbleRoutine());

        // Trigger animation if available
        if (animator != null)
        {
            animator.SetTrigger("Stumble");
        }
    }

    private IEnumerator MoveRoutine(Vector3 direction)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (direction * moveDistance);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            float curveValue = moveCurve.Evaluate(t);

            // Use MovePosition for physics-based movement
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate();
        }

        // Ensure we reach the exact target position
        rb.MovePosition(targetPosition);
        isMoving = false;
    }

    private IEnumerator StumbleRoutine()
    {
        isStumbling = true;
        
        // Disable input for stumble duration
        yield return new WaitForSeconds(stumbleDuration);
        
        isStumbling = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnGUI()
    {
        if (showDebugInfo)
        {
            GUI.Label(new Rect(10, 100, 300, 20), $"Player State: {(isStumbling ? "STUMBLING" : (isMoving ? "MOVING" : "READY"))}");
            
            if (Conductor.Instance != null)
            {
                float distanceToBeat = Conductor.Instance.GetDistanceToNearestBeat();
                bool wouldBeOnBeat = RhythmValidator.IsInputOnBeat();
                Color originalColor = GUI.color;
                GUI.color = wouldBeOnBeat ? Color.green : Color.red;
                GUI.Label(new Rect(10, 120, 300, 20), $"Input Window: {(wouldBeOnBeat ? "OPEN" : "CLOSED")} ({distanceToBeat:F3}s)");
                GUI.color = originalColor;
            }
        }
    }
}
