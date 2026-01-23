using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(AudioSource))]
public class EngineAudio : MonoBehaviour
{
    [Header("Audio Source (single)")]
    [SerializeField] private AudioSource engineSource;

    [Header("Engine Clips")]
    [SerializeField] private AudioClip idleClip;      // e.g. idle or low_on
    [SerializeField] private AudioClip drivingClip;   // e.g. med_on / high_on
    [SerializeField] private AudioClip reverseClip;   // optional, can reuse drivingClip
    [SerializeField] private AudioClip startupClip;   // startup

    [Header("Pitch & Volume")]
    [Range(0f, 3f)]  [SerializeField] private float minPitch  = 0.8f;
    [Range(0f, 3f)]  [SerializeField] private float maxPitch  = 2.0f;
    [Range(0f, 1f)]  [SerializeField] private float minVolume = 0.25f;
    [Range(0f, 1f)]  [SerializeField] private float maxVolume = 0.7f; // driving not too loud

    [Header("Runtime")]
    public bool IsEngineRunning => isEngineRunning;

    private CarController carController;
    private float speedRatio;        // 0..1 based on GetSpeedRatio()
    private bool isEngineRunning;    // engine loop active
    private bool isStarting;         // playing startup sound

    private void Awake()
    {
        carController = GetComponent<CarController>();

        if (!engineSource)
            engineSource = GetComponent<AudioSource>();

        engineSource.playOnAwake = false;
        engineSource.loop = true;
        engineSource.volume = 0f;
        engineSource.clip = null;
    }

    private void Update()
    {
        if (!carController || !engineSource)
            return;

        // Use your existing GetSpeedRatio (-1..1, negative for reverse)
        float value = carController.GetSpeedRatio();
        float sign = Mathf.Sign(value);
        speedRatio = Mathf.Clamp01(Mathf.Abs(value));   // 0..1

        // While starting or stopped, don't touch pitch/volume here
        if (!isEngineRunning || isStarting)
            return;

        // Decide which clip should be playing (forward vs reverse)
        AudioClip desiredClip = null;

        if (sign >= 0f)
        {
            // Forward
            desiredClip = drivingClip ? drivingClip : idleClip;
        }
        else
        {
            // Reverse (fallback to drivingClip if reverseClip is not set)
            desiredClip = reverseClip ? reverseClip : (drivingClip ? drivingClip : idleClip);
        }

        // If the wrong clip is playing, switch to the right one
        if (desiredClip && engineSource.clip != desiredClip)
        {
            engineSource.clip = desiredClip;
            engineSource.loop = true;
            engineSource.Play();
        }

        // Change pitch/volume with speedRatio
        engineSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
        engineSource.volume = Mathf.Lerp(minVolume, maxVolume, speedRatio);
    }

    // ----------------------------------------------------------------
    // Called from CarController when you press gas for the first time
    // OR from your pause menu when resuming.
    // ----------------------------------------------------------------
    public IEnumerator StartEngine()
    {
        if (isStarting) yield break; // already cranking

        isStarting = true;
        isEngineRunning = false;
        carController.isEngineRunning = 1;

        if (engineSource && startupClip)
        {
            // play startup full volume, unaffected by Update logic
            engineSource.Stop();
            engineSource.clip = startupClip;
            engineSource.loop = false;
            engineSource.volume = 1f;
            engineSource.pitch = 1f;
            engineSource.Play();

            yield return new WaitForSeconds(startupClip.length);
        }
        else
        {
            // fallback: small delay
            yield return new WaitForSeconds(0.3f);
        }

        // Switch to idle/driving loop
        if (engineSource)
        {
            engineSource.clip = idleClip ? idleClip : drivingClip;
            engineSource.loop = true;
            engineSource.volume = minVolume;
            engineSource.pitch = minPitch;
            engineSource.Play();
        }

        isStarting = false;
        isEngineRunning = true;
        carController.isEngineRunning = 2;
    }

