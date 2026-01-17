// prije insertanja manual modea a s poboljsanim RPMom



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


    [Header("Engine")]
    [SerializeField] private float motorPower;
    [SerializeField] private float brakePower;
    [SerializeField] private float maxSpeed;
    [SerializeField] private AnimationCurve hpToRPMCurve;

    [Header("Physics Tuning")]
    [SerializeField] private float engineInertia = 10000f; // How fast the engine revs (RPM per second)



    public int isEngineRunning;

    [Header("RPM")]
    public float RPM;
    public float redLine;
    public float idleRPM;

   
    [SerializeField] private TMP_Text speedText; // Drag your speed text object here in the Inspector

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

    
    // POKUSAJ DODAVANJA PARTICLESA
    private void CheckParticles()
{
    CheckWheelParticle(colliders.FRWheel, wheelParticles.FRWheel, wheelParticles.FRWheelTrail);
    CheckWheelParticle(colliders.FLWheel, wheelParticles.FLWheel, wheelParticles.FLWheelTrail);
    CheckWheelParticle(colliders.RRWheel, wheelParticles.RRWheel, wheelParticles.RRWheelTrail);
    CheckWheelParticle(colliders.RLWheel, wheelParticles.RLWheel, wheelParticles.RLWheelTrail);
}

private void CheckWheelParticle(
    WheelCollider wheel,
    ParticleSystem particle,
    TrailRenderer trail
)
{
    if (wheel == null) return;

    WheelHit hit;

    if (wheel.GetGroundHit(out hit))
    {
        bool isSlipping =
            Mathf.Abs(hit.forwardSlip) > 0.3f ||
            Mathf.Abs(hit.sidewaysSlip) > 0.3f;

        if (isSlipping)
        {
            if (!particle.isPlaying)
                particle.Play();

            trail.emitting = true;
        }
        else
        {
            particle.Stop();
            trail.emitting = false;
        }
    }
    else
    {
        particle.Stop();
        trail.emitting = false;
    }
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
        speed = playerRB.linearVelocity.magnitude;
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        CheckParticles();
        ApplyWheelPositions();
    }



    /*
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

    */

    private void UpdateUI()
{
    // --- Needle Rotation ---
    rpmNeedle.rotation = Quaternion.Euler(
        0f,
        0f,
        Mathf.Lerp(minNeedleRotation, maxNeedleRotation, RPM / (redLine * 1.1f))
    );

    // --- RPM Text ---
    rpmText.text = $"{RPM:0} RPM";

    // --- Gear Text ---
    gearText.text = gearState == GearState.Neutral ? "N" : (currentGear + 1).ToString();

    // --- Speed Calculation ---
    // linearVelocity.magnitude is in meters per second. 
    // Multiply by 3.6 for KM/H or 2.237 for MPH.
    float currentSpeed = playerRB.linearVelocity.magnitude * 3.6f; 
    
    // Smooth the speed value slightly for the UI so it doesn't flicker
    speedClamped = Mathf.Lerp(speedClamped, currentSpeed, Time.deltaTime * 10f);
    
    // Update the Text
    if (speedText != null)
    {
        speedText.text = $"{Mathf.Abs(speedClamped):0} KM/H";
    }
}

    private void CheckInput()
    {
        // Use GetAxis for smoother acceleration and steering (it handles W/S and A/D automatically)
        gasInput = Input.GetAxis("Vertical");       // W = 1, S = -1
        steeringInput = Input.GetAxis("Horizontal"); // D = 1, A = -1

        // --- Engine start logic ---
        if (Mathf.Abs(gasInput) > 0.1f && isEngineRunning == 0 && engineAudio != null)
        {
            StartCoroutine(engineAudio.StartEngine());
            gearState = GearState.Running;
        }

        // Determine if we are moving forward or backward relative to where the car is facing
        float movingDirection = Vector3.Dot(transform.forward, playerRB.linearVelocity);

        // --- Improved Braking vs Reversing Logic ---
        // If we press S while moving forward, we BRAKE.
        // If we are almost stopped and press S, we REVERSE.
        if (movingDirection > 0.5f && gasInput < -0.1f)
        {
            brakeInput = Mathf.Abs(gasInput);
            gasInput = 0; // Don't apply motor torque while braking
        }
        // If we press W while moving backward, we BRAKE.
        else if (movingDirection < -0.5f && gasInput > 0.1f)
        {
            brakeInput = gasInput;
            gasInput = 0;
        }
        else
        {
            brakeInput = 0;
        }

        // --- Physics/Slip Calculations ---
        slipAngle = Vector3.Angle(transform.forward, playerRB.linearVelocity - transform.forward);

        // Settings
        //[SerializeField] private float motorForce = 50000f;
        //[SerializeField] private float brakeForce = 10000f;
        //[SerializeField] private float maxSteerAngle = 30f;
        // --- Clutch/Gear Logic ---
        if (gearState != GearState.Changing)
        {
            if (gearState == GearState.Neutral)
            {
                clutch = 0f;
                if (Mathf.Abs(gasInput) > 0.1f) gearState = GearState.Running;
            }
            else
            {
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0f : Mathf.Lerp(clutch, 1f, Time.deltaTime * 5f);
            }
        }
    }


    private void ApplyMotor()
    {
        // Safety check to prevent the NullReferenceException you had earlier
        if (colliders.RRWheel == null || colliders.RLWheel == null) return;

        currentTorque = CalculateTorque();

        // Apply torque to rear wheels
        // If gasInput is negative (S), the motorTorque will be negative, making the car go backward
        colliders.RRWheel.motorTorque = currentTorque * gasInput;
        colliders.RLWheel.motorTorque = currentTorque * gasInput;
    }


