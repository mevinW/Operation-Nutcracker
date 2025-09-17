using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]  // so OnMouseDown can fire
public class BoxLimiter : MonoBehaviour
{
    [Header("Speed & Bounds Settings")]
    [Tooltip("Multiplier to dampen X-velocity each physics frame (0–1).")]
    [SerializeField, Range(0f, 1f)] private float xSpeedFactor = 0.5f;
    [Tooltip("Minimum X position allowed.")]
    [SerializeField] private float minX = -5f;
    [Tooltip("Maximum X position allowed.")]
    [SerializeField] private float maxX = 5f;

    private Rigidbody2D rb;
    private Vector3 originalPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Remember the spawn point
        originalPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // 1) Dampen horizontal velocity so box moves slower
        Vector2 v = rb.velocity;
        v.x *= xSpeedFactor;
        rb.velocity = v;

        // 2) Clamp position to stay within [minX, maxX]
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        transform.position = p;

        // 3) If box is against a bound, zero out any velocity pushing further
        if (p.x <= minX && rb.velocity.x < 0f) 
            rb.velocity = new Vector2(0f, rb.velocity.y);
        if (p.x >= maxX && rb.velocity.x > 0f) 
            rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    // Called when this object is clicked
    private void OnMouseDown()
    {
        // Reset position and stop all movement
        transform.position = originalPosition;
        rb.velocity = Vector2.zero;
    }
}
