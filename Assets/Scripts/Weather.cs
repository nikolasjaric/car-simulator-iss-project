using UnityEngine;
using UnityEngine.InputSystem;

public class CarWeather : MonoBehaviour
{
    public ParticleSystem rain;

    public Color fogColor = Color.gray;
    public float fogDensity = 0.02f;

    void Start()
    {
        ClearWeather();
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SetRain();

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SetFog();

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SetRainAndFog();

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            ClearWeather(); // OFF
    }

    void SetRain()
    {
        rain.Clear(true);
        rain.Play();

        RenderSettings.fog = false;
    }

    void SetFog()
    {
        rain.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    void SetRainAndFog()
    {
        rain.Clear(true);
        rain.Play();

        RenderSettings.fog = true;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    void ClearWeather()
    {
        rain.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        RenderSettings.fog = false;
    }
}
