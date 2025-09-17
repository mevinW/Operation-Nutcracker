using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ScrewProjectile : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("Transform to which players are returned on hit.")]
    [SerializeField] private Transform respawnMarker;
    [Tooltip("Tag used to locate the respawn marker if not assigned.")]
    [SerializeField] private string respawnMarkerTag = "Respawn";

    [Header("Player Tags")]
    [Tooltip("Tag of Player One.")]
    [SerializeField] private string playerOneTag = "Player";
    [Tooltip("Tag of Player Two.")]
    [SerializeField] private string playerTwoTag = "PlayerTwo";

    private void Awake()
    {
        
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
                Destroy(gameObject);
            }
        }
        if (!other.CompareTag("Air") && !other.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }
}