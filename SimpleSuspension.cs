using UnityEngine;

public class SimpleSuspension : MonoBehaviour
{
    public float restLength = 1f; // Default spring length at rest
    public float springStiffness = 100f; // Hooke's constant (k)
    public float damperStiffness = 10f; // Damping factor (c)
    public Transform floor; // Reference to the floor object (optional)

    private Rigidbody rb;
    private float springLength;
    private float springVelocity;
    private float springForce;
    private float damperForce;
    private Vector3 gravityForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure Rigidbody uses realistic gravity
        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;

        // Check if the cube is close enough to the floor
        if (Physics.Raycast(origin, direction, out hit, restLength * 2))
        {
            // Calculate the current spring length
            springLength = hit.distance;
            springLength = Mathf.Clamp(springLength, 0, restLength * 2);

            // Hooke's Law: F = -k * x (displacement from rest length)
            springForce = springStiffness * (restLength - springLength);

            // Damping: F = -c * v (velocity of spring compression/extension)
            springVelocity = (restLength - springLength) / Time.fixedDeltaTime;
            damperForce = damperStiffness * springVelocity;

            // Apply spring and damper forces
            Vector3 suspensionForce = (springForce - damperForce) * Vector3.up;
            rb.AddForce(suspensionForce, ForceMode.Acceleration);

            // Apply counter-gravity only when grounded
            gravityForce = -Physics.gravity * rb.mass * (springLength / restLength);
            rb.AddForce(gravityForce, ForceMode.Acceleration);
        }
    }
}
