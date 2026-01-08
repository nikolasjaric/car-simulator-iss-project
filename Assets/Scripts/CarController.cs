// PRVOBITNA VOZNJA STARTER
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

// POKUSAJ IZ VIDEA

using System.Collections;
using UnityEngine;
using TMPro;

public enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
}

public class CarController : MonoBehaviour
{
    [Header("References")]
    private Rigidbody playerRB;

    private EngineAudio engineAudio;


    [SerializeField] private WheelColliders colliders;
    [SerializeField] private WheelMeshes wheelMeshes;
    [SerializeField] private WheelParticles wheelParticles;

    [Header("Input")]
    private float gasInput;
    private float brakeInput;
    private float steeringInput;

    /*
    [SerializeField] private MyButton gasPedal;
    [SerializeField] private MyButton brakePedal;
    [SerializeField] private MyButton leftButton;
    [SerializeField] private MyButton rightButton;
    */

    [Header("Engine")]
    [SerializeField] private float motorPower;
    [SerializeField] private float brakePower;
    [SerializeField] private float maxSpeed;
    [SerializeField] private AnimationCurve hpToRPMCurve;




    public int isEngineRunning;

    [Header("RPM")]
    public float RPM;
    public float redLine;
    public float idleRPM;

    [SerializeField] private TMP_Text rpmText;
    [SerializeField] private TMP_Text gearText;
    [SerializeField] private Transform rpmNeedle;
    [SerializeField] private float minNeedleRotation;
    [SerializeField] private float maxNeedleRotation;

    [Header("Gears")]
    public int currentGear;
    public float[] gearRatios;
    public float differentialRatio;

    private float currentTorque;
    private float clutch;
    private float wheelRPM;
    private GearState gearState;

    public float increaseGearRPM;
    public float decreaseGearRPM;
    public float changeGearTime = 0.5f;

    [Header("Steering")]
    public float slipAngle;
    public AnimationCurve steeringCurve;

    [Header("Visuals")]
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private GameObject tireTrail;

    [SerializeField] private Material brakeMaterial;
    [SerializeField] private Color brakingColor;
    [SerializeField] private float brakeColorIntensity;

    private float speed;
    private float speedClamped;

    private void Awake()
    {
        TryGetComponent(out playerRB);
        TryGetComponent(out engineAudio);

    }

    private void Start()
    {
        //InitiateParticles();
    }

    private void Update()
    {
        UpdateUI();
        CheckInput();
    }

    private void FixedUpdate()
    {
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        //CheckParticles();
        ApplyWheelPositions();
    }

    private void UpdateUI()
    {
        rpmNeedle.rotation = Quaternion.Euler(
            0f,
            0f,
            Mathf.Lerp(minNeedleRotation, maxNeedleRotation, RPM / (redLine * 1.1f))
        );

        rpmText.text = $"{RPM:0,000} rpm";
        gearText.text = gearState == GearState.Neutral ? "N" : (currentGear + 1).ToString();

        speed = colliders.RRWheel.rpm * colliders.RRWheel.radius * 2f * Mathf.PI / 10f;
        speedClamped = Mathf.Lerp(speedClamped, speed, Time.deltaTime);
    }

 /*
    private void CheckInput()
    {
        gasInput = Input.GetAxis("Vertical");

        if (gasPedal.isPressed) gasInput += gasPedal.dampenPress;
        if (brakePedal.isPressed) gasInput -= brakePedal.dampenPress;

        if (Mathf.Abs(gasInput) > 0 && isEngineRunning == 0)
        {
            StartCoroutine(GetComponent<EngineAudio>().StartEngine());
            gearState = GearState.Running;
        }

        steeringInput = Input.GetAxis("Horizontal");
        if (rightButton.isPressed) steeringInput += rightButton.dampenPress;
        if (leftButton.isPressed) steeringInput -= leftButton.dampenPress;

        slipAngle = Vector3.Angle(transform.forward, playerRB.linearVelocity - transform.forward);

        float movingDirection = Vector3.Dot(transform.forward, playerRB.linearVelocity);

        if (gearState != GearState.Changing)
        {
            if (gearState == GearState.Neutral)
            {
                clutch = 0f;
                if (Mathf.Abs(gasInput) > 0f)
                    gearState = GearState.Running;
            }
            else
            {
                clutch = Input.GetKey(KeyCode.LeftShift)
                    ? 0f
                    : Mathf.Lerp(clutch, 1f, Time.deltaTime);
            }
        }
        else
        {
            clutch = 0f;
        }

        if ((movingDirection < -0.5f && gasInput > 0f) ||
            (movingDirection > 0.5f && gasInput < 0f))
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else
        {
            brakeInput = 0f;
        }
    }
    */

