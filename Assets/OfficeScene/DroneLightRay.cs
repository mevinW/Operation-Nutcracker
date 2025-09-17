using UnityEngine;

public class DroneLightRay : MonoBehaviour
{
    [Header("Pellet Settings")]
    [Tooltip("Prefab of the pellet to spawn (must have a Rigidbody2D).")]
    public GameObject pelletPrefab;
    [Tooltip("Local offset from the drone where pellets appear.")]
    public Vector2 spawnOffset = Vector2.zero;

    [Header("Spawn Timing")]
    [Tooltip("Seconds between each pellet spawn.")]
    public float spawnInterval = 1f;

    [Header("Initial Pellet Velocity")]
    [Tooltip("Horizontal speed applied to each spawned pellet.")]
    public float xVelocity = 0f;
    [Tooltip("Vertical speed applied to each spawned pellet (negative = down).")]
    public float yVelocity = -5f;

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            SpawnPellet();
            _timer = 0f;
        }
    }

    private void SpawnPellet()
    {
        if (pelletPrefab == null)
        {
            Debug.LogWarning("DronePelletSpawner: No pelletPrefab assigned.");
            return;
        }

        // Determine spawn position in world space
        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;

        // Instantiate and set velocity
        GameObject pellet = Instantiate(pelletPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(xVelocity, yVelocity);
        }
        else
        {
            Debug.LogWarning("DronePelletSpawner: Spawned pellet has no Rigidbody2D.");
        }
    }

    // Draw spawn point gizmo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 worldOffset = Application.isPlaying
            ? (Vector3)spawnOffset
            : transform.TransformPoint(spawnOffset) - transform.position;
        Gizmos.DrawSphere(transform.position + worldOffset, 0.1f);
    }
}
