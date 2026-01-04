using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCPathEntry
{
    public GameObject prefab;      // prefab vozila
    public WaypointPath path;      // njegova putanja
}

public class NPCVehicleSpawner : MonoBehaviour
{
    [Header("NPC Prefabs & Paths")]
    public List<NPCPathEntry> npcEntries = new List<NPCPathEntry>();

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int maxNPCs = 10;          // stavi barem koliko imaš npcEntries
    public bool spawnOnStart = true;

    [Header("Despawn")]
    public float npcLifetime = 30f;

    private int currentNPCs = 0;

    // broj aktivnih primjeraka po prefabu (0 ili 1)
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

        // 1) provjeri imamo li već jedan primjerak SVAKE vrste vozila
        int uniqueActive = 0;
        foreach (var entry in npcEntries)
        {
            if (entry.prefab == null) continue;

            int c = 0;
            activePerPrefab.TryGetValue(entry.prefab, out c);
            if (c > 0) uniqueActive++;
        }

        // ako već imamo sve tipove vozila u sceni, nema se što spawnati
        if (uniqueActive >= npcEntries.Count) return;

        // 2) napravi listu svih entryja koji NEMAJU aktivan primjerak
        List<NPCPathEntry> available = new List<NPCPathEntry>();

        foreach (var entry in npcEntries)
        {
            if (entry.prefab == null || entry.path == null || entry.path.waypoints.Length == 0)
                continue;

            int count = 0;
            activePerPrefab.TryGetValue(entry.prefab, out count);

            if (count == 0) // ovaj tip vozila nije trenutno u sceni
            {
                available.Add(entry);
            }
        }

        // ako nema slobodnih (možda su neki entryji neispravno podešeni), izađi
        if (available.Count == 0) return;

        // 3) nasumično odaberi jedan od slobodnih tipova
        NPCPathEntry selected = available[Random.Range(0, available.Count)];

        Transform startWp = selected.path.waypoints[0];
        Vector3 spawnPos = startWp.position;
        Quaternion spawnRot = startWp.rotation;

        GameObject npc = Instantiate(selected.prefab, spawnPos, spawnRot);

        var mover = npc.GetComponent<NPCVehicleMover>();
        if (mover != null)
        {
            mover.path = selected.path;
        }

        // zabilježi da je ovaj prefab aktivan
        if (!activePerPrefab.ContainsKey(selected.prefab))
            activePerPrefab[selected.prefab] = 0;
        activePerPrefab[selected.prefab]++;

        currentNPCs++;

        // despawn nakon lifetime-a i poništi brojače
        StartCoroutine(DespawnAfterTime(npc, selected.prefab));
    }

    IEnumerator DespawnAfterTime(GameObject npc, GameObject prefabKey)
    {
        yield return new WaitForSeconds(npcLifetime);

        if (npc != null)
            Destroy(npc);

        currentNPCs = Mathf.Max(0, currentNPCs - 1);

        if (activePerPrefab.ContainsKey(prefabKey))
        {
            activePerPrefab[prefabKey] = Mathf.Max(0, activePerPrefab[prefabKey] - 1);
        }
    }

    public void RemoveNPC()
    {
        currentNPCs = Mathf.Max(0, currentNPCs - 1);
    }
}
