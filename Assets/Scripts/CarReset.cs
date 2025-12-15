using UnityEngine;
using UnityEngine.InputSystem; // 1. Add this namespace

public class CarReset : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody carRigidbody;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        carRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 2. Check if Keyboard exists, then check for the R key
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetCar();
        }
    }

    public void ResetCar()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        if (carRigidbody != null)
        {
            carRigidbody.linearVelocity = Vector3.zero; 
            carRigidbody.angularVelocity = Vector3.zero;
        }
    }
}