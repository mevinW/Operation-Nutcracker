using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dialogue controller that shows initial lines via a button,
/// then advances based on both players' input and two‑acorn collection,
/// finally transitions to Pavilion scene.
/// </summary>
public class ShopDialogue : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Button continueButton;

    [Header("Dialogue Content")]
    [Tooltip("Lines of dialogue/instructions to display.")]
    [SerializeField] private string[] dialogueLines;

    [Header("Typing Settings")]
    [SerializeField] private float wordSpeed = 0.035f;

    private int currentIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        if (GameManager.Instance.currentShop == 1)
        {
            // Validate UI
            if (dialoguePanel == null || dialogueText == null || continueButton == null)
            {
                Debug.LogError("NPCDialogue: Assign dialoguePanel, dialogueText, and continueButton in the Inspector.");
                enabled = false;
                return;
            }

            // Wire up Continue button
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);

            // Init UI
            dialogueText.text = string.Empty;
            dialoguePanel.SetActive(true);
            continueButton.gameObject.SetActive(false);

            // Start first line
            StartCoroutine(TypeLine());
        }
    }

    private void OnContinueClicked()
    {
        if (isTyping) return;

        // After last line, go to shop
        if (currentIndex == dialogueLines.Length - 1)
        {
            dialoguePanel.SetActive(false);
        }
        else
        {
            Advance();
        }
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = string.Empty;
        continueButton.gameObject.SetActive(false);

        foreach (char c in dialogueLines[currentIndex])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
        continueButton.gameObject.SetActive(true);
    }

    private void Advance()
    {
        StopAllCoroutines();
        currentIndex++;
        if (currentIndex < dialogueLines.Length)
            StartCoroutine(TypeLine());
        else
            dialoguePanel.SetActive(false);
    }
}
