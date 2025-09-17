using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public enum ControlScheme
    {
        WASD,
        ArrowKeys
    }

    [Header("Which Player (0=P1,1=P2)")]
    [SerializeField] private int playerIndex = 0;

    [Header("Controls")]
    [Tooltip("Which keyset to use for this player.")]
    [SerializeField] private ControlScheme controlScheme = ControlScheme.WASD;

    [Header("Movement Settings")]
    [Tooltip("Horizontal movement speed in units/second.")]
    [SerializeField] private float moveSpeed = 8f;
    [Tooltip("Impulse force applied when jumping.")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float jumpForceBoosted = 8f;
    [SerializeField] private float lateralInAir = 0.75f;  // without wingsuit

    [Header("Ground Check Settings")]
    [Tooltip("GameObject used to check whether the player is on the ground.")]
    [SerializeField] private GameObject groundCheck;
    [Tooltip("Radius of the ground check circle.")]
    [SerializeField] private float groundCheckRadius = 0.1f;
    [Tooltip("Layers considered as ground.")]
    [SerializeField] private LayerMask groundLayer;

    // components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private GadgetController2D gadgetController;

    // state
    private bool isGrounded;
    private float horizontalInput;
    public bool isInvisible = false;
    public bool isShield = false;
    public bool isBoots = false;
    private bool inAirCurrent = false;

    private Vector3 originalSpawnPos;
    public bool IsGrounded => isGrounded;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.5f, 0f);

    public Animator animator;

    private float conveyorSpeedModifier = 0f;
    public float GetLateralControl() => lateralInAir;

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.collider.CompareTag("ConveyorBelt"))
            conveyorSpeedModifier = col.collider.GetComponent<ConveyorBelt>().Speed;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("ConveyorBelt"))
            conveyorSpeedModifier = 0f;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSpawnPos = transform.position;     // ← store spawn

        // find the matching GadgetController2D by playerIndex
        foreach (var gc in FindObjectsOfType<GadgetController2D>())
        {
            if (gc.PlayerIndex == playerIndex)
            {
                gadgetController = gc;
                break;
            }
        }

        if (groundCheck == null)
            Debug.LogError("PlayerController: Please assign a GroundCheck GameObject in the Inspector.");
    }

    private void Update()
    {
        // Reset
        horizontalInput = 0f;

        // Read input based on chosen scheme
        if (controlScheme == ControlScheme.WASD)
        {
            if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
            else if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

            if (Input.GetKeyDown(KeyCode.W) && isGrounded)
            {
                // 1) clear any existing vertical motion
                rb.velocity = new Vector2(rb.velocity.x, 0f);

                // 2) apply jump
                if (isBoots)
                {
                    rb.AddForce(Vector2.up * jumpForceBoosted, ForceMode2D.Impulse);
                    isBoots = false;
                }
                else
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }
        }
        else // ArrowKeys
        {
            if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f;
            else if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f;

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            {
                // 1) clear any existing vertical motion
                rb.velocity = new Vector2(rb.velocity.x, 0f);

                // 2) apply jump
                if (isBoots)
                {
                    rb.AddForce(Vector2.up * jumpForceBoosted, ForceMode2D.Impulse);
                    isBoots = false;
                }
                else
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }
        }

        // Animator
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        Vector3 scale = transform.localScale;

        if (horizontalInput > 0f)
        {
            // Ensure X is positive
            scale.x = Mathf.Abs(scale.x);
        }
        else if (horizontalInput < 0f)
        {
            // Ensure X is negative
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        // 2) Ground check
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.transform.position,
                groundCheckRadius,
                groundLayer
            );
        }

        animator.SetBool("isJumping", !isGrounded);
        // if we’re inside an AirCurrent, bail and let AirCurrent set velocity
        if (inAirCurrent)
           return;

        // otherwise, your normal movement logic:
        float targetSpeedX = horizontalInput * moveSpeed
                          + conveyorSpeedModifier;
        Vector2 vel = rb.velocity;
        vel.x = targetSpeedX;
        rb.velocity = vel;
    }

    public void SetInAirCurrent(bool inside) => inAirCurrent = inside;
    public float GetHorizontalInput() => horizontalInput;
    public float GetVerticalInput()
    {
        if (controlScheme == ControlScheme.WASD)
        {
            if (Input.GetKey(KeyCode.W)) return +1f;
            if (Input.GetKey(KeyCode.S)) return -1f;
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow)) return +1f;
            if (Input.GetKey(KeyCode.DownArrow)) return -1f;
        }
        return 0f;
    }

    public void Respawn()
    {
        // default: start position + offset
        Vector3 targetPos = originalSpawnPos + offset;

        // look for the matching gadget controller
        foreach (var g in FindObjectsOfType<GadgetController2D>())
        {
            if (g.PlayerIndex == playerIndex && g.IsTeleporterPlaced)
            {
                targetPos = g.TeleporterPosition + offset;
                break;
            }
        }

        // move & reset velocity
        transform.position = targetPos;
        rb.velocity = Vector2.zero;

        // reset animator state
        animator.SetBool("isJumping", false);
    }

    public int getIndex()
    {
        return playerIndex;
    }

    public bool HasWingsuitSelected()
    {
        if (gadgetController == null) return false;

        int sel = gadgetController.SelectedIndex;
        if (sel < 0) return false;

        // get the equipped list in order
        var equipped = GameManager.Instance.GetEquippedItems(playerIndex);
        // guard against empty slots
        if (sel >= equipped.Count) return false;

        return equipped[sel] == "Wingsuit";
    }

    /*
    public bool HasWingsuitEquipped()
    {
        int sel = getIndex();
        var eq = GameManager.Instance.GetEquippedItems(playerIndex);
        return sel >= 0 && sel < eq.Count && eq[sel] == "Wingsuit";
    }
    */

    public void SetSpawnpoint(Vector3 newPos)
    {
        originalSpawnPos = newPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
        }
    }
}
