using UnityEngine;
using UnityEngine.InputSystem;

public class CarSounds : MonoBehaviour
{
    [SerializeField] private AudioSource hornAudioSource;
    [SerializeField] private AudioSource blinkerAudioSource;
    [SerializeField] private AudioClip carHornSound;
    [SerializeField] private AudioClip carBlinkerSound;
    
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            Honk();
        }

        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {

            ToggleBlinker();

        }
    }

    public void Honk()
    {
        if (hornAudioSource == null || carHornSound == null)
        {
            Debug.LogWarning("hornAudioSource or Horn sound not assigned!");
            return;
        }

        hornAudioSource.PlayOneShot(carHornSound);
    }
    public void ToggleBlinker()
    {
    if (blinkerAudioSource == null || carBlinkerSound == null) {
        Debug.LogWarning("hornAudioSource or Horn sound not assigned!");
        return;
    }
    
    if (blinkerAudioSource.isPlaying)
    {
        blinkerAudioSource.Stop();
    }
    else
    {
        blinkerAudioSource.clip = carBlinkerSound;
        blinkerAudioSource.loop = true;
        blinkerAudioSource.Play();
    }
}
}



/*using UnityEngine;
using UnityEngine.InputSystem;

public class CarSounds : MonoBehaviour
{

    public AudioClip carHornSound;

    public void Honk()
    {
        AudioSource engineSource = GetComponent<AudioSource>();
        engineSource.PlayOneShot(carHornSound);
    }

    void Update()
{
    if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
    {
        Honk();
    }
}
    
    public AudioSource engineAudioSource;
    public AudioClip engineIdleClip;
    public AudioClip engineDrivingClip;

    private bool isDriving = false;

    void Start()
    {
        SetEngineIdle();
    }

    void Update()
    {
        // Check for Keyboard existence to prevent errors
        if (Keyboard.current == null) return;

        // Use the New Input System syntax
        if (Keyboard.current.dKey.isPressed)
        {
            if (!isDriving)
            {
                SetEngineDriving();
            }
        }
        else
        {
            if (isDriving)
            {
                SetEngineIdle();
            }
        }
    }

    void SetEngineIdle()
    {
        engineAudioSource.clip = engineIdleClip;
        engineAudioSource.Play();
        isDriving = false;
    }

    void SetEngineDriving()
    {
        engineAudioSource.clip = engineDrivingClip;
        engineAudioSource.Play();
        isDriving = true;
    }
}*/