using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyPickup : MonoBehaviour
{
    [Header("Who can pick this up?")]
    [Tooltip("Tag of the player object.")]
    [SerializeField] private string playerTag = "Player";

    [Header("What to turn off on pickup")]
    [Tooltip("Assign the GameObject you want disabled when this key is collected.")]
    [SerializeField] private GameObject targetObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("PlayerTwo")) && targetObject != null)
        {
            // Disable the target (e.g. a door)
            targetObject.SetActive(false);

            // Optionally disable or destroy the key so it can't be picked up again
            gameObject.SetActive(false);
            // — or —
            // Destroy(gameObject);
        }
    }
}
