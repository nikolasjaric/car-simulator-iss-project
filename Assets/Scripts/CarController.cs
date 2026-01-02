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
    [SerializeField] private float brakeForce = 3000f;
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


// script with added manual option


/*

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBrakeForce;
    private bool isBraking;
    private Rigidbody rb;

    // --- NEW: Transmission & Engine Variables ---
    private bool isManual = false;
    private int currentGear = 1; // 0 = Neutral, -1 = Reverse, 1-6 = Forward
    private float engineRPM;
    private float redlineTimer = 0f;
    private bool hasExploded = false;

    [Header("Stability")]
    [Tooltip("Drag an empty GameObject here to change the car's weight balance")]
    [SerializeField] private Transform centerOfMass;

    [Header("Engine Settings")]
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float maxRPM = 8000f;
    [SerializeField] private float minRPM = 1000f;
    [SerializeField] private float explodeTime = 15f; // Seconds before explosion at max RPM
    
    [Header("Transmission")]
    // Gear ratios: Index 0 is Reverse, 1 is Gear 1, etc.
    // Typical ratios: Rev(3.0), 1st(2.5), 2nd(2.0), 3rd(1.5), 4th(1.1), 5th(0.9), 6th(0.7)
    [SerializeField] private float[] gearRatios = { 3.0f, 2.5f, 2.0f, 1.5f, 1.1f, 0.9f, 0.7f };
    [SerializeField] private float finalDriveRatio = 3.4f; // Differential ratio
    [SerializeField] private float autoShiftDelay = 1.0f; // Minimal time between shifts for auto (optional)

    [Header("Audio & VFX")]
    [SerializeField] private AudioSource engineAudio;
    [SerializeField] private GameObject explosionEffect; // Drag a particle system prefab here
    [SerializeField] private AudioClip explosionSound;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels Transforms
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }

        // Setup Audio
        if (engineAudio == null) 
            engineAudio = GetComponent<AudioSource>();
            
        if(engineAudio != null && !engineAudio.isPlaying)
             engineAudio.Play();
    }

    private void FixedUpdate()
    {
        if (hasExploded) return; // Stop processing if car is dead

        GetInput();
        CalculateEnginePhysics(); // Calculate RPM
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        HandleAudio();
        CheckEngineStress(); // Check for explosion
    }

    private void GetInput()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
        isBraking = false;

        // --- Keyboard Input ---
        if (Keyboard.current != null)
        {
            // Steering
            if (Keyboard.current.aKey.isPressed) horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed) horizontalInput = 1f;
            
            // Gas / Brake
            if (Keyboard.current.wKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.sKey.isPressed) verticalInput = -1f;
            if (Keyboard.current.spaceKey.isPressed) isBraking = true;

            // --- Toggle Manual Mode ('V') ---
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                isManual = !isManual;
                Debug.Log("Manual Mode: " + isManual);
            }

            // --- Manual Shifting ---
            if (isManual)
            {
                // Shift Up (E)
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    ShiftGear(1);
                }
                // Shift Down (Q)
                if (Keyboard.current.qKey.wasPressedThisFrame)
                {
                    ShiftGear(-1);
                }
            }
        }

        // --- Gamepad Input ---
        if (Gamepad.current != null)
        {
            Vector2 move = Gamepad.current.leftStick.ReadValue();
            horizontalInput = move.x;
            verticalInput = move.y;
            isBraking |= Gamepad.current.rightTrigger.isPressed;

            // Toggle Manual (Button North / Y)
            if (Gamepad.current.buttonNorth.wasPressedThisFrame) isManual = !isManual;

            if (isManual)
            {
                if (Gamepad.current.rightShoulder.wasPressedThisFrame) ShiftGear(1);
                if (Gamepad.current.leftShoulder.wasPressedThisFrame) ShiftGear(-1);
            }
        }
    }

    private void ShiftGear(int direction)
    {
        // Direction: 1 = Up, -1 = Down
        // Gear mapping: -1 (Reverse), 0 (Neutral), 1-6 (Forward)
        
        int nextGear = currentGear + direction;

        // Clamp gear between -1 (Reverse) and 6 (6th Gear)
        currentGear = Mathf.Clamp(nextGear, -1, 6);
        
        // Debug log for checking
        Debug.Log("Gear: " + (currentGear == -1 ? "R" : currentGear == 0 ? "N" : currentGear.ToString()));
    }

    private void CalculateEnginePhysics()
    {
        // 1. Calculate Average Wheel RPM (Absolute value to handle reversing)
        float wheelRPM = (Mathf.Abs(frontLeftWheelCollider.rpm) + Mathf.Abs(frontRightWheelCollider.rpm)) / 2f;

        // 2. Determine Gear Ratio
        // If Neutral (0), engine spins freely based on gas input (simplified)
        // If Reverse (-1), use index 0 of array
        // If Forward (1-6), use index 1-6 (offset by 1 usually, but let's map directly)
        
        float currentRatio = 0f;

        if (currentGear == 0) 
        {
             // In neutral, RPM revs up fast if gas is pressed, drops if not
             engineRPM = Mathf.Lerp(engineRPM, verticalInput > 0 ? maxRPM : minRPM, Time.fixedDeltaTime * 2f);
             return; 
        }
        else if (currentGear == -1)
        {
            currentRatio = gearRatios[0]; // Use first ratio for reverse
        }
        else
        {
            // Clamp index to array bounds safely
            int ratioIndex = Mathf.Clamp(currentGear, 1, gearRatios.Length - 1);
            currentRatio = gearRatios[ratioIndex];
        }

        // 3. Calculate Engine RPM
        // EngineRPM = WheelRPM * GearRatio * FinalDrive
        float targetRPM = wheelRPM * currentRatio * finalDriveRatio;

        // Clamp RPM to min (Idle) and max (Redline)
        engineRPM = Mathf.Lerp(engineRPM, targetRPM, Time.fixedDeltaTime * 10f); // Smooth it out
        engineRPM = Mathf.Clamp(engineRPM, minRPM, maxRPM);
        
        // Automatic Gear Shifting Logic (Simple fallback if not manual)
        if (!isManual && currentGear > 0)
        {
            if (engineRPM > maxRPM * 0.9f && currentGear < 6) currentGear++;
            if (engineRPM < maxRPM * 0.4f && currentGear > 1) currentGear--;
        }
    }

    private void HandleMotor()
    {
        float torque = 0f;

        // Only apply torque if we are in gear
        if (currentGear != 0)
        {
            // Modify torque based on Gear Ratio (Lower gears = more torque, higher gears = less torque)
            // We use the same ratio logic as RPM
            float ratio = (currentGear == -1) ? gearRatios[0] : gearRatios[Mathf.Clamp(currentGear, 1, gearRatios.Length -1)];
            
            // Engine Torque Curve: Usually simplified as: Force / GearRatio
            // But for Unity WheelColliders, we often just want raw torque * ratio
            torque = verticalInput * motorForce * ratio;
            
            // Handle Reverse Input
            if (currentGear == -1 && verticalInput > 0) 
            {
                 // If in reverse gear, 'W' should move backward
                 // Depending on your setup, verticalInput might be positive. 
                 // We simply invert the torque application for Unity WheelColliders in reverse context
                 torque = -verticalInput * motorForce * ratio; 
            }
        }

        // If we hit the absolute RPM limiter, cut the gas (Rev limiter effect)
        if (engineRPM >= maxRPM)
        {
            torque = 0;
        }

        // Apply to Drive Wheels (assuming Front Wheel Drive based on your script, or convert to Rear/AWD)
        // Your original script applied to Front wheels.
        frontLeftWheelCollider.motorTorque = torque;
        frontRightWheelCollider.motorTorque = torque;

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

    private void HandleAudio()
    {
        if (engineAudio == null) return;

        // Calculate pitch based on RPM
        // Example: Idle (1000rpm) = 0.5 pitch, Max (8000rpm) = 2.0 pitch
        float pitch = Mathf.Lerp(0.8f, 2.5f, (engineRPM - minRPM) / (maxRPM - minRPM));
        engineAudio.pitch = pitch;
    }

    private void CheckEngineStress()
    {
        // Tolerance zone (e.g. within 100 RPM of max)
        if (engineRPM >= maxRPM - 100f)
        {
            redlineTimer += Time.fixedDeltaTime;
            
            // Optional: Shake camera or emit smoke here

            if (redlineTimer >= explodeTime)
            {
                ExplodeCar();
            }
        }
        else
        {
            // Reset timer if we drop below redline
            redlineTimer = 0f;
        }
    }

    private void ExplodeCar()
    {
        if (hasExploded) return;
        hasExploded = true;

        Debug.Log("ENGINE BLOWN!");

        // 1. Cut Engine Power
        frontLeftWheelCollider.motorTorque = 0;
        frontRightWheelCollider.motorTorque = 0;
        
        // 2. Play Sound
        if (engineAudio != null && explosionSound != null)
        {
            engineAudio.Stop();
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // 3. Spawn Particle Effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 4. Add Force (Visual jump)
        if (rb != null)
        {
            rb.AddExplosionForce(5000f, transform.position, 5f, 3.0f);
        }

        // 5. Disable script (optional, stops inputs)
        this.enabled = false;
    }
}

*/


