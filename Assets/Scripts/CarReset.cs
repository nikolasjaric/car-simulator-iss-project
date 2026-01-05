
/*
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

*/
// za random spawnanje

/*

using UnityEngine;
using UnityEngine.InputSystem;

public class CarReset : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;

    private Rigidbody carRigidbody;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        SpawnAtRandomPoint();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SpawnAtRandomPoint();
        }
    }

    private void SpawnAtRandomPoint()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        // Pick a random spawn point
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

        transform.position = spawn.position;
        transform.rotation = spawn.rotation;

        if (carRigidbody != null)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
        }
    }
}

*/


// drugi pokusaj

using UnityEngine;
using UnityEngine.InputSystem;

public class CarReset : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Drop empty GameObjects here to act as potential spawn locations")]
    [SerializeField] private Transform[] spawnPoints;

    private Rigidbody carRigidbody;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        
        // Randomize position immediately when the simulator starts
        ResetCar();
    }

    void Update()
    {
        // Check for the R key
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetCar();
        }
    }

    public void ResetCar()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned to CarReset script!");
            return;
        }

        // 1. Pick a random index from the array
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        // 2. Move the car to the selected point
        transform.position = selectedPoint.position;
        transform.rotation = selectedPoint.rotation;

        // 3. Reset physics so the car doesn't keep its old momentum
        if (carRigidbody != null)
        {
            carRigidbody.linearVelocity = Vector3.zero; 
            carRigidbody.angularVelocity = Vector3.zero;
        }
        
        Debug.Log($"Car reset to spawn point: {selectedPoint.name}");
    }
}