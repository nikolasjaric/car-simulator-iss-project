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
    private Rigidbody rb; // Reference to the car's physics body

    // Stability
    [Header("Stability")]
    [Tooltip("Drag an empty GameObject here to change the car's weight balance")]
    [SerializeField] private Transform centerOfMass; 

    // Settings
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 6000f;
    [SerializeField] private float maxSteerAngle = 30f;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // This is the magic line that stops the car from flipping
        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
        isBraking = false;

        // Keyboard Input
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed) horizontalInput = 1f;
            if (Keyboard.current.wKey.isPressed) verticalInput = 5f;
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