    private void CheckInput()
{
    // --- Gas and brake ---
    gasInput = 0f;
    brakeInput = 0f;

    if (Input.GetKey(KeyCode.W))
        gasInput = 1f;

    if (Input.GetKey(KeyCode.S))
        gasInput = -1f; // reverse/backward

    // --- Steering ---
    steeringInput = 0f;

    if (Input.GetKey(KeyCode.A))
        steeringInput = -1f;

    if (Input.GetKey(KeyCode.D))
        steeringInput = 1f;

    // --- Engine start ---
    if (Mathf.Abs(gasInput) > 0f && isEngineRunning == 0)
    {
        //StartCoroutine(GetComponent<EngineAudio>().StartEngine());
        gearState = GearState.Running;
    }

    // --- Physics calculations ---
    slipAngle = Vector3.Angle(transform.forward, playerRB.linearVelocity - transform.forward);
    float movingDirection = Vector3.Dot(transform.forward, playerRB.linearVelocity);

    if (gearState != GearState.Changing)
    {
        if (gearState == GearState.Neutral)
        {
            clutch = 0f;
            if (Mathf.Abs(gasInput) > 0f)
                gearState = GearState.Running;
        }
        else
        {
            clutch = Input.GetKey(KeyCode.LeftShift)
                ? 0f
                : Mathf.Lerp(clutch, 1f, Time.deltaTime);
        }
    }
    else
    {
        clutch = 0f;
    }

    // --- Braking logic ---
    if ((movingDirection < -0.5f && gasInput > 0f) ||
        (movingDirection > 0.5f && gasInput < 0f))
    {
        brakeInput = Mathf.Abs(gasInput);
    }
    else
    {
        brakeInput = 0f;
    }
}


    private void ApplyMotor()
    {
        currentTorque = CalculateTorque();
        colliders.RRWheel.motorTorque = currentTorque * gasInput;
        colliders.RLWheel.motorTorque = currentTorque * gasInput;
    }

    private float CalculateTorque()
    {
        float torque = 0f;

        if (RPM < idleRPM + 200 && gasInput == 0 && currentGear == 0)
            gearState = GearState.Neutral;

        if (gearState == GearState.Running && clutch > 0f)
        {
            if (RPM > increaseGearRPM) StartCoroutine(ChangeGear(1));
            else if (RPM < decreaseGearRPM) StartCoroutine(ChangeGear(-1));
        }

        if (isEngineRunning > 0)
        {
            if (clutch < 0.1f)
            {
                RPM = Mathf.Lerp(
                    RPM,
                    Mathf.Max(idleRPM, redLine * gasInput) + Random.Range(-50, 50),
                    Time.deltaTime
                );
            }
            else
            {
                wheelRPM = Mathf.Abs((colliders.RRWheel.rpm + colliders.RLWheel.rpm) * 0.5f)
                           * gearRatios[currentGear]
                           * differentialRatio;

                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);

                torque =
                    hpToRPMCurve.Evaluate(RPM / redLine) * motorPower / RPM
                    * gearRatios[currentGear]
                    * differentialRatio
                    * 5252f
                    * clutch;

            }
        }

        return torque;
    }

    private void ApplySteering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);

        if (slipAngle < 120f)
        {
            steeringAngle += Vector3.SignedAngle(
                transform.forward,
                playerRB.linearVelocity + transform.forward,
                Vector3.up
            );
        }

        steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    private void ApplyBrake()
    {
        colliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;

        if (!brakeMaterial) return;

        if (brakeInput > 0f)
        {
            brakeMaterial.EnableKeyword("_EMISSION");
            brakeMaterial.SetColor("_EmissionColor",
                brakingColor * Mathf.Pow(2f, brakeColorIntensity));
        }
        else
        {
            brakeMaterial.DisableKeyword("_EMISSION");
            brakeMaterial.SetColor("_EmissionColor", Color.black);
        }
    }

    public float GetSpeedRatio()
    {
        float gas = Mathf.Clamp(Mathf.Abs(gasInput), 0.5f, 1f);
        return RPM * gas / redLine;
    }

    private void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }

    private static void UpdateWheel(WheelCollider collider, MeshRenderer mesh)
    {
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.transform.SetPositionAndRotation(pos, rot);
    }

    private IEnumerator ChangeGear(int direction)
    {
        gearState = GearState.CheckingChange;

        if (currentGear + direction < 0) yield break;

        yield return new WaitForSeconds(direction > 0 ? 0.7f : 0.1f);

        gearState = GearState.Changing;
        yield return new WaitForSeconds(changeGearTime);

        currentGear = Mathf.Clamp(currentGear + direction, 0, gearRatios.Length - 1);
        gearState = GearState.Running;
    }
}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}

