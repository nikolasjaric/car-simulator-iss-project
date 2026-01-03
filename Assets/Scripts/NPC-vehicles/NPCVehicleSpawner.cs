using System.Collections;
using UnityEngine;

public class NPCVehicleSpawner : MonoBehaviour
{
    [Header("NPC Prefabs")]
    public GameObject[] npcPrefabs;

    [Header("Path Settings")]
    public WaypointPath defaultPath;      // Putanja kojom NPC vozila voze

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int maxNPCs = 10;
    public bool spawnOnStart = true;

    [Header("Despawn")]
    public float npcLifetime = 30f; // 30 sekundi

    private int currentNPCs = 0;
    private Transform player;

    void Start()
    {
        Debug.Log("NPC Spawner started! Prefabs: " + npcPrefabs.Length);

        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Pretpostavljaš Player tag
        if (spawnOnStart)
            StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (currentNPCs < maxNPCs)
            {
                SpawnNPCOnPath();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnNPCOnPath()
    {
        if (npcPrefabs == null || npcPrefabs.Length == 0) return;
        if (defaultPath == null || defaultPath.waypoints == null || defaultPath.waypoints.Length == 0) return;

        // odaberi random prefab
        GameObject randomPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        // SPAVN NA POČETAK PUTANJE
        Transform startWp = defaultPath.waypoints[0];
        Vector3 spawnPos = startWp.position;
        Quaternion spawnRot = startWp.rotation;

        GameObject npc = Instantiate(randomPrefab, spawnPos, spawnRot);

        // spoji path na NPC mover skriptu
        var mover = npc.GetComponent<NPCVehicleMover>();
        if (mover != null)
        {
            mover.path = defaultPath;
            // mover.speed = ...  // po želji možeš ovdje randomizirati brzinu
        }

        // despawn nakon npcLifetime sekundi
        Destroy(npc, npcLifetime);
        currentNPCs++;
    }

    public void RemoveNPC() => currentNPCs--;
}
