using UnityEngine;
using UnityEngine.InputSystem;

public class CarSounds : MonoBehaviour
{
    [SerializeField] private AudioSource hornAudioSource;
    [SerializeField] private AudioSource blinkerAudioSource;
    [SerializeField] private AudioClip carHornSound;
    [SerializeField] private AudioClip carBlinkerSound;

    
    private string lastPressedKey = null;
    
    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Horn
        if (kb.tKey.wasPressedThisFrame)
        {
            Honk();
        }

       
        string pressedKey = null;
        if (kb.bKey.wasPressedThisFrame) pressedKey = "b";
        else if (kb.qKey.wasPressedThisFrame) pressedKey = "q";
        else if (kb.eKey.wasPressedThisFrame) pressedKey = "e";

        if (pressedKey != null)
        {
            ToggleBlinker(pressedKey);
        }
    }

    public void Honk()
    {
        if (hornAudioSource == null || carHornSound == null)
        {
            Debug.LogWarning("hornAudioSource or carHornSound not assigned!");
            return;
        }

        hornAudioSource.PlayOneShot(carHornSound);
    }

    public void ToggleBlinker(string pressedKey)
    {
        if (blinkerAudioSource == null || carBlinkerSound == null)
        {
            Debug.LogWarning("blinkerAudioSource or carBlinkerSound not assigned!");
            return;
        }

        
        if (!blinkerAudioSource.isPlaying)
        {
            blinkerAudioSource.clip = carBlinkerSound;
            blinkerAudioSource.loop = true;
            blinkerAudioSource.Play();
            lastPressedKey = pressedKey;     
            return;
        }

        
        if (pressedKey == lastPressedKey)
        {
            blinkerAudioSource.Stop();
            lastPressedKey = null;
            return;
        }

        
         lastPressedKey = pressedKey;
    }
}
