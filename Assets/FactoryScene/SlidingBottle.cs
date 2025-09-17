using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class SlidingBottle : MonoBehaviour
{
    [Header("Destroy Settings")]
    [Tooltip("If this bottle’s Y ≤ destroyMarker.Y it'll be destroyed.")]
    [SerializeField] private Transform destroyMarker;
    [SerializeField] private Transform respawnMarker;

    [Header("Player Reset Settings")]
    [Tooltip("Tag used to identify Player One.")]
    [SerializeField] private string playerOneTag = "Player";
    [Tooltip("Tag used to identify Player Two.")]
    [SerializeField] private string playerTwoTag = "PlayerTwo";

    [SerializeField] private string destroyMarkerTag = "DestroyBottle";
    [SerializeField] private string respawnMarkerTag = "Respawn";

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        /*
        // Auto‑locate destroy marker
        var dm = GameObject.FindWithTag(destroyMarkerTag);
        if (dm != null) destroyMarker = dm.transform;
        else Debug.LogError($"[SlidingBottle] No '{destroyMarkerTag}' found.");

        // Auto‑locate respawn marker
        var rm = GameObject.FindWithTag(respawnMarkerTag);
        if (rm != null) respawnMarker = rm.transform;
        else Debug.LogError($"[SlidingBottle] No '{respawnMarkerTag}' found.");
        */
    }

    private void FixedUpdate()
    {
        // Keep sliding left
        rb.velocity = new Vector2(-1f, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerTwo") || other.CompareTag("Belt"))
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
}
