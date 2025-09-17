// PuzzleButton.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class PuzzleButton : MonoBehaviour
{
    [Tooltip("Alpha when the button is inactive (0 = invisible, 1 = fully opaque)")]
    [Range(0f, 1f)]
    public float inactiveAlpha = 0.3f;

    private const float activeAlpha = 1f;

    private SpriteRenderer _sr;
    private Collider2D _col;
    private Color _baseColor;

    /// True whenever there’s at least one “Box”-tagged collider overlapping our bounds.
    public bool IsPressed { get; private set; }

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;

        // capture RGB—the alpha is ours to control
        _baseColor = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1f);

        SetActiveVisual(false);
    }

    /// Call this every frame from your puzzle manager to refresh pressed state.
    public void CheckPressed()
    {
        // Use our collider's world-space bounds as an OverlapBox
        Bounds b = _col.bounds;
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            b.center,
            b.size,
            0f    // no rotation
        );

        // If any overlapping collider carries the "Box" tag, we're pressed
        bool nowPressed = false;
        foreach (var c in hits)
        {
            if (c != null && c.CompareTag("Box"))
            {
                nowPressed = true;
                break;
            }
        }

        IsPressed = nowPressed;
    }

    /// Toggle the button’s opacity.
    public void SetActiveVisual(bool on)
    {
        float alpha = on ? activeAlpha : inactiveAlpha;
        _sr.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, alpha);
    }
}
