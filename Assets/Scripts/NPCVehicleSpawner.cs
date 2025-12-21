using System.Collections;
using UnityEngine;

public class NPCVehicleSpawner : MonoBehaviour
{
    [Header("NPC Prefabs")]
    public GameObject[] npcPrefabs;

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int maxNPCs = 10;
    public bool spawnOnStart = true;
    
    [Header("Random Spawn Area")]
    public Vector2 spawnAreaSize = new Vector2(180f, 120f); // X,Z granice spawn područja
    public float spawnHeight = 20f; // Visina za raycast
    public LayerMask groundLayer = 1; // Layer za tlo (default = sve)
    public float minDistanceFromPlayer = 25f; // Min distanca od igrača

    [Header("Despawn")]
    public float npcLifetime = 30f;     // 30 sekundi

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
                Vector3 spawnPos = GetRandomSpawnPosition();
                if (spawnPos != Vector3.zero)
                {
                    GameObject randomPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
                    GameObject npc = Instantiate(randomPrefab, spawnPos, Quaternion.identity);

                    // despawn nakon npcLifetime sekundi
                    Destroy(npc, npcLifetime);
                    currentNPCs++;
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Nasumična pozicija u spawn području (relativno od spawnera)
        Vector3 randomPos = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            spawnHeight,
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
        ) + transform.position;

        // Provjeri da li je dovoljno daleko od playera
        if (player != null && Vector3.Distance(randomPos, player.position) < minDistanceFromPlayer)
            return Vector3.zero;

        // Raycast dolje da nađeš tlo
        if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, spawnHeight * 2f, groundLayer))
        {
            return hit.point; // Vrati točku na tlu
        }

        return Vector3.zero; // Nevaljana pozicija
    }

    public void RemoveNPC() => currentNPCs--;
}