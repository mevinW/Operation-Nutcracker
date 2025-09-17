using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dialogue controller that shows initial lines via a button,
/// then advances based on both players' input and two‑acorn collection,
/// finally transitions to Pavilion scene.
/// </summary>
public class NPCDialogue : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Button continueButton;

    [Header("Dialogue Content")]
    [Tooltip("Lines of dialogue/instructions to display.")]
    [SerializeField] private string[] dialogueLines;

    [Header("Script References")]
    [Tooltip("Reference the AcornCollector script to check collected count.")]
    [SerializeField] private AcornCollector acornCollector;
    [Tooltip("Reference the SceneChanger script for scene transition.")]
    [SerializeField] private SceneChanger sceneChanger;

    [Header("Typing Settings")]
    [SerializeField] private float wordSpeed = 0.05f;

    [Header("Tutorial Objects")]
    [Tooltip("First acorn GameObject to spawn.")]
    [SerializeField] private GameObject acorn1;
    [Tooltip("Second acorn GameObject to spawn.")]
    [SerializeField] private GameObject acorn2;

    private int currentIndex = 0;
    private bool isTyping = false;

    // Flags for step 2 (movement) and step 3 (jump)
    private bool p1Moved, p2Moved;
    private bool p1Jumped, p2Jumped;

    void Start()
    {
        // Validate UI
        if (dialoguePanel == null || dialogueText == null || continueButton == null)
        {
            Debug.LogError("NPCDialogue: Assign dialoguePanel, dialogueText, and continueButton in the Inspector.");
            enabled = false;
            return;
        }
        // Validate scripts
        if (acornCollector == null || sceneChanger == null)
        {
            Debug.LogError("NPCDialogue: Assign AcornCollector and SceneChanger in the Inspector.");
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

        // Hide acorns until needed
        if (acorn1) acorn1.SetActive(false);
        if (acorn2) acorn2.SetActive(false);

        // Start first line
        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (isTyping) return;

        // --- STEP 2: movement tutorial ---
        if (currentIndex == 2)
        {
            // Track each press independently
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                p1Moved = true;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                p2Moved = true;

            if (p1Moved && p2Moved)
            {
                ResetMovementFlags();
                Advance();
            }
        }
        // --- STEP 3: jump tutorial ---
        else if (currentIndex == 3)
        {
            if (Input.GetKeyDown(KeyCode.W))
                p1Jumped = true;
            if (Input.GetKeyDown(KeyCode.UpArrow))
                p2Jumped = true;

            if (p1Jumped && p2Jumped)
            {
                ResetJumpFlags();
                Advance();
            }
        }
        // --- STEP 4: two‑acorn collection ---
        else if (currentIndex == 4)
        {
            if (acorn1.activeSelf == false && acorn2.activeSelf == false)
                Advance();
        }
    }

    private void OnContinueClicked()
    {
        if (isTyping) return;

        // After last line, go to shop
        if (currentIndex == dialogueLines.Length - 1)
        {
            sceneChanger.loadShop();
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

        // Show Continue button only on lines 0–1 and the very last line
        if (currentIndex < 2 || currentIndex == dialogueLines.Length - 1)
            continueButton.gameObject.SetActive(true);

        // Spawn both acorns at the two‑acorn step
        if (currentIndex == 4)
        {
            acorn1.SetActive(true);
            acorn2.SetActive(true);
        }
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

    private void ResetMovementFlags()
    {
        p1Moved = p2Moved = false;
    }

    private void ResetJumpFlags()
    {
        p1Jumped = p2Jumped = false;
    }
}
