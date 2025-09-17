using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AirCurrent : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string playerOneTag = "Player";
    [SerializeField] private string playerTwoTag = "PlayerTwo";

    [Header("Current Settings")]
    [Tooltip("Direction the wind blows (e.g. (0,1) for updraft, (1,0) for rightward).")]
    [SerializeField] private Vector2 currentDirection = Vector2.up;
    [SerializeField] private float initialSpeed = 1f;
    [SerializeField] private float acceleration = 0.2f;
    [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private float wingsSpeedMultiplier = 1.5f;

    private Vector2 dirNorm;
    private Dictionary<Rigidbody2D, float> speedMap = new Dictionary<Rigidbody2D, float>();

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Awake()
    {
        // Normalize once for performance
        dirNorm = currentDirection.normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerOneTag) && !other.CompareTag(playerTwoTag)) return;
        var rb = other.attachedRigidbody;
        if (rb == null) return;

        // start them at the initial wind speed
        speedMap[rb] = initialSpeed;
        other.GetComponent<PlayerController>()?.SetInAirCurrent(true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (rb == null || !speedMap.ContainsKey(rb)) return;

        // ramp speed along current direction
        float cur = Mathf.Min(speedMap[rb] + acceleration * Time.deltaTime, maxSpeed);
        speedMap[rb] = cur;

        var pc = other.GetComponent<PlayerController>();
        bool wings = pc != null && pc.HasWingsuitSelected();

        if (wings)
        {
            // full 2D control, with wingsuit boost
            float h = pc.GetHorizontalInput();
            float v = pc.GetVerticalInput();
            Vector2 userDir = new Vector2(h, v);
            if (userDir.sqrMagnitude > 1f) userDir.Normalize();
            rb.velocity = userDir * cur * wingsSpeedMultiplier;
        }
        else
        {
            // base wind velocity
            Vector2 windVel = dirNorm * cur;

            // choose lateral axis based on whether the current is mostly vertical or horizontal
            Vector2 lateral = Vector2.zero;
            float lateralInput;
            float controlStrength = pc.GetLateralControl();

            if (Mathf.Abs(dirNorm.y) > Mathf.Abs(dirNorm.x))
            {
                // up/down draft → allow left/right steering
                lateralInput = pc.GetHorizontalInput();
                lateral = Vector2.right * lateralInput * controlStrength;
            }
            else
            {
                // left/right draft → allow up/down steering
                lateralInput = pc.GetVerticalInput();
                lateral = Vector2.up * lateralInput * controlStrength;
            }

            rb.velocity = windVel + lateral;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null) speedMap.Remove(rb);
        other.GetComponent<PlayerController>()?.SetInAirCurrent(false);
    }
}
