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
