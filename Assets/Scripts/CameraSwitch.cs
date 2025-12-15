using UnityEngine;
using UnityEngine.InputSystem; // Add this namespace

public class CameraSwitch : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    void Start()
    {
        SetFirstPerson();
    }

    void Update()
    {
        // Check for Keyboard existence to prevent errors
        if (Keyboard.current == null) return;

        // Use the New Input System syntax
        if (Keyboard.current.cKey.wasPressedThisFrame) 
        {
            if (firstPersonCamera.enabled)
                SetThirdPerson();
            else
                SetFirstPerson();

            Debug.Log("Camera switched");
        }
    }

    void SetFirstPerson()
    {
        firstPersonCamera.enabled = true;
        thirdPersonCamera.enabled = false;

        // Manage Audio Listeners (Prevent the "Two Audio Listeners" warning)
        var fpListener = firstPersonCamera.GetComponent<AudioListener>();
        var tpListener = thirdPersonCamera.GetComponent<AudioListener>();
        
        if(fpListener != null) fpListener.enabled = true;
        if(tpListener != null) tpListener.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetThirdPerson()
    {
        firstPersonCamera.enabled = false;
        thirdPersonCamera.enabled = true;

        var fpListener = firstPersonCamera.GetComponent<AudioListener>();
        var tpListener = thirdPersonCamera.GetComponent<AudioListener>();

        if(fpListener != null) fpListener.enabled = false;
        if(tpListener != null) tpListener.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}