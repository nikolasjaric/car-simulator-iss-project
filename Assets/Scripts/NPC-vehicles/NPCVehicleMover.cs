using UnityEngine;

public class NPCVehicleMover : MonoBehaviour
{
    public WaypointPath path;
    public float speed = 10f;
    public bool continueStraightAfterLast = true;

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
        else if (finishedPath && continueStraightAfterLast)
        {
            // nastavi ravno naprijed bez ikakvih waypointa
            transform.position += transform.forward * speed * Time.deltaTime;
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
                // završili smo putanju – zapamti smjer zadnjeg segmenta
                finishedPath = true;
                // orijentiraj se prema zadnjem segmentu
                Vector3 lastDir = (target.position - path.waypoints[currentIndex - 2].position).normalized;
                if (lastDir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(lastDir);
            }

            return;
        }

        Vector3 dir = toTarget.normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
