using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight;
    public float rotationSpeed = 10f;

    void Update()
    {
        // Rotate the light around the X axis (sun movement)
        directionalLight.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}
