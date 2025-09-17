using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressureButton : MonoBehaviour
{
    [Tooltip("The primary object to toggle when this button is pressed/released.")]
    [SerializeField] private GameObject primary;

    [Tooltip("Optional secondary object: one is always on while the other is off.")]
    [SerializeField] private GameObject secondary;

    // Keep track of objects currently on the button
    private int pressCount = 0;

    private void Awake()
    {
        if (primary == null)
            Debug.LogError($"[{nameof(PressureButton)}] Primary reference not set on {gameObject.name}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            Debug.Log("Enter");
            pressCount++;
            if (pressCount == 1)
                SetStates(isPressed: true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            pressCount = Mathf.Max(pressCount - 1, 0);
            if (pressCount == 0)
                SetStates(isPressed: false);
        }
    }

    /// <summary>
    /// When pressed: primary ON, secondary OFF. When released: primary OFF, secondary ON.
    /// </summary>
    private void SetStates(bool isPressed)
    {
        primary.SetActive(!isPressed);
        if (secondary != null)
            secondary.SetActive(isPressed);
    }
}
