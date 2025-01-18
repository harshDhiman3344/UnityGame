using UnityEngine;

public class WheelController : MonoBehaviour
{
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    public float maxSteeringAngle = 30f; // Maximum angle for steering
    public float motorForce = 1500f; // Force applied to the wheels
    public float brakeForce = 3000f; // Force applied when braking
    public float driftFactor = 0.95f; // Factor for drifting/sliding
    public float bounceMultiplier = 1.5f; // Multiplier for bounce effect

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateWheelMeshes();
    }

    private void FixedUpdate()
    {
        HandleWheels();
        ApplyDrift();
    }

    private void HandleWheels()
    {
        // Get the input for steering and throttle
        float steerInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float throttleInput = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow
        bool isBraking = Input.GetKey(KeyCode.Space); // Spacebar for braking

        // Apply steering to the front wheels
        float steeringAngle = steerInput * maxSteeringAngle;
        frontLeftWheelCollider.steerAngle = steeringAngle;
        frontRightWheelCollider.steerAngle = steeringAngle;

        // Apply motor torque to the rear wheels
        rearLeftWheelCollider.motorTorque = throttleInput * motorForce;
        rearRightWheelCollider.motorTorque = throttleInput * motorForce;

        // Apply brake force if braking
        if (isBraking)
        {
            ApplyBrakes();
        }
        else
        {
            ReleaseBrakes();
        }
    }

    private void ApplyBrakes()
    {
        frontLeftWheelCollider.brakeTorque = brakeForce;
        frontRightWheelCollider.brakeTorque = brakeForce;
        rearLeftWheelCollider.brakeTorque = brakeForce;
        rearRightWheelCollider.brakeTorque = brakeForce;
    }

    private void ReleaseBrakes()
    {
        frontLeftWheelCollider.brakeTorque = 0f;
        frontRightWheelCollider.brakeTorque = 0f;
        rearLeftWheelCollider.brakeTorque = 0f;
        rearRightWheelCollider.brakeTorque = 0f;
    }

    private void UpdateWheelMeshes()
    {
        UpdateWheelMesh(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelMesh(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelMesh(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelMesh(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateWheelMesh(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 wheelPosition;
        Quaternion wheelRotation;

        // Get the wheel collider's world position and rotation
        collider.GetWorldPose(out wheelPosition, out wheelRotation);

        // Update the wheel mesh position and rotation
        wheelTransform.position = wheelPosition;
        wheelTransform.rotation = wheelRotation;
    }

    private void ApplyDrift()
    {
        // Calculate drift effect
        Vector3 velocity = rb.velocity;
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(velocity, transform.forward );
        Vector3 rightVelocity = transform.right * Vector3.Dot(velocity, transform.right);

        // Apply drift factor
        rb.velocity = forwardVelocity + rightVelocity * driftFactor;

        // Add a bounce effect when the car is in the air
        if (Mathf.Abs(rb.velocity.y) > 0.1f)
        {
            rb.AddForce(Vector3.up * bounceMultiplier, ForceMode.Acceleration);
        }
    }
}