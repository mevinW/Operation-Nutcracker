using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class NPCPatrol : MonoBehaviour
{
    [Header("Patrol Boundaries")]
    [Tooltip("Empty GameObject marking the left boundary")]
    public Transform leftBoundary;
    [Tooltip("Empty GameObject marking the right boundary")]
    public Transform rightBoundary;

    [Header("Movement Settings")]
    [Tooltip("Patrol speed in units per second")]
    public float speed = 2f;

    // patrol direction
    private int direction = -1;

    // references & start positions for both players
    private Transform playerOneTransform;
    private Vector3 playerOneStartPos;
    private Transform playerTwoTransform;
    private Vector3 playerTwoStartPos;

    void Start()
    {
        // Cache Player One
        var p1 = GameObject.FindGameObjectWithTag("Player");
        if (p1 != null)
        {
            playerOneTransform = p1.transform;
            playerOneStartPos = p1.transform.position;
        }
        else
        {
            Debug.LogWarning("NPCPatrol: No GameObject tagged 'Player' found.");
        }

        // Cache Player Two
        var p2 = GameObject.FindGameObjectWithTag("PlayerTwo");
        if (p2 != null)
        {
            playerTwoTransform = p2.transform;
            playerTwoStartPos = p2.transform.position;
        }
        else
        {
            Debug.LogWarning("NPCPatrol: No GameObject tagged 'PlayerTwo' found.");
        }

        // Make this NPC kinematic so we drive its motion manually
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        // move NPC back and forth
        float step = speed * Time.deltaTime * direction;
        transform.Translate(step, 0f, 0f);

        // flip at boundaries
        if (transform.position.x <= leftBoundary.position.x && direction == -1)
        {
            direction = 1;
            FlipSprite();
        }
        else if (transform.position.x >= rightBoundary.position.x && direction == 1)
        {
            direction = -1;
            FlipSprite();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerTwo"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                if (!pc.isInvisible)
                {
                    pc.Respawn();
                    Debug.Log($"Respawned {other.tag} via teleporter or spawn");
                }
            }
        }
    }

    /// <summary>
    /// Puts the given player Transform back at its start, and zeroes its Rigidbody2D velocity.
    /// </summary>
    private void ResetPlayer(Transform playerT, Vector3 startPos, Collider2D otherCollider)
    {
        Debug.Log($"Reset {otherCollider.tag}");
        playerT.position = startPos;
        var rb = otherCollider.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    private void FlipSprite()
    {
        var scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    public IEnumerator Stun(float duration)
    {
        enabled = false;
        yield return new WaitForSeconds(duration);
        enabled = true;
    }
}
