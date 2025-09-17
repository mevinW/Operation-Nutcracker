using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(SpriteRenderer))]
public class SinusoidalMovingPlatform : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [Tooltip("Angular frequency of the horizontal sine oscillation (radians/sec).")]
    [SerializeField] private float horizontalSpeed = 3f;
    [Tooltip("Maximum horizontal distance from start position.")]
    [SerializeField] private float travelDistance = 5f;
    [Tooltip("Start moving to the right? If false, moves to the left.")]
    [SerializeField] private bool startMovingRight = true;

    [Header("Vertical Wave")]
    [Tooltip("Vertical bobbing amplitude.")]
    [SerializeField] private float verticalAmplitude = 2f;
    [Tooltip("Vertical wave frequency (cycles per second).")]
    [SerializeField] private float verticalFrequency = 1f;

    [Header("Delay & Sprite")]
    [Tooltip("Time in seconds to wait before the platform begins moving.")]
    [SerializeField] private float initialDelay = 0f;
    [SerializeField] private bool flipSpriteWithMovement = true;

    [Header("Aggression")]
    [SerializeField] private bool isAggressive = false;

    // runtime state
    private SpriteRenderer _spriteRenderer;
    private Vector3 _startPos;
    private float _startTime;
    private float _previousX;
    private bool _started = false;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _startPos = transform.position;
        _previousX = _startPos.x;
        _startTime = Time.time + initialDelay;

        if (initialDelay > 0f)
            StartCoroutine(DelayThenStart());
        else
            _started = true;
    }

    private IEnumerator DelayThenStart()
    {
        yield return new WaitForSeconds(initialDelay);
        _started = true;
    }

    private void Update()
    {
        if (!_started) return;

        float t = Time.time - _startTime;

        // —— HORIZONTAL: one‑sided sine easing between 0 and travelDistance —— 
        float rawSinH = Mathf.Sin(t * horizontalSpeed);
        float normH = (rawSinH + 1f) * 0.5f;
        float xOffset = normH * travelDistance;
        if (!startMovingRight) xOffset = -xOffset;
        float newX = _startPos.x + xOffset;

        // —— VERTICAL: classic sine wave around startY ——
        float newY = _startPos.y
                         + Mathf.Sin(t * verticalFrequency * 2f * Mathf.PI)
                         * verticalAmplitude;

        transform.position = new Vector3(newX, newY, _startPos.z);

        // —— Sprite flipping based on horizontal motion ——
        float deltaX = newX - _previousX;
        bool movingRight = deltaX > 0f;
        if (flipSpriteWithMovement)
            _spriteRenderer.flipX = !movingRight;
        else
            _spriteRenderer.flipX = movingRight;
        _previousX = newX;
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
                Debug.Log($"Respawned {other.tag}");
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
        Vector3 start = Application.isPlaying ? _startPos : transform.position;
        Vector3 endH = start + Vector3.right * (startMovingRight ? travelDistance : -travelDistance);
        Gizmos.DrawLine(start, endH);
        Gizmos.DrawSphere(endH, 0.1f);

        Gizmos.color = Color.yellow;
        Vector3 top = start + Vector3.up * verticalAmplitude;
        Vector3 bottom = start + Vector3.down * verticalAmplitude;
        Gizmos.DrawLine(top, bottom);
        Gizmos.DrawSphere(top, 0.1f);
        Gizmos.DrawSphere(bottom, 0.1f);
    }
}