    // ----------------------------------------------------------------
    // PAUSE: fade out and stop engine
    // ----------------------------------------------------------------
    public IEnumerator FadeOutAndStop(float duration)
    {
        if (!engineSource)
            yield break;

        float startVolume = engineSource.volume;
        float t = 0f;

        isEngineRunning = false; // Update() will stop touching pitch/volume

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;   // so it still works when Time.timeScale = 0
            float k = Mathf.Clamp01(t / duration);
            engineSource.volume = Mathf.Lerp(startVolume, 0f, k);
            yield return null;
        }

        engineSource.volume = 0f;
        engineSource.Stop();

        isStarting = false;
        carController.isEngineRunning = 0;
    }

    // ----------------------------------------------------------------
    // Immediate stop (no fade) – if you ever need it
    // ----------------------------------------------------------------
    public void StopImmediate()
    {
        isEngineRunning = false;
        isStarting = false;

        if (engineSource)
        {
            engineSource.Stop();
            engineSource.volume = 0f;
        }

        carController.isEngineRunning = 0;
    }
}





/*using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(AudioSource))]
public class EngineAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource engineSource;   // single source for engine

    [Header("Engine Clips")]
    [SerializeField] private AudioClip idleClip;        // e.g. idle or low_on
    [SerializeField] private AudioClip drivingClip;     // e.g. med_on / high_on
    [SerializeField] private AudioClip reverseClip;     // optional, can reuse drivingClip
    [SerializeField] private AudioClip startupClip;     // startup

    [Header("Pitch & Volume")]
    [SerializeField] private float minPitch = 0.8f;     // at idle
    [SerializeField] private float maxPitch = 2.0f;     // at max speed/RPM
    [SerializeField] private float minVolume = 0.4f;    // idle volume
    [SerializeField] private float maxVolume = 1.0f;    // full-throttle volume

    [Header("Runtime")]
    public bool isEngineRunning;

    private CarController carController;
    private float speedRatio;   // 0..1 based on GetSpeedRatio()

    private void Awake()
    {
        carController = GetComponent<CarController>();

        if (!engineSource)
            engineSource = GetComponent<AudioSource>();

        engineSource.loop = true;
        engineSource.playOnAwake = false;
        engineSource.volume = 0f;
    }

    private void Update()
    {
        if (!carController || !engineSource)
            return;

        // Use your existing GetSpeedRatio() (can be negative for reverse)
        float value = carController.GetSpeedRatio();
        float sign = Mathf.Sign(value);
        speedRatio = Mathf.Clamp01(Mathf.Abs(value));   // 0..1

        if (!isEngineRunning)
        {
            engineSource.volume = 0f;
            return;
        }

        // Decide which clip should be playing (forward vs reverse)
        AudioClip desiredClip = null;

        if (sign >= 0f)
        {
            // Forward
            desiredClip = drivingClip ? drivingClip : idleClip;
        }
        else
        {
            // Reverse (fallback to drivingClip if reverseClip is not set)
            desiredClip = reverseClip ? reverseClip : drivingClip;
        }

        // If the wrong clip is playing, switch to the right one
        if (desiredClip && engineSource.clip != desiredClip)
        {
            engineSource.clip = desiredClip;
            engineSource.Play();
        }

        // Change pitch/volume with speedRatio
        engineSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
        engineSource.volume = Mathf.Lerp(minVolume, maxVolume, speedRatio);
    }

    // Called from CarController when you first press gas
    public IEnumerator StartEngine()
    {
        // mark engine cranking
        carController.isEngineRunning = 1;

        // play startup one-shot if we have it
        if (startupClip && engineSource)
        {
            engineSource.PlayOneShot(startupClip);
            yield return new WaitForSeconds(Mathf.Min(1.0f, startupClip.length * 0.8f));
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }

        // start main loop with idle (or driving if idle missing)
        if (engineSource)
        {
            engineSource.clip = idleClip ? idleClip : drivingClip;
            engineSource.loop = true;
            engineSource.Play();
        }

        isEngineRunning = true;
        yield return new WaitForSeconds(0.4f);

        carController.isEngineRunning = 2;
    }
}


*/











/*using System.Collections;
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
*/