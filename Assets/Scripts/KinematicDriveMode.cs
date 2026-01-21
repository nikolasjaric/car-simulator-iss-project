using UnityEngine;
using UnityEngine.InputSystem;

public class KinematicDriveMode : MonoBehaviour
{
    [Header("UI")]
    public GameObject speedUIRoot; 

    [Header("References")]
    public Rigidbody rb;
    public CarController automaticController;
    public ManualCarController manualController;

    [Header("Kinematic Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 60f;
    public LayerMask terrainMask;

    private bool isKinematicMode;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (isKinematicMode)
                DisableKinematicMode();
            else
                EnableKinematicMode();
        }
    }

    void FixedUpdate()
    {
    if (!isKinematicMode) return;

    if (!HasMovementInput())
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        return;
    }

    rb.useGravity = true;
    HandleKinematicMovement();
    }


    void EnableKinematicMode()
    {
        isKinematicMode = true;

        if (automaticController) automaticController.enabled = false;
        if (manualController) manualController.enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (speedUIRoot != null)
            speedUIRoot.SetActive(false);

        Debug.Log("KINEMATIC MODE ON");
    }

    void DisableKinematicMode()
    {
    isKinematicMode = false;

    rb.useGravity = true;

    if (automaticController) automaticController.enabled = true;

    if (speedUIRoot != null)
        speedUIRoot.SetActive(true);

    Debug.Log("KINEMATIC MODE OFF");
    }


    void HandleKinematicMovement()
    {
        float move = 0f;
        float turn = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) move = 1f;
            if (Keyboard.current.sKey.isPressed) move = -1f;
            if (Keyboard.current.aKey.isPressed) turn = -1f;
            if (Keyboard.current.dKey.isPressed) turn = 1f;
        }

        Vector3 forwardMove =
            transform.forward * move * moveSpeed * Time.fixedDeltaTime;

        Quaternion turnRotation =
            Quaternion.Euler(0f, turn * turnSpeed * Time.fixedDeltaTime, 0f);

        rb.MovePosition(rb.position + forwardMove);
        rb.MoveRotation(rb.rotation * turnRotation);

        StickToGround();
    }

    void StickToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(rb.position + Vector3.up * 2f, Vector3.down,
            out hit, 5f, terrainMask))
        {
            Vector3 pos = rb.position;
            pos.y = hit.point.y + 0.5f;
            rb.MovePosition(pos);
        }
    }
    bool HasMovementInput()
    {
    if (Keyboard.current == null) return false;

    return
        Keyboard.current.wKey.isPressed ||
        Keyboard.current.sKey.isPressed ||
        Keyboard.current.aKey.isPressed ||
        Keyboard.current.dKey.isPressed;
    }

}
