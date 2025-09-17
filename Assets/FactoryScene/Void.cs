using UnityEngine;

public class Void : MonoBehaviour
{
    // References & start positions for both players
    private Transform playerOneTransform;
    private Vector3 playerOneStartPos;
    private Transform playerTwoTransform;
    private Vector3 playerTwoStartPos;

    void Start()
    {
        // Cache Player One
        GameObject p1 = GameObject.FindGameObjectWithTag("Player");
        if (p1 != null)
        {
            playerOneTransform = p1.transform;
            playerOneStartPos = p1.transform.position;
        }
        else
        {
            Debug.LogWarning("Void: No GameObject tagged 'Player' found.");
        }

        // Cache Player Two
        GameObject p2 = GameObject.FindGameObjectWithTag("PlayerTwo");
        if (p2 != null)
        {
            playerTwoTransform = p2.transform;
            playerTwoStartPos = p2.transform.position;
        }
        else
        {
            Debug.LogWarning("Void: No GameObject tagged 'PlayerTwo' found.");
        }
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
        }
    }

    /// <summary>
    /// Resets the given player transform to its start position and zeroes its velocity.
    /// </summary>
    private void ResetPlayer(Transform t, Vector3 startPos, Collision2D collision)
    {
        Debug.Log($"Reset {collision.gameObject.tag}");
        t.position = startPos;
        var rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;
    }
}
