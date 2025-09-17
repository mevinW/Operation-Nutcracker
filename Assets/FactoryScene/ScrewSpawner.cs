// ScrewSpawner.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ScrewSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab of the screw projectile (must have Rigidbody2D + Collider2D)")]
    [SerializeField] private GameObject screwPrefab;
    [Tooltip("Point from which to launch screws")]
    [SerializeField] private Transform spawnPoint;

    [Header("Timing")]
    [Tooltip("Minimum time between shots")]
    [SerializeField] private float minInterval = 1f;
    [Tooltip("Maximum time between shots")]
    [SerializeField] private float maxInterval = 1.5f;

    [Header("Flight")]
    [Tooltip("Initial speed of the screw")]
    [SerializeField] private float launchSpeed = 5f;

    [Header("Rotation")]
    [Tooltip("Angle to apply when spawning")]
    [SerializeField] private float rotationZ = -90f;

    private void Start()
    {
        if (screwPrefab == null || spawnPoint == null)
        {
            Debug.LogError("ScrewSpawner: Assign both screwPrefab and spawnPoint in the Inspector.");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            // Instantiate and launch with configurable rotation
            GameObject screw = Instantiate(
                screwPrefab,
                spawnPoint.position,
                Quaternion.Euler(0f, 0f, rotationZ)
            );

            Rigidbody2D rb = screw.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.velocity = spawnPoint.right * launchSpeed;
            }
        }
    }
}