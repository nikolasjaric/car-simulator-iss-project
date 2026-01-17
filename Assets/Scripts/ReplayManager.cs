using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // za novu tipkovnicu

public class ReplayManager : MonoBehaviour
{
    [System.Serializable]
    public class VehicleReplay
    {
        public Transform vehicleTransform;
        public List<Vector3> positions = new List<Vector3>();
        public List<Quaternion> rotations = new List<Quaternion>();
    }

    [Header("Sva vozila za replay")]
    public List<VehicleReplay> vehicles = new List<VehicleReplay>();

    [Header("Podešavanja")]
    public float recordInterval = 0.02f;

    private bool isReplaying = false;
    private float timer = 0f;

    void Update()
    {
        HandleReplayToggle(); // provjerava tipku P u Update
    }

    void FixedUpdate()
    {
        if (isReplaying)
            PlayReplay();
        else
            RecordReplay();
    }

    void HandleReplayToggle()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Debug.Log("Pritisnuto P za toggle replay");

            if (isReplaying)
                StopReplay();
            else
                StartReplay();
        }
    }

    void RecordReplay()
    {
        timer += Time.fixedDeltaTime;
        if (timer < recordInterval) return;

        foreach (var v in vehicles)
        {
            v.positions.Add(v.vehicleTransform.position);
            v.rotations.Add(v.vehicleTransform.rotation);
        }

        timer = 0f;
    }

    void PlayReplay()
    {
        foreach (var v in vehicles)
        {
            if (v.positions.Count > 0)
            {
                v.vehicleTransform.position = v.positions[0];
                v.vehicleTransform.rotation = v.rotations[0];

                v.positions.RemoveAt(0);
                v.rotations.RemoveAt(0);
            }
        }
    }

    public void StartReplay()
    {
        isReplaying = true;

        foreach (var v in vehicles)
        {
            Rigidbody rb = v.vehicleTransform.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true; // blokira fiziku za replay
        }

        Debug.Log("Replay started");
    }

    public void StopReplay()
    {
        isReplaying = false;

        foreach (var v in vehicles)
        {
            Rigidbody rb = v.vehicleTransform.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false; // vraća fiziku

            // opcionalno: resetiranje snimljenih podataka
            v.positions.Clear();
            v.rotations.Clear();
        }

        Debug.Log("Replay stopped");
    }
}
