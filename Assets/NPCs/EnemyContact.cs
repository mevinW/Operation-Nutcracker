using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    // Called if your collider is NOT marked "Is Trigger"
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.collider);
    }

    // Called if your collider IS marked "Is Trigger"
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other);
    }

    private void HandleHit(Collider2D col)
    {
        // only care about Player or PlayerTwo tags
        if (col.CompareTag("Player") || col.CompareTag("PlayerTwo"))
        {
            // get their PlayerController
            var pc = col.GetComponent<PlayerController>();
            if (pc != null)
            {
                if (!pc.isInvisible)
                {
                    pc.Respawn();
                    Debug.Log($"Respawned {col.tag} via enemy contact");
                }
            }

            // destroy this enemy so it can't hit again
            Destroy(gameObject);
        }
    }
}
