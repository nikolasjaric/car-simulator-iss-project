using UnityEngine;
using UnityEngine.InputSystem;

public class CarLights : MonoBehaviour
{
    [Header("Headlights")]
    [SerializeField] private Light headlightLeft;
    [SerializeField] private Light headlightRight;
    [SerializeField] private bool headlightsOnAtStart = true;

    [Header("Brake Lights")]
    [SerializeField] private Light brakeLightLeft;
    [SerializeField] private Light brakeLightRight;

    [Header("Blinkers")]
    [SerializeField] private Light blinkerFrontLeft;
    [SerializeField] private Light blinkerRearLeft;
    [SerializeField] private Light blinkerFrontRight;
    [SerializeField] private Light blinkerRearRight;
    [SerializeField] private float blinkerInterval = 0.5f; // seconds on/off

    private bool headlightsOn;

    private enum BlinkerMode
    {
        Off,
        Left,
        Right,
        Hazard
    }

    private BlinkerMode blinkerMode = BlinkerMode.Off;
    private float blinkerTimer = 0f;
    private bool blinkerStateOn = false; // current on/off phase of the blink

    void Start()
    {
        headlightsOn = headlightsOnAtStart;
        SetHeadlights(headlightsOn);
        SetBrakeLights(false);
        SetAllBlinkers(false);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Headlights toggle (L)
        if (kb.lKey.wasPressedThisFrame)
        {
            headlightsOn = !headlightsOn;
            SetHeadlights(headlightsOn);
        }

        // Brake lights (Space)
        bool braking = kb.sKey.isPressed;
        SetBrakeLights(braking);

        // Blinkers control:
        // Q -> left
        if (kb.qKey.wasPressedThisFrame)
        {
            // Pressing the same direction again turns them off
            if (blinkerMode == BlinkerMode.Left)
                SetBlinkerMode(BlinkerMode.Off);
            else
                SetBlinkerMode(BlinkerMode.Left);
        }

        // E -> right
        if (kb.eKey.wasPressedThisFrame)
        {
            if (blinkerMode == BlinkerMode.Right)
                SetBlinkerMode(BlinkerMode.Off);
            else
                SetBlinkerMode(BlinkerMode.Right);
        }

        // B -> hazard (all four)
        if (kb.bKey.wasPressedThisFrame)
        {
            if (blinkerMode == BlinkerMode.Hazard)
                SetBlinkerMode(BlinkerMode.Off);
            else
                SetBlinkerMode(BlinkerMode.Hazard);
        }

        // Handle blinking over time
        UpdateBlinkers(Time.deltaTime);
    }

    public void SetHeadlights(bool on)
    {
        if (headlightLeft) headlightLeft.enabled = on;
        if (headlightRight) headlightRight.enabled = on;
    }

    public void SetBrakeLights(bool on)
    {
        if (brakeLightLeft) brakeLightLeft.enabled = on;
        if (brakeLightRight) brakeLightRight.enabled = on;
    }

    // ---- Blinkers ----

    private void SetBlinkerMode(BlinkerMode mode)
    {
        blinkerMode = mode;
        blinkerTimer = 0f;
        blinkerStateOn = false;

        // When mode changes, turn everything off first
        SetAllBlinkers(false);
    }

    private void UpdateBlinkers(float deltaTime)
    {
        if (blinkerMode == BlinkerMode.Off)
        {
            // Ensure they stay off
            SetAllBlinkers(false);
            return;
        }

        blinkerTimer += deltaTime;
        if (blinkerTimer >= blinkerInterval)
        {
            blinkerTimer = 0f;
            blinkerStateOn = !blinkerStateOn; // toggle on/off phase

            ApplyBlinkerState();
        }
    }

    private void ApplyBlinkerState()
    {
        switch (blinkerMode)
        {
            case BlinkerMode.Left:
                SetLeftBlinkers(blinkerStateOn);
                SetRightBlinkers(false);
                break;

            case BlinkerMode.Right:
                SetRightBlinkers(blinkerStateOn);
                SetLeftBlinkers(false);
                break;

            case BlinkerMode.Hazard:
                SetLeftBlinkers(blinkerStateOn);
                SetRightBlinkers(blinkerStateOn);
                break;
        }
    }

    private void SetLeftBlinkers(bool on)
    {
        if (blinkerFrontLeft) blinkerFrontLeft.enabled = on;
        if (blinkerRearLeft) blinkerRearLeft.enabled = on;
    }

    private void SetRightBlinkers(bool on)
    {
        if (blinkerFrontRight) blinkerFrontRight.enabled = on;
        if (blinkerRearRight) blinkerRearRight.enabled = on;
    }

    private void SetAllBlinkers(bool on)
    {
        SetLeftBlinkers(on);
        SetRightBlinkers(on);
    }
}



/*using UnityEngine;
using UnityEngine.InputSystem;

public class CarLights : MonoBehaviour
{
    [Header("Headlights")]
    [SerializeField] private Light headlightLeft;
    [SerializeField] private Light headlightRight;
    [SerializeField] private bool headlightsOnAtStart = true;

    [Header("Brake Lights")]
    [SerializeField] private Light brakeLightLeft;
    [SerializeField] private Light brakeLightRight;

    private bool headlightsOn;

    void Start()
    {
        headlightsOn = headlightsOnAtStart;
        SetHeadlights(headlightsOn);
        SetBrakeLights(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {
            headlightsOn = !headlightsOn;
            SetHeadlights(headlightsOn);
        }

        if (Keyboard.current != null)
        {
            bool braking = Keyboard.current.spaceKey.isPressed;
            SetBrakeLights(braking);
        }
    }

    public void SetHeadlights(bool on)
    {
        if (headlightLeft) headlightLeft.enabled = on;
        if (headlightRight) headlightRight.enabled = on;
    }

    public void SetBrakeLights(bool on)
    {
        if (brakeLightLeft) brakeLightLeft.enabled = on;
        if (brakeLightRight) brakeLightRight.enabled = on;
    }
}
*/