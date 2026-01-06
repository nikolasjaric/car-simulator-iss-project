/*
using System;
using UnityEngine;
using UnityEngine.InputSystem;  // New Input System

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBrakeForce;
    private bool isBraking;

    // Settings
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        // Reset inputs
        horizontalInput = 0f;
        verticalInput = 0f;
        isBraking = false;

        // Keyboard Input
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed) horizontalInput = 1f;
            if (Keyboard.current.wKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.sKey.isPressed) verticalInput = -1f;
            if (Keyboard.current.spaceKey.isPressed) isBraking = true;
        }

        // Gamepad Input
        if (Gamepad.current != null)
        {
            Vector2 move = Gamepad.current.leftStick.ReadValue();
            horizontalInput = move.x;
            verticalInput = move.y;
            isBraking |= Gamepad.current.rightTrigger.isPressed;
        }
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
}

*/

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBrakeForce;
    private bool isBraking;
    private Rigidbody rb;

    private bool useKinematicMode = false;

    [Header("Kinematic Settings")]
    [SerializeField] private float kinematicSpeed = 10f;
    [SerializeField] private float kinematicTurnSpeed = 60f;
    [SerializeField] private LayerMask terrainMask; // Layer za teren

    [Header("Stability")]
    [SerializeField] private Transform centerOfMass;

    [Header("Settings")]
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (centerOfMass != null)
            rb.centerOfMass = centerOfMass.localPosition;
    }

    private void FixedUpdate()
    {
        GetInput();

        if (useKinematicMode)
        {
            HandleKinematicMovement();
        }
        else
        {
            HandleMotor();
            HandleSteering();
            UpdateWheels();
        }
    }

    private void GetInput()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
        isBraking = false;

        // Keyboard
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed) horizontalInput = 1f;
            if (Keyboard.current.wKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.sKey.isPressed) verticalInput = -1f;
            if (Keyboard.current.spaceKey.isPressed) isBraking = true;

            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                useKinematicMode = !useKinematicMode;
                // rb ostaje non-kinematic, koristimo MovePosition da kontroliramo auto
                Debug.Log("Kinematic Mode: " + useKinematicMode);
            }
        }

        // Gamepad
        if (Gamepad.current != null)
        {
            Vector2 move = Gamepad.current.leftStick.ReadValue();
            if (move.magnitude > 0.1f)
            {
                horizontalInput = move.x;
                verticalInput = move.y;
            }
            isBraking |= Gamepad.current.rightTrigger.isPressed;
        }
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void HandleKinematicMovement()
    {
        // Pomak naprijed/nazad i okretanje
        Vector3 forwardMove = transform.forward * verticalInput * kinematicSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, horizontalInput * kinematicTurnSpeed * Time.fixedDeltaTime, 0f);

        Vector3 targetPos = rb.position + forwardMove;
        rb.MovePosition(targetPos);
        rb.MoveRotation(rb.rotation * turnRotation);

        // Držanje auta na terenu
        RaycastHit hit;
        if (Physics.Raycast(rb.position + Vector3.up * 2f, Vector3.down, out hit, 10f, terrainMask))
        {
            Vector3 terrainPos = hit.point;
            rb.MovePosition(new Vector3(rb.position.x, terrainPos.y, rb.position.z));
        }
    }
}
