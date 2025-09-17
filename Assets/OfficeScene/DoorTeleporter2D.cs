using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorTeleporter2D : MonoBehaviour
{
    [Tooltip("Where to send the object after the delay")]
    public Vector2 teleportLocation;
    public string tag;

    [Tooltip("Delay in seconds before teleporting")]
    public float delay = 1f;

    void Awake()
    {
        // Make sure this collider is set as a trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tag))
        {
            // Start the teleport coroutine
            StartCoroutine(TeleportAfterDelay(other.transform));
        }
    }

    private IEnumerator TeleportAfterDelay(Transform target)
    {
        yield return new WaitForSeconds(delay);
        target.position = teleportLocation;
    }
}
