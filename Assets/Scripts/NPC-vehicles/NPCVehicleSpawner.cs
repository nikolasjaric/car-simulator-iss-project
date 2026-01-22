using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCPathEntry
{
    public GameObject prefab;    // prefab vozila
    public WaypointPath path;    // njegova putanja
}

public class NPCVehicleSpawner : MonoBehaviour
{
    [Header("NPC Prefabs & Paths")]
    public List<NPCPathEntry> npcEntries = new List<NPCPathEntry>();

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int maxNPCs = 14;          // ukupni max broj vozila u sceni
    public int maxPerPrefab = 2;      // MAX broj istih vozila (po prefabu)
    public bool spawnOnStart = true;

    [Header("Despawn")]
    public float npcLifetime = 30f;

    private int currentNPCs = 0;

    // broj aktivnih primjeraka po prefabu (0..maxPerPrefab)
    private Dictionary<GameObject, int> activePerPrefab = new Dictionary<GameObject, int>();

    void Start()
    {
        if (spawnOnStart)
            StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // ne pređi maxNPCs
            if (currentNPCs < maxNPCs)
            {
                SpawnNPCWithOwnPath();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnNPCWithOwnPath()
    {
        if (npcEntries == null || npcEntries.Count == 0) return;

        // 1) lista entryja za koje još nismo dosegli maxPerPrefab
        List<NPCPathEntry> available = new List<NPCPathEntry>();

        foreach (var entry in npcEntries)
        {
            if (entry.prefab == null || entry.path == null || entry.path.waypoints.Length == 0)
                continue;

            int count = 0;
            activePerPrefab.TryGetValue(entry.prefab, out count);

            // sada dopuštamo 0..maxPerPrefab-1 aktivnih instanci
            if (count < maxPerPrefab)
            {
                available.Add(entry);
            }
        }

        if (available.Count == 0) return;

        // 2) nasumično odaberi jedan od slobodnih tipova
        NPCPathEntry selected = available[Random.Range(0, available.Count)];
        Transform startWp = selected.path.waypoints[0];

        // koristi točno poziciju waypointa
        Vector3 spawnPos = startWp.position;
        Quaternion spawnRot = startWp.rotation;

        GameObject npc = Instantiate(selected.prefab, spawnPos, spawnRot);

        // Poveži mover s putanjom i spawnerom
        var mover = npc.GetComponent<NPCVehicleMover>();
        if (mover != null)
        {
            mover.path = selected.path;
            mover.Spawner = this;
            mover.PrefabKey = selected.prefab;
        }

        if (!activePerPrefab.ContainsKey(selected.prefab))
            activePerPrefab[selected.prefab] = 0;

        activePerPrefab[selected.prefab]++;
        currentNPCs++;

        // opcionalno: ako još koristiš lifetime despawn
        if (npcLifetime > 0f)
            StartCoroutine(DespawnAfterTime(npc, selected.prefab));
    }

    IEnumerator DespawnAfterTime(GameObject npc, GameObject prefabKey)
    {
        yield return new WaitForSeconds(npcLifetime);

        if (npc != null)
        {
            Destroy(npc);
            OnNPCDestroyed(prefabKey);
        }
    }

    // Poziva se kad se NPC uništi (bilo vremenski, bilo na kraju putanje)
    public void OnNPCDestroyed(GameObject prefabKey)
    {
        currentNPCs = Mathf.Max(0, currentNPCs - 1);

        if (prefabKey != null && activePerPrefab.ContainsKey(prefabKey))
        {
            activePerPrefab[prefabKey] = Mathf.Max(0, activePerPrefab[prefabKey] - 1);
        }
    }

    // Ako negdje već pozivaš ovo, proslijedi i prefab
    public void RemoveNPC(GameObject prefabKey)
    {
        OnNPCDestroyed(prefabKey);
    }
}