using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreationSpawnEvent", menuName = "WorldEvents/CreationSpawn") ]
public class CreationSpawns : WorldEvent
{
    public GameObject zombiePrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 1f;
    public int maxZombies = 20;

    [Header("Map Settings")]
    public float mapRadius = 50f;
    public float raycastHeight = 50f;
    public LayerMask groundMask;

    private float timer;
    private bool active;

    private List<GameObject> spawnedZombies = new List<GameObject>();

    public override void StartEvent()
    {
        active = true;
        timer = 0f;
        spawnedZombies.Clear();

        EventUpdater.Instance.Register(UpdateEvent);
    }

    void UpdateEvent()
    {
        if (!active) return;

        if (spawnedZombies.Count >= maxZombies)
            return;

        timer += Time.deltaTime;
        if (timer >= 1f / spawnRate)
        {
            timer = 0f;
            TrySpawnZombie();
        }
    }

    void TrySpawnZombie()
    {
        float x = Random.Range(-mapRadius, mapRadius);
        float z = Random.Range(-mapRadius, mapRadius);

        Vector3 rayOrigin = new Vector3(x, raycastHeight, z);
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, groundMask))
        {
            GameObject zombie = Instantiate(
                zombiePrefab,
                hit.point,
                Quaternion.identity
            );

            spawnedZombies.Add(zombie);
        }
    }

    public override void EndEvent()
    {
        active = false;
        EventUpdater.Instance.Unregister(UpdateEvent);

        for (int i = spawnedZombies.Count - 1; i >= 0; i--)
        {
            if (spawnedZombies[i] != null)
                Destroy(spawnedZombies[i]);
        }

        spawnedZombies.Clear();
    }
}