using UnityEngine;

public class WheelSuspension : MonoBehaviour
{
    public Transform wheelFL; // Front-left wheel
    public Transform wheelFR; // Front-right wheel
    public Transform wheelRL; // Rear-left wheel
    public Transform wheelRR; // Rear-right wheel

    public WheelCollider wheelColliderFL;
    public WheelCollider wheelColliderFR;
    public WheelCollider wheelColliderRL;
    public WheelCollider wheelColliderRR;

    public float wheelRadius = 0.5f; // Adjust as needed based on your wheel size
    public float wheelRotationSpeed = 100f; // Speed of wheel rotation

    void Update()
    {
        // Rotate the wheels as the car moves
        UpdateWheelRotation(wheelColliderFL, wheelFL);
        UpdateWheelRotation(wheelColliderFR, wheelFR);
        UpdateWheelRotation(wheelColliderRL, wheelRL);
        UpdateWheelRotation(wheelColliderRR, wheelRR);

        // Apply steering to the front wheels
        float steeringAngle = Input.GetAxis("Horizontal") * 30f; // Steering input
        wheelColliderFL.steerAngle = steeringAngle;
        wheelColliderFR.steerAngle = steeringAngle;
    }

    // Function to update wheel rotation based on wheel collider
    void UpdateWheelRotation(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 wheelPosition;
        Quaternion wheelRotation;

        wheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
        wheelTransform.position = wheelPosition;
        wheelTransform.rotation = wheelRotation;

        // Rotate the wheels according to the car's speed
        float wheelSpeed = wheelCollider.rpm * 2 * Mathf.PI * wheelRadius / 60f;
        wheelTransform.Rotate(Vector3.right, wheelSpeed * Time.deltaTime * wheelRotationSpeed);
    }
}
