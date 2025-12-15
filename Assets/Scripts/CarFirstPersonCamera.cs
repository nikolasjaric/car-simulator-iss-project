

/*
using UnityEngine;
public class CarFirstPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 120f;
    public Transform carBody;

    float xRotation = 0f;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }
void Update()
    {
        if (!cam.enabled) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Handle Up/Down rotation (Pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);

        // Handle Left/Right rotation (Yaw) - Apply to Camera ONLY, not carBody
        transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y + mouseX, 0f);
        
        // Remove this line:
        // carBody.Rotate(Vector3.up * mouseX); 
    }
}


*/


/*
using UnityEngine;
using UnityEngine.InputSystem; // 1. REQUIRED for New Input System

public class CarFirstPersonCamera : MonoBehaviour
{
    // 2. Lowered default sensitivity because New Input values are higher
    public float mouseSensitivity = 10f; 
    public Transform carBody;

    float xRotation = 0f;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        // Optional: Lock cursor to center so it doesn't leave the window
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    void Update()
    {
        if (!cam.enabled) return;

        // 3. Safety Check: Ensure a mouse is connected
        if (Mouse.current == null) return;

        // 4. Get Raw Mouse Input
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Calculate rotation
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // Handle Up/Down rotation (Pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);

        // Handle Left/Right rotation (Yaw) - Apply to Camera ONLY
        // We use the current y rotation and add the mouseX
        float currentY = transform.localEulerAngles.y;
        transform.localRotation = Quaternion.Euler(xRotation, currentY + mouseX, 0f);
    }
}

*/


/*
using UnityEngine;
using UnityEngine.InputSystem;

public class CarFirstPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 10f; 
    public Transform carBody; 
    
    // Dedicated variables to hold our stable rotation values
    float xRotation = 0f;
    float yRotation = 0f; // This is the key to stable left/right rotation
    
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
        
        // Initialize yRotation with the camera's starting local Y angle
        yRotation = transform.localEulerAngles.y;
    }

    void Update()
    {
        if (!cam.enabled) return;
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Calculate rotation based on Time.deltaTime for frame rate independence
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // 1. Handle Pitch (Up/Down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f); // Prevents looking behind
        
        // 2. Handle Yaw (Left/Right)
        yRotation += mouseX; // Increment our stable Y rotation variable

        // 3. Apply both rotations at once
        // We set the local rotation, which is relative to the stable CameraPivot parent.
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        
        // IMPORTANT: The script never touches transform.position, so the position is fixed by its parent.
    }
}

*/


using UnityEngine;

public class CarFirstPersonCamera : MonoBehaviour
{
    [Tooltip("Drag the GameObject representing the driver's eye position here")]
    public Transform cameraMountPoint; 

    // We use LateUpdate because the Car moves in Update/FixedUpdate.
    // The camera should move LAST to prevent jittering.
    void LateUpdate()
    {
        if (cameraMountPoint == null) return;

        // 1. Snap Position to the mount point
        transform.position = cameraMountPoint.position;

        // 2. Snap Rotation to the mount point (Look where the car looks)
        transform.rotation = cameraMountPoint.rotation;
    }
}