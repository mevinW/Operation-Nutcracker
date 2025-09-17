using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(SpriteRenderer))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Speed of the platform in units/second.")]
    [SerializeField] private float speed = 3f;
    [Tooltip("Distance (in world units) to travel before reversing.")]
    [SerializeField] private float travelDistance = 5f;
    [Tooltip("Start by moving right? If false, starts moving left.")]
    [SerializeField] private bool startMovingRight = true;
    [Tooltip("Time in seconds to wait before the platform begins moving.")]
    [SerializeField] private float initialDelay = 0f;

    [Header("Adjustment Settings")]
    [Tooltip("If true, speed is fastest at midpoint and slowest near endpoints.")]
    [SerializeField] private bool isAdjusted = false;
    [Tooltip("Minimum fraction of 'speed' that the platform will move at endpoints (0–1).")]
    [Range(0f, 1f)]
    [SerializeField] private float minSpeedFactor = 0.1f;

    [Header("Sprite Flip Settings")]
    [Tooltip("If true, the sprite will face the movement direction; if false it'll face opposite.")]
    [SerializeField] private bool flipSpriteWithMovement = true;

    [Header("Aggression Settings")]
    [Tooltip("If true, platform will perform aggressive actions on collision.")]
    [SerializeField] private bool isAggressive = false;

    // Components
    private SpriteRenderer _spriteRenderer;

    // Tracking variables
    private Vector3 _startPosition;
    private bool _movingRight;
    private float _distanceTraveled;
    [SerializeField] private float adjustmentExponent = 1f;

    // Has the delay elapsed?
    private bool _hasStarted = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _startPosition = transform.position;
        _movingRight = startMovingRight;
        _distanceTraveled = 0f;

        UpdateSpriteFlip();

        if (initialDelay > 0f)
            StartCoroutine(DelayBeforeStart());
        else
            _hasStarted = true;
    }

    private IEnumerator DelayBeforeStart()
    {
        yield return new WaitForSeconds(initialDelay);
        _hasStarted = true;
    }

    private void Update()
    {
        if (!_hasStarted) return;

        // Base direction: right = +1, left = -1
        float dir = _movingRight ? 1f : -1f;

        // Determine speed for this frame
        float currentSpeed = speed;
        if (isAdjusted)
        {
            float t = Mathf.Clamp01(_distanceTraveled / travelDistance);
            // base sine
            float rawFactor = Mathf.Sin(t * Mathf.PI);
            // remap via exponent
            float remapped = Mathf.Pow(rawFactor, adjustmentExponent);
            // lerp between minSpeedFactor and 1
            float speedFactor = Mathf.Lerp(minSpeedFactor, 1f, remapped);
            currentSpeed *= speedFactor;
        }

        // Move and accumulate
        float delta = currentSpeed * Time.deltaTime * dir;
        transform.Translate(Vector2.right * delta);
        _distanceTraveled += Mathf.Abs(delta);

        // Reverse when reaching the limit
        if (_distanceTraveled >= travelDistance)
        {
            _movingRight = !_movingRight;
            _distanceTraveled = 0f;
            UpdateSpriteFlip();
        }
    }

    private void UpdateSpriteFlip()
    {
        _spriteRenderer.flipX = flipSpriteWithMovement
            ? !_movingRight
            : _movingRight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAggressive) return;
        if (other.CompareTag("Player") || other.CompareTag("PlayerTwo"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null && !pc.isInvisible)
            {
                pc.Respawn();
                Debug.Log($"Respawned {other.tag} via teleporter or spawn");
            }
        }
    }

    public IEnumerator Stun(float duration)
    {
        enabled = false;
        yield return new WaitForSeconds(duration);
        enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 from = Application.isPlaying ? _startPosition : transform.position;
        Vector3 rightEnd = from + Vector3.right * travelDistance;
        Vector3 leftEnd = from + Vector3.left * travelDistance;
        Gizmos.DrawLine(leftEnd, rightEnd);
        Gizmos.DrawSphere(rightEnd, 0.1f);
        Gizmos.DrawSphere(leftEnd, 0.1f);
    }
}
