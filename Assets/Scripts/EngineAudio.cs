using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class EngineAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource idleSound;
    [SerializeField] private AudioSource runningSound;
    [SerializeField] private AudioSource reverseSound;
    [SerializeField] private AudioSource startingSound;

    [Header("Running Sound")]
    [SerializeField] private float runningMaxVolume = 1f;
    [SerializeField] private float runningMaxPitch = 1.5f;

    [Header("Reverse Sound")]
    [SerializeField] private float reverseMaxVolume = 1f;
    [SerializeField] private float reverseMaxPitch = 1.2f;

    [Header("Idle Sound")]
    [SerializeField] private float idleMaxVolume = 0.5f;

    [Header("Rev Limiter")]
    [SerializeField] private float limiterSound = 1f;
    [SerializeField] private float limiterFrequency = 3f;
    [SerializeField] private float limiterEngage = 0.8f;

    [Header("Runtime")]
    public bool isEngineRunning;

    private CarController carController;
    private float speedRatio;
    private float revLimiter;

    void Awake()
    {
        carController = GetComponent<CarController>();

        idleSound.volume = 0f;
        runningSound.volume = 0f;
        reverseSound.volume = 0f;
    }
    

    void Update()
    {
        
        if (!carController)
            return;

        float speedValue = carController.GetSpeedRatio();
        float speedSign = Mathf.Sign(speedValue);
        speedRatio = Mathf.Abs(speedValue);

        HandleRevLimiter();

        if (!isEngineRunning)
        {
            StopAllSounds();
            return;
        }

        UpdateIdleSound();
        UpdateDriveSounds(speedSign);
    }

    void HandleRevLimiter()
    {
        if (speedRatio > limiterEngage)
        {
            revLimiter =
                (Mathf.Sin(Time.time * limiterFrequency) + 1f)
                * limiterSound
                * (speedRatio - limiterEngage);
        }
        else
        {
            revLimiter = 0f;
        }
    }

    void UpdateIdleSound()
    {
        idleSound.volume = Mathf.Lerp(0.1f, idleMaxVolume, speedRatio);
    }

    void UpdateDriveSounds(float speedSign)
    {
        if (speedSign >= 0)
        {
            reverseSound.volume = 0f;

            runningSound.volume =
                Mathf.Lerp(0.3f, runningMaxVolume, speedRatio) - revLimiter;

            runningSound.pitch =
                Mathf.Lerp(0.3f, runningMaxPitch, speedRatio);
        }
        else
        {
            runningSound.volume = 0f;

            reverseSound.volume =
                Mathf.Lerp(0f, reverseMaxVolume, speedRatio);

            reverseSound.pitch =
                Mathf.Lerp(0.2f, reverseMaxPitch, speedRatio);
        }
    }

    void StopAllSounds()
    {
        idleSound.volume = 0f;
        runningSound.volume = 0f;
        reverseSound.volume = 0f;
    }
    

    public IEnumerator StartEngine()
    {
        if (startingSound)
            startingSound.Play();

        carController.isEngineRunning = 1;
        yield return new WaitForSeconds(0.6f);

        isEngineRunning = true;
        yield return new WaitForSeconds(0.4f);

        carController.isEngineRunning = 2;
    }

}
