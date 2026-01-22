using UnityEngine;

public class ManualCarController : MonoBehaviour
{
    [Header("Engine Settings")]
    public AnimationCurve torqueCurve; // Define your torque (Y) vs RPM (X)
    public float maxRPM = 7000f;
    public float idleRPM = 1000f;
    public float engineRPM;

    [Header("Gearbox Settings")]
    public float[] gearRatios; // e.g., 1st: 3.5, 2nd: 2.1, etc.
    public float finalDriveRatio = 3.4f;
    public int currentGear = 1; // 0 = Reverse, 1 = Neutral, 2+ = Gears
    
    [Header("Input")]
    public float clutchInput; // 0 = Fully engaged, 1 = Fully pressed (disengaged)
    private float throttleInput;

    [Header("Wheel Colliders")]
    public WheelCollider[] driveWheels;

    void Update()
    {
        throttleInput = Input.GetAxis("Vertical");
        clutchInput = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 0.0f; // Simplified clutch key

        HandleGears();
        CalculateRPM();
        ApplyTorque();
    }

    void CalculateRPM()
    {
        // Calculate wheel RPM
        float wheelRPM = 0;
        foreach (var wheel in driveWheels) wheelRPM += wheel.rpm;
        wheelRPM /= driveWheels.Length;

        // If clutch is pressed or in neutral, engine is decoupled
        if (clutchInput > 0.5f || currentGear == 1)
        {
            engineRPM = Mathf.Lerp(engineRPM, idleRPM + (throttleInput * maxRPM), Time.deltaTime * 2f);
        }
        else
        {
            // Engine RPM is tied to Wheel RPM via Gear Ratio
            float targetRPM = wheelRPM * gearRatios[currentGear] * finalDriveRatio;
            engineRPM = Mathf.Clamp(targetRPM, idleRPM, maxRPM);
        }
    }

    void ApplyTorque()
    {
        // Get torque from curve based on current RPM
        float engineTorque = torqueCurve.Evaluate(engineRPM) * throttleInput;
        
        // Calculate total torque to wheels
        // Torque is 0 if clutch is fully pressed
        float transmissionTorque = engineTorque * gearRatios[currentGear] * finalDriveRatio * (1 - clutchInput);

        foreach (var wheel in driveWheels)
        {
            wheel.motorTorque = transmissionTorque / driveWheels.Length;
        }
    }

    void HandleGears()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentGear < gearRatios.Length - 1) currentGear++;
        if (Input.GetKeyDown(KeyCode.Q) && currentGear > 0) currentGear--;
    }
}