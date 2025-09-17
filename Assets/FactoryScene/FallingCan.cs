using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FallingCan : MonoBehaviour
{
    [Header("Destroy Settings")]
    [Tooltip("If this can’s Y falls to or below this marker’s Y, it will be destroyed.")]
    [SerializeField] private Transform destroyMarker;
    [SerializeField] private Transform respawnMarker;

    [Header("Tags")]
    [Tooltip("Tag of Player One.")]
    [SerializeField] private string playerOneTag = "Player";
    [Tooltip("Tag of Player Two.")]
    [SerializeField] private string playerTwoTag = "PlayerTwo";

    [SerializeField] private string destroyMarkerTag = "Destroy";
    [SerializeField] private string respawnMarkerTag = "Respawn";

    private void Awake()
    {
        /*
        // Locate destroy marker by tag if not set in Inspector
        if (destroyMarker == null)
        {
            var dm = GameObject.FindWithTag(destroyMarkerTag);
            if (dm != null) destroyMarker = dm.transform;
            else Debug.LogError($"[FallingCan] No object found with tag '{destroyMarkerTag}' in scene.");
        }

        // Locate respawn marker by tag if not set in Inspector
        if (respawnMarker == null)
        {
            var rm = GameObject.FindWithTag(respawnMarkerTag);
            if (rm != null) respawnMarker = rm.transform;
            else Debug.LogError($"[FallingCan] No object found with tag '{respawnMarkerTag}' in scene.");
        }
        */
    }

    private void Update()
    {
        if (destroyMarker != null && transform.position.y <= destroyMarker.position.y)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerTwo"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.Respawn();
                Debug.Log($"Respawned {other.tag} via teleporter or spawn");
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
