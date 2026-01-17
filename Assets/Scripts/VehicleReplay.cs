using UnityEngine;

[DisallowMultipleComponent]
public class VehicleReplay : MonoBehaviour
{
    [HideInInspector]
    public Transform vehicleTransform;

    private void Awake()
    {
        // automatski referencira svoj transform
        vehicleTransform = transform;
    }
}
