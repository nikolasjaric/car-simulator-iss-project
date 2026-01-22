using UnityEngine;

public class NPCVehicleMover : MonoBehaviour
{
    public WaypointPath path;
    public float speed = 10f;

    // NOVO: Traffic Avoidance
    [Header("Traffic Avoidance")]
    public float safeDistance = 50f;
    public LayerMask vehicleLayer = -1; // Default sve, ili "Vehicle" layer

    // više nam ne treba nastavak ravno nakon zadnjeg
    public bool continueStraightAfterLast = true; // možeš obrisati ako ga ne koristiš drugdje

    // reference za spawner
    public NPCVehicleSpawner Spawner;
    public GameObject PrefabKey;

    private int currentIndex = 0;
    private bool finishedPath = false;

    void Start()
    {
        if (path == null || path.waypoints.Length == 0) return;
        transform.position = path.waypoints[0].position;
        currentIndex = 1; // kreće prema drugom
    }

    void Update()
    {
        if (path == null || path.waypoints.Length == 0) return;

        if (!finishedPath && currentIndex < path.waypoints.Length)
        {
            MoveAlongPath();
        }
        else if (finishedPath)
        {
            // javi spawneru prije nego što se uništi
            if (Spawner != null)
            {
                Spawner.OnNPCDestroyed(PrefabKey);
            }
            Destroy(gameObject);
        }
    }

    void MoveAlongPath()
    {
        Transform target = path.waypoints[currentIndex];
        Vector3 toTarget = target.position - transform.position;

        // ako smo vrlo blizu ili smo preletjeli waypoint, skoči na sljedeći
        if (toTarget.sqrMagnitude < 0.5f * 0.5f)
        {
            currentIndex++;
            if (currentIndex >= path.waypoints.Length)
            {
                // završili smo putanju
                finishedPath = true;
                return;
            }
            target = path.waypoints[currentIndex];
            toTarget = target.position - transform.position;
        }

        Vector3 dir = toTarget.normalized;

        // NOVO: Provjeri vozilo ispred PRIJE kretanja
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, safeDistance, vehicleLayer))
        {
            // Provjeri je li vozilo (tag ili komponenta)
            if (hit.collider.CompareTag("Vehicle") || hit.collider.GetComponent<NPCVehicleMover>() != null ||
                hit.collider.GetComponent<CarController>() != null)
            {
                Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward * hit.distance, Color.red, 0.1f);

                // Smanji speed proporcionalno udaljenosti
                float slowdown = hit.distance / safeDistance; // 1=full speed, 0=stop
                speed = Mathf.Lerp(speed, originalSpeed * slowdown * 0.7f, Time.deltaTime * 2f);
                return; // NE kreći se ovaj frame
            }
        }
        else
        {
            // Nema prepreke, vrati punu brzinu
            speed = Mathf.Lerp(speed, originalSpeed, Time.deltaTime * 1f);
        }

        // Kretanje
        transform.position += dir * speed * Time.deltaTime;

        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    // Spreči prevelike promjene speeda
    private float originalSpeed;
    void Awake() { originalSpeed = speed; }
}