/* TREBA RADITI NA NJOJ

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBrakeForce;
    private bool isBraking;
    private Rigidbody rb;

    // --- Transmission & Engine Variables ---
    private bool isManual = false;
    private int currentGear = 1; // 0 = Neutral, -1 = Reverse, 1-6 = Forward
    private float engineRPM;
    private float redlineTimer = 0f;
    private bool hasExploded = false;

    [Header("Stability")]
    [Tooltip("Drag an empty GameObject here to change the car's weight balance")]
    [SerializeField] private Transform centerOfMass;

    [Header("Engine Settings")]
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float maxRPM = 8000f;
    [SerializeField] private float minRPM = 1000f;
    [SerializeField] private float explodeTime = 15f; 
    
    [Header("Transmission")]
    [SerializeField] private float[] gearRatios = { 3.0f, 2.5f, 2.0f, 1.5f, 1.1f, 0.9f, 0.7f };
    [SerializeField] private float finalDriveRatio = 3.4f; 

    [Header("Audio & VFX")]
    [SerializeField] private AudioSource engineAudio;
    [SerializeField] private GameObject explosionEffect; 
    [SerializeField] private AudioClip explosionSound;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }

        if (engineAudio == null) 
            engineAudio = GetComponent<AudioSource>();
            
        if(engineAudio != null && !engineAudio.isPlaying)
             engineAudio.Play();
    }

    // UPDATE: Runs every frame. Use this for Input to prevent double-triggering.
    private void Update()
    {
        if (hasExploded) return;
        GetInput();
        HandleAudio(); // Audio is visual/auditory, good to update every frame for smoothness
    }

    // FIXED UPDATE: Runs on physics ticks. Use this for forces and movement.
    private void FixedUpdate()
    {
        if (hasExploded) return;

        // Input is already gathered in Update(), we just use the values here
        CalculateEnginePhysics(); 
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        CheckEngineStress(); 
    }

    private void GetInput()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
        isBraking = false;

        // --- Keyboard Input ---
        if (Keyboard.current != null)
        {
            // Steering
            if (Keyboard.current.aKey.isPressed) horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed) horizontalInput = 1f;
            
            // Gas / Brake
            if (Keyboard.current.wKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.sKey.isPressed) verticalInput = -1f;
            if (Keyboard.current.spaceKey.isPressed) isBraking = true;

            // --- Toggle Manual Mode ('V') ---
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                isManual = !isManual;
                Debug.Log("Manual Mode: " + isManual);
            }

            // --- Manual Shifting ---
            if (isManual)
            {
                if (Keyboard.current.eKey.wasPressedThisFrame) ShiftGear(1);
                if (Keyboard.current.qKey.wasPressedThisFrame) ShiftGear(-1);
            }
        }

        // --- Gamepad Input ---
        if (Gamepad.current != null)
        {
            Vector2 move = Gamepad.current.leftStick.ReadValue();
            // Don't overwrite keyboard input if gamepad is idle
            if (move.magnitude > 0.1f) 
            {
                horizontalInput = move.x;
                verticalInput = move.y;
            }
            
            if (Gamepad.current.rightTrigger.isPressed) isBraking = true;

            if (Gamepad.current.buttonNorth.wasPressedThisFrame) 
            {
                isManual = !isManual;
                Debug.Log("Manual Mode: " + isManual);
            }

            if (isManual)
            {
                if (Gamepad.current.rightShoulder.wasPressedThisFrame) ShiftGear(1);
                if (Gamepad.current.leftShoulder.wasPressedThisFrame) ShiftGear(-1);
            }
        }
    }

    private void ShiftGear(int direction)
    {
        int nextGear = currentGear + direction;
        currentGear = Mathf.Clamp(nextGear, -1, 6);
        Debug.Log("Gear: " + (currentGear == -1 ? "R" : currentGear == 0 ? "N" : currentGear.ToString()));
    }

    private void CalculateEnginePhysics()
    {
        float wheelRPM = (Mathf.Abs(frontLeftWheelCollider.rpm) + Mathf.Abs(frontRightWheelCollider.rpm)) / 2f;
        float currentRatio = 0f;

        if (currentGear == 0) 
        {
             engineRPM = Mathf.Lerp(engineRPM, verticalInput > 0 ? maxRPM : minRPM, Time.fixedDeltaTime * 2f);
             return; 
        }
        else if (currentGear == -1)
        {
            currentRatio = gearRatios[0]; 
        }
        else
        {
            int ratioIndex = Mathf.Clamp(currentGear, 1, gearRatios.Length - 1);
            currentRatio = gearRatios[ratioIndex];
        }

        float targetRPM = wheelRPM * currentRatio * finalDriveRatio;
        engineRPM = Mathf.Lerp(engineRPM, targetRPM, Time.fixedDeltaTime * 10f); 
        engineRPM = Mathf.Clamp(engineRPM, minRPM, maxRPM);
        
        // Automatic Shifting Logic
        if (!isManual && currentGear > 0 && currentGear < 6)
        {
            if (engineRPM > maxRPM * 0.9f) ShiftGear(1); // Use helper function to log it
        }
        if (!isManual && currentGear > 1)
        {
            if (engineRPM < maxRPM * 0.4f) ShiftGear(-1);
        }
    }

    private void HandleMotor()
    {
        float torque = 0f;

        if (currentGear != 0)
        {
            float ratio = (currentGear == -1) ? gearRatios[0] : gearRatios[Mathf.Clamp(currentGear, 1, gearRatios.Length -1)];
            torque = verticalInput * motorForce * ratio;
            
            if (currentGear == -1 && verticalInput > 0) 
            {
                 torque = -verticalInput * motorForce * ratio; 
            }
        }

        if (engineRPM >= maxRPM) torque = 0;

        frontLeftWheelCollider.motorTorque = torque;
        frontRightWheelCollider.motorTorque = torque;

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

    private void HandleAudio()
    {
        if (engineAudio == null) return;
        float pitch = Mathf.Lerp(0.8f, 2.5f, (engineRPM - minRPM) / (maxRPM - minRPM));
        engineAudio.pitch = pitch;
    }

    private void CheckEngineStress()
    {
        if (engineRPM >= maxRPM - 100f)
        {
            redlineTimer += Time.fixedDeltaTime;
            if (redlineTimer >= explodeTime) ExplodeCar();
        }
        else
        {
            redlineTimer = 0f;
        }
    }

    private void ExplodeCar()
    {
        if (hasExploded) return;
        hasExploded = true;
        Debug.Log("ENGINE BLOWN!");

        frontLeftWheelCollider.motorTorque = 0;
        frontRightWheelCollider.motorTorque = 0;
        
        if (engineAudio != null && explosionSound != null)
        {
            engineAudio.Stop();
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        if (explosionEffect != null) Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (rb != null) rb.AddExplosionForce(5000f, transform.position, 5f, 3.0f);

        this.enabled = false;
    }
}

*/