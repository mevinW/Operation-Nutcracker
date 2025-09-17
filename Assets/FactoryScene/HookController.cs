using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HookController2D : MonoBehaviour
{
    [Header("Release Impulse")]
    [Tooltip("Guaranteed horizontal impulse (world units) applied when unlatching.")]
    [SerializeField] private float horizontalImpulse = 10f;
    [Tooltip("Guaranteed vertical impulse (world units) applied when unlatching.")]
    [SerializeField] private float verticalImpulse = 5f;

    [Header("Latch Settings")]
    [Tooltip("Collider at the tip of the hook (Is Trigger = true).")]
    [SerializeField] private Collider2D latchCollider;
    [Tooltip("How long (seconds) before you can latch again after releasing.")]
    [SerializeField] private float relatchDelay = 0.3f;
    [SerializeField] private float duration = 0.5f;

    [Header("Player Tags")]
    [Tooltip("Tag of Player One.")]
    [SerializeField] private string playerOneTag = "Player";
    [Tooltip("Tag of Player Two.")]
    [SerializeField] private string playerTwoTag = "PlayerTwo";

    private Rigidbody2D hookRb;

    // State
    private bool isPlayerLatched = false;
    private bool canLatch = true;
    private Rigidbody2D playerRb;
    private HingeJoint2D playerJoint;
    private Vector2 latchPointWorld;
    private string latchedTag;

    private void Awake()
    {
        hookRb = GetComponent<Rigidbody2D>();
        if (latchCollider == null)
            Debug.LogError("[HookController2D] Assign a trigger collider at the hook tip for latching.");
    }

    private void Update()
    {
        if (!isPlayerLatched) return;

        // Only the latched player can unlatch with their own key
        if ((latchedTag == playerOneTag && Input.GetKeyDown(KeyCode.W)) ||
            (latchedTag == playerTwoTag && Input.GetKeyDown(KeyCode.UpArrow)))
        {
            ReleasePlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!canLatch || isPlayerLatched) return;

        // Latch either player
        if (col.CompareTag(playerOneTag) || col.CompareTag(playerTwoTag))
        {
            LatchPlayer(col.attachedRigidbody, col.tag);
        }
    }

    private void LatchPlayer(Rigidbody2D playerBody, string tag)
    {
        canLatch = false;
        isPlayerLatched = true;
        playerRb = playerBody;
        latchedTag = tag;
        latchPointWorld = latchCollider.bounds.center;

        playerJoint = playerRb.gameObject.AddComponent<HingeJoint2D>();
        playerJoint.autoConfigureConnectedAnchor = false;
        playerJoint.connectedBody = hookRb;
        playerJoint.anchor = playerRb.transform.InverseTransformPoint(playerRb.position);
        playerJoint.connectedAnchor = hookRb.transform.InverseTransformPoint(latchPointWorld);
    }

    private void ReleasePlayer()
    {
        if (!isPlayerLatched) return;

        // 1) Destroy the hinge so the player is free
        Destroy(playerJoint);
        playerJoint = null;
        isPlayerLatched = false;

        // 2) Reset any residual velocity so the impulse is consistent
        playerRb.velocity = Vector2.zero;

        // 3) Determine horizontal direction from hook → player
        float dir = Mathf.Sign((playerRb.position - hookRb.position).x);

        // 4) Build an impulse vector exactly as before
        Vector2 impulse = new Vector2(horizontalImpulse * dir,
                                      verticalImpulse);

        // 5) Apply it as an impulse
        playerRb.AddForce(impulse, ForceMode2D.Impulse);

        // 6) Start relatch cooldown
        StartCoroutine(RelatchCooldown());

        // disable player controller briefly
        var pc = playerRb.GetComponent<PlayerController>();
        if (pc != null)
            StartCoroutine(DisableControllerFor(pc, duration));
    }

    private IEnumerator DisableControllerFor(PlayerController pc, float duration)
    {
        pc.enabled = false;
        yield return new WaitForSeconds(duration);
        pc.enabled = true;
    }

    private IEnumerator RelatchCooldown()
    {
        yield return new WaitForSeconds(relatchDelay);
        canLatch = true;
    }
}
