using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("The prefab to spawn.")]
    [SerializeField] private GameObject prefab;

    [Tooltip("The Transform from which to base spawn positions.")]
    [SerializeField] private Transform spawnOrigin;

    [Header("Offset Settings")]
    [Tooltip("How far (units) from spawnOrigin on the X axis.")]
    [SerializeField] private float offsetMagnitude = 0.5f;

    [Header("Timing")]
    [Tooltip("Minimum delay (seconds) between spawns.")]
    [SerializeField] private float minSpawnInterval = 0.5f;
    [Tooltip("Maximum delay (seconds) between spawns.")]
    [SerializeField] private float maxSpawnInterval = 1.5f;

    private float timer;
    private float nextSpawnTime;

    private void Start()
    {
        ScheduleNextSpawn();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextSpawnTime)
        {
            SpawnOne();
            ScheduleNextSpawn();
        }
    }

    private void ScheduleNextSpawn()
    {
        timer = 0f;
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void SpawnOne()
    {
        if (prefab == null || spawnOrigin == null)
        {
            Debug.LogWarning("ObjectSpawner: prefab or spawnOrigin is not assigned.");
            return;
        }

        // Randomly choose left (-1) or right (+1)
        float sign = Random.value < 0.5f ? -1f : 1f;
        Vector3 spawnPos = spawnOrigin.position + Vector3.right * offsetMagnitude * sign;

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
