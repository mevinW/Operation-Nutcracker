using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("Where should the player respawn?")]
    [SerializeField] private Vector3 spawnPos = Vector3.zero;
    [Tooltip("Gizmo color in the editor.")]
    [SerializeField] private Color gizmoColor = Color.green;
    [Tooltip("How big to draw the spawn point gizmo.")]
    [SerializeField] private float gizmoRadius = 0.3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerTwo"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.SetSpawnpoint(spawnPos);
        }
    }

    // Draw a gizmo in the Scene view to show exactly where spawnPos is
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        // Draw a wire sphere at the spawn position (in world space)
        Gizmos.DrawWireSphere(spawnPos, gizmoRadius);

        // Optionally, draw a small solid cube for extra visibility:
        Gizmos.DrawCube(spawnPos, Vector3.one * gizmoRadius * 0.5f);
    }
}