[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}

[System.Serializable]
public class WheelParticles
{
    public ParticleSystem FRWheel;
    public ParticleSystem FLWheel;
    public ParticleSystem RRWheel;
    public ParticleSystem RLWheel;

    public TrailRenderer FRWheelTrail;
    public TrailRenderer FLWheelTrail;
    public TrailRenderer RRWheelTrail;
    public TrailRenderer RLWheelTrail;
}













/// ISPOD NEKI NEUSPJELI POKUSAJI

/*
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{

        
    private TransmissionController transmission;
    public Rigidbody Rb => rb; // expose rb if needed


    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBrakeForce;
    private bool isBraking;
    public Rigidbody rb; // Reference to the car's physics body

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();   // Rigidbody is now guaranteed to exist early
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        transmission = GetComponent<TransmissionController>();

        
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
    float torque;

    if (transmission != null)
    {
        torque = transmission.GetTorque(verticalInput) * motorForce;
    }
    else
    {
        torque = verticalInput * motorForce; // fallback
    }

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
}
*/

/*
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    private TransmissionController transmission;
    private Rigidbody rb;
    
    // This property ensures that if another script asks for the Rb, it gets it even if Start hasn't run yet
    public Rigidbody Rb 
    {
        get {
            if (rb == null) rb = GetComponent<Rigidbody>();
            return rb;
        }
    }

    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBrakeForce;
    private bool isBraking;

    [Header("Stability")]
    [SerializeField] private Transform centerOfMass; 

    [Header("Settings")]
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [Header("Wheels Visuals")]
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        transmission = GetComponent<TransmissionController>();
        
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

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed) horizontalInput = 1f;
            if (Keyboard.current.wKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.sKey.isPressed) verticalInput = -1f;
            if (Keyboard.current.spaceKey.isPressed) isBraking = true;
        }

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
        float torque = (transmission != null) ? transmission.GetTorque(verticalInput) * motorForce : verticalInput * motorForce;

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
        if (wheelTransform == null) return;
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
}


*/

/*

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    public Rigidbody Rb { get { if (rb == null) rb = GetComponent<Rigidbody>(); return rb; } }

    [Header("Settings")]
    [SerializeField] private float motorForce = 1200f; // Adjusted for "Average Car" feel
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;

    [Header("Wheels")]
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private float horizontalInput, verticalInput;
    private bool isBraking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
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

    // 1. Keyboard Input
    if (Keyboard.current != null)
    {
        if (Keyboard.current.wKey.isPressed) verticalInput = 1f;
        if (Keyboard.current.sKey.isPressed) verticalInput = -1f;
        if (Keyboard.current.aKey.isPressed) horizontalInput = -1f;
        if (Keyboard.current.dKey.isPressed) horizontalInput = 1f;
        if (Keyboard.current.spaceKey.isPressed) isBraking = true;
    }

    // 2. Gamepad Input with Deadzone Fix
    if (Gamepad.current != null)
    {
        Vector2 move = Gamepad.current.leftStick.ReadValue();
        
        // Only take input if moved more than 10% (prevents drift)
        if (Mathf.Abs(move.y) > 0.1f) verticalInput = move.y;
        if (Mathf.Abs(move.x) > 0.1f) horizontalInput = move.x;
        
        isBraking |= Gamepad.current.rightTrigger.isPressed;
    }
}

private void HandleMotor()
{
    float torque = 0f;

   

    rearLeftWheelCollider.motorTorque = torque;
    rearRightWheelCollider.motorTorque = torque;

    // 3. Idle Braking Fix
    // If no input is given and we aren't manually braking, 
    // apply a tiny brake force to stop the car from "creeping"
    float currentBrakeForce = isBraking ? brakeForce : 0f;

    if (verticalInput == 0 && !isBraking)
    {
        currentBrakeForce = 100f; // Small force to hold the car still
    }

    frontLeftWheelCollider.brakeTorque = currentBrakeForce;
    frontRightWheelCollider.brakeTorque = currentBrakeForce;
    rearLeftWheelCollider.brakeTorque = currentBrakeForce;
    rearRightWheelCollider.brakeTorque = currentBrakeForce;
}

    private void HandleSteering()
    {
        float steerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
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
        if (wheelTransform == null) return;
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
}

*/