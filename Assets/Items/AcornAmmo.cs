using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AcornAmmo : MonoBehaviour
{
    [Header("Stun Settings")]
    [Tooltip("How long (in seconds) to stun an NPC")]
    [SerializeField] private float stunDuration = 3f;

    private void Awake()
    {
        // Ensure we can trigger collisions
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1) Hit an NPC?
        if (other.CompareTag("NPC"))
        {
            Debug.Log("Hit NPC");
            var npc = other.GetComponent<NPCPatrol>();
            var npcAlt = other.GetComponent<MovingPlatform>();
            if (npc != null)
                npc.StartCoroutine(npc.Stun(stunDuration));
            if (npcAlt != null)
                npcAlt.StartCoroutine(npcAlt.Stun(stunDuration));

            Destroy(gameObject);
            return;
        }

        // 2) Hit a player?
        if (other.CompareTag("Player") || other.CompareTag("PlayerTwo"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.Respawn();  // your respawn code goes here

            Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Air") && !other.CompareTag("Acorn") && !other.CompareTag("Climb")) {
            Destroy(gameObject);
        }
    }

    /*
    private IEnumerator StunComponent(Behaviour comp, float duration)
    {
        comp.enabled = false;
        yield return new WaitForSeconds(duration);
        comp.enabled = true;
    }
    */
}
