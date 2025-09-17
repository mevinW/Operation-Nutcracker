using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DummyAcorn : MonoBehaviour
{
    [Header("Who Placed This Acorn")]
    [Tooltip("Player index who placed this acorn (0 = P1, 1 = P2)")]
    public int placer = 0;

    [Header("Stun Settings")]
    [Tooltip("How many seconds to stun the opposing player")]
    public float stunDuration = 3f;

    private void Awake()
    {
        // Ensure we have a trigger collider
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only care about PlayerController collisions
        var pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        // If it's the opposing player, stun them
        if (pc.getIndex() != placer)
        {
            StartCoroutine(StunAndDestroy(pc));
        }
    }

    private IEnumerator StunAndDestroy(PlayerController pc)
    {
        // Disable their movement & input
        pc.enabled = false;

        // Optionally, fire an animation trigger:
        // if (pc.animator != null) pc.animator.SetTrigger("Stunned");

        // Grab their Rigidbody2D
        var rb = pc.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Kill all motion immediately
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Wait the configured duration
        yield return new WaitForSeconds(stunDuration);

        // Re‑enable movement
        if (pc != null)
            pc.enabled = true;

        // Clean up the acorn
        Destroy(gameObject);
    }
}