/*
private float CalculateTorque()
    {
        float torque = 0f;

        if (RPM < idleRPM + 200 && gasInput == 0 && currentGear == 0)
            gearState = GearState.Neutral;

        if (gearState == GearState.Running && clutch > 0f)
        {
            if (RPM > increaseGearRPM)
            {
                StartCoroutine(ChangeGear(1));
            }
            else if (RPM < decreaseGearRPM && 1 <= currentGear)
            {
                StartCoroutine(ChangeGear(-1));
            }
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

    */


    private float CalculateTorque()
{
    float torque = 0f;

    // 1. Gear state management
    if (RPM < idleRPM + 200 && gasInput == 0 && currentGear == 0)
        gearState = GearState.Neutral;

    if (gearState == GearState.Running && clutch > 0f)
    {
        if (RPM > increaseGearRPM) StartCoroutine(ChangeGear(1));
        else if (RPM < decreaseGearRPM && currentGear > 0) StartCoroutine(ChangeGear(-1));
    }

    if (isEngineRunning > 0)
    {
        if (clutch < 0.1f) // NEUTRAL / CLUTCH IN
        {
            // Calculate target RPM based on throttle
            float targetRPM = Mathf.Max(idleRPM, redLine * Mathf.Abs(gasInput));
            
            // MoveTowards is better than Lerp for constant acceleration feeling
            RPM = Mathf.MoveTowards(RPM, targetRPM + Random.Range(-50, 50), engineInertia * Time.deltaTime);
        }
        else // DRIVE / CLUTCH ENGAGED
        {
            // Calculate what the RPM SHOULD be based on wheel speed
            float wheelRPMInput = Mathf.Abs((colliders.RRWheel.rpm + colliders.RLWheel.rpm) * 0.5f);
            wheelRPM = wheelRPMInput * gearRatios[currentGear] * differentialRatio;

            // Clamp the RPM so it doesn't drop below idle or fly past redline + 500
            float targetRPM = Mathf.Clamp(wheelRPM, idleRPM - 100, redLine + 500);
            
            // Smooth the transition so the needle doesn't "flicker"
            RPM = Mathf.Lerp(RPM, targetRPM, Time.deltaTime * 10f);

            // Calculate Torque with a "Safe RPM" to prevent division by zero/low numbers
            float safeRPMForCalculation = Mathf.Max(RPM, 1000f); 
            
            float hp = hpToRPMCurve.Evaluate(RPM / redLine) * motorPower;
            torque = (hp / safeRPMForCalculation) * 5252f 
                     * gearRatios[currentGear] 
                     * differentialRatio 
                     * clutch;
        }
    }

    return torque;
}

    private void ApplySteering()
{
    if (colliders.FRWheel == null || colliders.FLWheel == null) return;

    // 1. Get the base angle from curve (ensure curve is not 0!)
    float maxSteer = steeringCurve.Evaluate(speed);
    
    // 2. Simple steering calculation
    float steeringAngle = steeringInput * maxSteer * 10f;
    // Inside ApplySteering, ensure this isn't clamping too low

    // 3. Apply only to FRONT wheels
    colliders.FRWheel.steerAngle = steeringAngle;
    colliders.FLWheel.steerAngle = steeringAngle;

    if (playerRB.linearVelocity.magnitude > 0.1f)
{
    slipAngle = Vector3.Angle(transform.forward, playerRB.linearVelocity - transform.forward);
}
else
{
    slipAngle = 0f;
}
    
    // Debug: Uncomment the line below to see the value in the Console
    // Debug.Log("Steering Angle: " + steeringAngle);
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



