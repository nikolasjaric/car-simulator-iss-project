using UnityEngine;

public class CarThirdPersonCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform carTransform; // Drag your Car_1 here

    [Header("Camera Settings")]
    public float distance = 6.0f;    // How far behind the car
    public float height = 2.5f;      // How high above the car
    public float followSpeed = 10f;  // How fast it moves to catch up
    public float lookSpeed = 10f;    // How fast it rotates to look at the car

    private void FixedUpdate()
    {
        if (carTransform == null) return;

        // 1. Calculate where the camera WANTS to be (Behind and above the car)
        // We use the car's forward direction to stay behind it, but we ignore the car's chaotic rotation
        Vector3 targetPosition = carTransform.position - (carTransform.forward * distance) + (Vector3.up * height);

        // 2. Smoothly move the camera to that position
        // Vector3.Lerp moves smoothly between two points
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // 3. Look at the car
        // We also smooth the rotation so it doesn't snap instantly
        Vector3 directionToCar = carTransform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCar);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
    }
}