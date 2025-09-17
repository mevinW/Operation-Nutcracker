using UnityEngine;
using UnityEngine.UI;

public class AcornCollector : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Text component that displays the acorn count.")]
    [SerializeField] private Text acornText;

    [Header("Gameplay Settings")]
    [Tooltip("Tag used to identify acorn pickups.")]
    [SerializeField] private string acornTag = "Acorn";

    private int acornCount = 0;

    private void Start()
    {
        if (acornText == null)
        {
            Debug.LogError("AcornCollector: Please assign a UI Text component in the Inspector.");
            return;
        }
        UpdateAcornText();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(acornTag))
        {
            acornCount++;
            //GameManager.Instance.acornsCollected += 1;
            UpdateAcornText();
            other.gameObject.SetActive(false);
            Debug.Log("Collected Acorn");
        }
    }

    /// <summary>
    /// Updates the UI text to reflect the current acorn count.
    /// </summary>
    private void UpdateAcornText()
    {
        acornText.text = acornCount.ToString();
    }

    public int getAcornCount()
    {
        return acornCount;
    }
}
