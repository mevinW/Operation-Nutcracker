// PuzzleTriggerZoneSimple.cs
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PuzzleTrigger : MonoBehaviour
{
    [Header("Drag exactly 4 PuzzleButton objects here:")]
    [Tooltip("Indexes 0‑2 = Player One; 1‑3 = Player Two.")]
    public PuzzleButton[] buttons = new PuzzleButton[4];

    [Header("What to activate when Player One solves")]
    public GameObject playerOneSuccessObject;

    [Header("What to activate when Player Two solves")]
    public GameObject playerTwoSuccessObject;

    // Pre‑cache each player's full 3‑button sets
    private PuzzleButton[] _oneSet;
    private PuzzleButton[] _twoSet;

    // Which triple is currently highlighted (0 = buttons[0..2], 1 = buttons[1..3])
    private int _activeStart = -1;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        if (buttons.Length != 4 || buttons.Any(b => b == null))
            Debug.LogError("[PuzzleTriggerZoneSimple] Assign exactly 4 PuzzleButtons (none null).");

        _oneSet = buttons.Take(3).ToArray();         // buttons[0], [1], [2]
        _twoSet = buttons.Skip(1).Take(3).ToArray();  // buttons[1], [2], [3]
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            ShowVisualSet(startIndex: 0);
        else if (other.CompareTag("PlayerTwo"))
            ShowVisualSet(startIndex: 1);
    }

    /// <summary>
    /// Fade‑in the 3 buttons starting at startIndex, fade‑out the other one.
    /// </summary>
    private void ShowVisualSet(int startIndex)
    {
        _activeStart = startIndex;
        for (int i = 0; i < buttons.Length; i++)
        {
            bool highlight = (i >= startIndex && i < startIndex + 3);
            buttons[i].SetActiveVisual(highlight);
        }
    }

    void Update()
    {
        // 0) Let each button re‑check whether it’s covered by a Box
        foreach (var btn in buttons)
            btn.CheckPressed();

        // 1) Immediately turn each player's success object ON iff all their buttons are pressed
        UpdateSuccessState(_oneSet, playerOneSuccessObject, "Player One");
        UpdateSuccessState(_twoSet, playerTwoSuccessObject, "Player Two");
    }

    /// <summary>
    /// Ensures successObj.activeSelf == allPressed, and logs only on changes.
    /// </summary>
    private void UpdateSuccessState(PuzzleButton[] set, GameObject successObj, string playerName)
    {
        if (successObj == null) return;

        bool allPressed = set.All(b => b.IsPressed);
        if (successObj.activeSelf != allPressed)
        {
            successObj.SetActive(allPressed);
            Debug.Log($"[PuzzleTriggerZoneSimple] {playerName} {(allPressed ? "solved!" : "unsolved (buttons uncovered).")}");
        }
    }
}
