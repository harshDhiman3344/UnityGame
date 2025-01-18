using UnityEngine;

public class JellyCameraController : MonoBehaviour
{
    public Transform car; // Reference to the car object
    public float smoothSpeed = 0.125f; // How fast the camera follows
    public float rotationSpeed = 5f; // Speed at which the camera rotates to follow the car
    public float heightDamping = 2f; // Smoothly adjusts the height of the camera

    // Variables for positioning the camera behind the car
    public float distanceBehind = 10f; // Distance behind the car
    public float heightOffset = 5f; // Height offset from the car

    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        FollowCar();
    }

    // Function to make the camera follow the car
    void FollowCar()
    {
        // Calculate the desired position behind the car
        Vector3 behindPosition = car.position - car.forward * distanceBehind + Vector3.up * heightOffset;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, behindPosition, ref velocity, smoothSpeed);

        // Set the position of the camera
        transform.position = smoothedPosition;

        // Handle rotation to follow the car's direction
        Quaternion targetRotation = Quaternion.LookRotation(car.position - transform.position);

        // Smoothly rotate the camera
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
