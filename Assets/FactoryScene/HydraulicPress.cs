using UnityEngine;

public class HydraulicPress : MonoBehaviour
{
    [Header("Head Motion")]
    [Tooltip("Transform of the head sprite.")]
    [SerializeField] private Transform headTransform;
    [Tooltip("Lowest world Y of the head.")]
    [SerializeField] private float bottomY = 0f;
    [Tooltip("Highest world Y of the head.")]
    [SerializeField] private float topY = 5f;
    [Tooltip("Speed at which the head moves (units/sec).")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Body Stretch")]
    [Tooltip("Transform of the body sprite (pivot at top).")]
    [SerializeField] private Transform bodyTransform;
    private float unitHeight;  // the sprite's local-space height
    private float anchorY;     // world Y of the body pivot

    [Header("Timing")]
    [Tooltip("Delay in seconds before the press starts moving.")]
    [SerializeField] private float startDelay = 1f;
    private float movementStartTime;

    [Header("Player Tags")]
    [Tooltip("Tag of Player One.")]
    [SerializeField] private string playerOneTag = "Player";
    [Tooltip("Tag of Player Two.")]
    [SerializeField] private string playerTwoTag = "PlayerTwo";

    void Start()
    {
        if (headTransform == null || bodyTransform == null)
        {
            Debug.LogError("HydraulicPress: Assign both headTransform and bodyTransform in the inspector!");
            enabled = false;
            return;
        }

        var sr = bodyTransform.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("HydraulicPress: Body Transform needs a SpriteRenderer!");
            enabled = false;
            return;
        }

        // compute the sprite's local height (before any scaling)
        unitHeight = sr.sprite.bounds.size.y;
        // pivot at top is our fixed anchor
        anchorY = bodyTransform.position.y;

        // schedule movement to start after delay
        movementStartTime = Time.time + startDelay;
    }

    void Update()
    {
        // haven't reached the start time yet?
        float elapsed = Time.time - movementStartTime;
        if (elapsed < 0f)
            return;

        // 1) Move the head: start at topY and move downward first
        float span = topY - bottomY;
        float t = Mathf.PingPong(elapsed * moveSpeed, span);
        float headY = topY - t;
        headTransform.position = new Vector3(
            headTransform.position.x,
            headY,
            headTransform.position.z
        );

        // 2) Stretch the body downward from its top pivot
        float desiredDistance = anchorY - headY;            // positive when head is below pivot
        float targetScaleY = Mathf.Max(0f, desiredDistance / unitHeight);

        Vector3 ls = bodyTransform.localScale;
        ls.y = targetScaleY;
        bodyTransform.localScale = ls;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerOneTag) || other.CompareTag(playerTwoTag))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.Respawn();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-5, bottomY, 0), new Vector3(5, bottomY, 0));
        Gizmos.DrawLine(new Vector3(-5, topY, 0), new Vector3(5, topY, 0));
    }
}
