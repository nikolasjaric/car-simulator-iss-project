using UnityEngine;

public class NPCVehicleMover : MonoBehaviour
{
    public WaypointPath path;      // referenca na putanju
    public float speed = 10f;      // konstanta brzina
    public bool loop = true;       // želiš li da se vraća na početak

    private int currentIndex = 0;

    void Start()
    {
        if (path == null || path.waypoints.Length == 0) return;

        // Postavi početnu poziciju na prvi waypoint
        transform.position = path.waypoints[0].position;
        currentIndex = 1; // kreće prema drugom
    }

    void Update()
    {
        if (path == null || path.waypoints.Length == 0) return;
        if (currentIndex >= path.waypoints.Length) return;

        Transform target = path.waypoints[currentIndex];
        Vector3 dir = (target.position - transform.position).normalized;

        // konstantna brzina
        transform.position += dir * speed * Time.deltaTime;

        // orijentacija auta u smjeru kretanja (po želji)
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);

        // jesmo li stigli dovoljno blizu waypointa
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 0.5f)
        {
            currentIndex++;

            if (currentIndex >= path.waypoints.Length)
            {
                if (loop)
                {
                    currentIndex = 0;
                }
                else
                {
                    // opcionalno: zaustavi auto ili ga uništi
                    enabled = false;
                }
            }
        }
    }
}
