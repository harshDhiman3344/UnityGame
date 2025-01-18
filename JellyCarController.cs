using UnityEngine;

public class JellyCarController : MonoBehaviour
{
    public float speed = 10f; // Speed of the car
    public float turnSpeed = 100f; // Turning speed
    public float driftFactor = 0.95f; // How much the car slides when drifting
    public float suspensionRestHeight = 1f; // Default height from the ground
    public float springForceMultiplier = 50f; // Strength of the spring
    public float bounceFactor = 0.3f; // Extra wacky bounce on impact
    public float damper = 5f; // Reduces oscillation over time
    public LayerMask groundLayer; // Ensure raycast only hits the ground
    public float customGravity = -9.81f; // Custom gravity for faster fall
    public float brakeForce = 2f; // Braking force applied downward

    private Rigidbody rb;
    private float currentHeight;
    private float velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable Unity's default gravity
        rb.velocity = Vector3.zero; // Ensure the car isn't moving initially
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleSuspension();
    }

    // Handle the movement with WASD keys
    private void HandleMovement()
    {
        // Getting input for movement
        float moveInput = Input.GetAxis("Vertical"); // Forward and backward movement (W/S or Up/Down Arrow)
        float turnInput = Input.GetAxis("Horizontal"); // Left and right movement (A/D or Left/Right Arrow)

        // Calculate movement force
        Vector3 moveForce = transform.forward * moveInput * speed;
        rb.AddForce(moveForce, ForceMode.Acceleration);

        // Calculate turning (steering)
        float turnAmount = turnInput * turnSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, turnAmount);

        // Apply drifting effect to reduce friction when sliding
        rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity * driftFactor, Time.deltaTime);

        // Check for braking input
        if (Input.GetKey(KeyCode.Space)) // Spacebar for braking
        {
            ApplyBraking();
        }
    }

    // Apply braking force
    private void ApplyBraking()
    {
        rb.AddForce(Vector3.down * brakeForce, ForceMode.Acceleration); // Apply downward force
    }

    // Handle suspension physics
    private void HandleSuspension()
    {
        // Apply custom gravity manually
        rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);

        // Cast a ray downwards to detect the ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, suspensionRestHeight * 2, groundLayer))
        {
            currentHeight = hit.distance; // Distance from the ground

            // Calculate spring force based on distance from rest height
            float springForce = (suspensionRestHeight - currentHeight) * springForceMultiplier;

            // Add damping to reduce bounciness over time
            float dampingForce = damper * velocity;

            // Final suspension force
            float suspensionForce = springForce - dampingForce;

            // Apply force upwards (simulate suspension)
            rb.AddForce(Vector3.up * suspensionForce, ForceMode.Acceleration);

            // Add some extra bounce for that *Dani* effect
            if (springForce > 20f) // Only bounce when hitting the ground hard
            {
                rb.AddForce(Vector3.up * bounceFactor, ForceMode.Impulse);
            }

            // Simulate "randomness" for wacky behavior
            rb.AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.Acceleration);
        }
    }
}