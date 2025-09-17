using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float conveyorSpeed = -2f;
    public float Speed => conveyorSpeed;
}
