using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Climber : MonoBehaviour
{
    public enum ControlScheme
    {
        WASD,
        ArrowKeys
    }

    [Header("Which Player (0 = P1, 1 = P2)")]
    [SerializeField] private int playerIndex = 0;

    [Header("Controls")]
    [Tooltip("Which keys to use for climbing.")]
    [SerializeField] private ControlScheme controlScheme = ControlScheme.WASD;

    [Header("Climbing Settings")]
    [Tooltip("How fast the player climbs up/down.")]
    public float climbSpeed = 3f;

    private Rigidbody2D rb;
    private float normalGravityScale;

    // for detecting climb‐zones
    private bool inClimbZone = false;
    // once you press the key to latch
    private bool isClimbing = false;

    // reference to this player’s gadget controller
    private GadgetController2D gadgetController;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        normalGravityScale = rb.gravityScale;

        // find the matching GadgetController2D by playerIndex
        foreach (var gc in FindObjectsOfType<GadgetController2D>())
        {
            if (gc.PlayerIndex == playerIndex)
            {
                gadgetController = gc;
                break;
            }
        }
    }

    void Update()
    {
        // must have claws **selected** to even attempt climbing
        if (!HasClawsSelected())
        {
            if (isClimbing)
            {
                // force‐drop if we lose selection mid‐climb
                isClimbing = false;
                rb.gravityScale = normalGravityScale;
            }
            return;
        }

        // map up/down keys
        KeyCode upKey = controlScheme == ControlScheme.WASD ? KeyCode.W : KeyCode.UpArrow;
        KeyCode downKey = controlScheme == ControlScheme.WASD ? KeyCode.S : KeyCode.DownArrow;

        // 1) latch on
        if (inClimbZone && !isClimbing && Input.GetKeyDown(upKey))
        {
            isClimbing = true;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        if (isClimbing)
        {
            // suspend gravity
            rb.gravityScale = 0f;

            // 2) climb movement
            float v = 0f;
            if (Input.GetKey(upKey)) v = 1f;
            if (Input.GetKey(downKey)) v = -1f;
            rb.velocity = new Vector2(rb.velocity.x, v * climbSpeed);

            // 3) if you exit the zone, drop
            if (!inClimbZone)
            {
                isClimbing = false;
                rb.gravityScale = normalGravityScale;
            }
        }
        else
        {
            // restore gravity
            rb.gravityScale = normalGravityScale;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Climb") && HasClawsSelected())
            inClimbZone = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Climb"))
        {
            inClimbZone = false;
            isClimbing = false;
        }
    }

    // helper: returns true only if the selected gadget is ClimberClaws
    private bool HasClawsSelected()
    {
        if (gadgetController == null) return false;

        int sel = gadgetController.SelectedIndex;
        if (sel < 0) return false;

        // get the equipped list in order
        var equipped = GameManager.Instance.GetEquippedItems(playerIndex);
        // guard against empty slots
        if (sel >= equipped.Count) return false;

        return equipped[sel] == "ClimberClaws";
    }
}
