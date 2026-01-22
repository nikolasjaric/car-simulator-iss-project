using UnityEngine;

public class GearModeSwitcher : MonoBehaviour
{
    public CarController automaticController;
    public ManualCarController manualController;

    private bool isAutomatic = true;

    void Start()
    {
        // Default state
        SetAutomatic();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMode();
        }
    }

    void ToggleMode()
    {
        if (isAutomatic)
        {
            SetManual();
        }
        else
        {
            SetAutomatic();
        }
    }

    void SetAutomatic()
    {
        isAutomatic = true;
        automaticController.enabled = true;
        manualController.enabled = false;
        Debug.Log("Mode: Automatic");
    }

    void SetManual()
    {
        isAutomatic = false;
        automaticController.enabled = false;
        manualController.enabled = true;
        Debug.Log("Mode: Manual");
    }
}

