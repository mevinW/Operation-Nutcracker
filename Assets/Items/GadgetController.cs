using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetController2D : MonoBehaviour
{
    [Header("Self Configuration")]
    [Tooltip("Tag of the GameObject that owns this gadget (so we don’t hit ourselves).")]
    [SerializeField] private string selfTag = "Player";

    [Header("Which Player (0=P1,1=P2)")]
    [SerializeField] private int playerIndex = 0;
    public int PlayerIndex => playerIndex;

    [Header("Player & Input")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform launcherTransform;
    [SerializeField] private PlayerController playerController;

    // Sprite dims
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Color originalColor;

    [Header("Teleporter (\"Checkpoint\")")]
    [SerializeField] private GameObject teleporterPrefab;
    [SerializeField] private Vector3 placementOffset = new Vector3(0, 0f, 0);
    [SerializeField] private float teleportCooldown = 2f;
        //13f;
    private float teleportTimer = 0f;
    private bool canUseTeleporter = true;
    private GameObject teleporterInstance;

    [Header("Dummy Acorn")]
    [SerializeField] private GameObject dummyAcorn;
    [SerializeField] private Vector3 placementOffsetAcorn = new Vector3(0, 0.2f, 0);
    [SerializeField] private float acornCooldown = 2f;
        //7f;
    private float acornTimer = 0f;
    private bool canUseAcorn = true;

    [Header("Acorn Launcher")]
    [SerializeField] private GameObject ammo;
    [SerializeField] private Vector3 placementOffsetAmmo = new Vector3(0, 0, 0);
    [SerializeField] private float launcherCooldown = 2f;
        //5f;
    private float launcherTimer = 0f;
    private bool canUseLauncher = true;
    [SerializeField] private float launchSpeed = 5f;    // ← new!

    [Header("Tail‑Swipe")]
    [SerializeField] private float swipeRadius = 1.5f;
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private float swipeCooldown = 2f;
        //8f;
    private float swipeTimer = 0f;
    private bool canUseSwipe = true;
    [SerializeField] private LayerMask swipeLayerMask;

    [Header("Invisibility Cloak")]
    [SerializeField] private float invisibilityDuration = 3f;
    [SerializeField] private float invisibilityCooldown = 2f;
        //8f;
    private float invisibilityTimer = 0f;
    private bool canUseInvisibility = true;

    [Header("Shield")]
    [SerializeField] private float shieldDuration = 3f;
    [SerializeField] private float shieldCooldown = 2f;
        //8f;
    private float shieldTimer = 0f;
    private bool canUseShield = true;
    private GameObject shield;
    private ShieldBehavior shieldBehavior;

    [Header("Boots")]
    [SerializeField] private float bootsDuration = 3f;
    [SerializeField] private float bootsCooldown = 2f;
        //6f;
    private float bootsTimer = 0f;
    private bool canUseBoots = true;

    // dynamic selection
    private int selectedIndex = 0;
    public int SelectedIndex => selectedIndex;

    // track last known equip count
    private int lastEquipCount = 0;

    void Awake()
    {
        // cache sprite renderer and original color
        // try GetComponentInChildren in case SpriteRenderer isn't on root
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"[Gadget] No SpriteRenderer found on or under {playerTransform.name}");
        }
        else
        {
            originalColor = spriteRenderer.color;
            Debug.Log($"[Gadget] Cached original color {originalColor} on {spriteRenderer.gameObject.name}");
        }

        shieldBehavior = GetComponentInChildren<ShieldBehavior>(true);
        if (shieldBehavior != null)
        {
            shield = shieldBehavior.gameObject;
            shield.SetActive(false);  // start disabled
        }
        else
        {
            Debug.LogError($"[Gadget] Couldn't find a ShieldBehavior component in children of {name}");
        }
    }

    void Update()
    {
        // Cooldown ticks
        if (!canUseTeleporter && (teleportTimer -= Time.deltaTime) <= 0f)
        {
            canUseTeleporter = true;
            RefreshHUD();
        }
        if (!canUseAcorn && (acornTimer -= Time.deltaTime) <= 0f)
        {
            canUseAcorn = true;
            RefreshHUD();
        }
        if (!canUseSwipe && (swipeTimer -= Time.deltaTime) <= 0f)
        {
            canUseSwipe = true;
            RefreshHUD();
        }
        if (!canUseInvisibility && (invisibilityTimer -= Time.deltaTime) <= 0f)
        {
            canUseInvisibility = true;
            RefreshHUD();
        }
        if (!canUseShield && (shieldTimer -= Time.deltaTime) <= 0f)
        {
            canUseShield = true;
            RefreshHUD();
        }
        if (!canUseBoots && (bootsTimer -= Time.deltaTime) <= 0f)
        {
            canUseBoots = true;
            RefreshHUD();
        }

        if (!canUseLauncher && (launcherTimer -= Time.deltaTime) <= 0f)
        {
            canUseLauncher = true;
            RefreshHUD();
        }

        // Update equipped list and selection
        var equipped = GameManager.Instance.GetEquippedItems(playerIndex);
        if (equipped.Count != lastEquipCount)
        {
            lastEquipCount = equipped.Count;
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, equipped.Count - 1));
            RefreshHUD();
        }

        // Selection keys
        if (playerIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && equipped.Count >= 1) ChangeSelection(0);
            if (Input.GetKeyDown(KeyCode.Alpha2) && equipped.Count >= 2) ChangeSelection(1);
            if (Input.GetKeyDown(KeyCode.Alpha3) && equipped.Count >= 3) ChangeSelection(2);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha8) && equipped.Count >= 1) ChangeSelection(0);
            if (Input.GetKeyDown(KeyCode.Alpha9) && equipped.Count >= 2) ChangeSelection(1);
            if (Input.GetKeyDown(KeyCode.Alpha0) && equipped.Count >= 3) ChangeSelection(2);
        }
        if (Input.GetKeyDown(KeyCode.Q) && equipped.Count > 0)
            ChangeSelection((selectedIndex + equipped.Count - 1) % equipped.Count);
        if (Input.GetKeyDown(KeyCode.E) && equipped.Count > 0)
            ChangeSelection((selectedIndex + 1) % equipped.Count);

        // Use gadget
        KeyCode useKey = (playerIndex == 0) ? KeyCode.R : KeyCode.P;
        if (Input.GetKeyDown(useKey) && equipped.Count > 0)
        {
            string id = equipped[selectedIndex];
            switch (id)
            {
                case "Checkpoint":
                    TryUseTeleporter();
                    break;
                case "Acorn":
                    TryUseAcorn();
                    break;
                case "Launcher":
                    TryUseLauncher();
                    break;
                case "TailSwipe":
                    TryUseSwipe();
                    break;
                case "InvisibilityCloak":
                    TryUseInvisibility();
                    break;
                case "Shield":
                    TryUseShield();
                    break;
                case "Boots":
                    Debug.Log("Tried using boots");
                    TryUseBoots();
                    break;
            }
        }
    }

    private void ChangeSelection(int newIndex)
    {
        if (newIndex == selectedIndex) return;
        selectedIndex = newIndex;
        RefreshHUD();
    }

    private void TryUseTeleporter()
    {
        var gm = GameManager.Instance;
        if (!gm.IsEquipped("Checkpoint", playerIndex) || !canUseTeleporter || (playerController == null && !playerController.IsGrounded))
            return;

        Vector3 pos = playerTransform.position + placementOffset;
        if (teleporterInstance == null)
            teleporterInstance = Instantiate(teleporterPrefab, pos, Quaternion.identity);
        else
            teleporterInstance.transform.position = pos;

        canUseTeleporter = false;
        teleportTimer = teleportCooldown;
        RefreshHUD();
    }

    private void TryUseAcorn()
    {
        var gm = GameManager.Instance;
        if (!gm.IsEquipped("Acorn", playerIndex) || !canUseAcorn || playerController == null)
            return;

        Vector3 pos = playerTransform.position + placementOffsetAcorn;
        var acorn = Instantiate(dummyAcorn, pos, Quaternion.identity)
            .GetComponent<DummyAcorn>();
        acorn.placer = playerController.getIndex();
        canUseAcorn = false;
        acornTimer = acornCooldown;
        RefreshHUD();
    }

    private void TryUseLauncher()
    {
        var gm = GameManager.Instance;
        if (!gm.IsEquipped("Launcher", playerIndex) || !canUseLauncher || launcherTransform == null)
            return;

        Vector3 spawnPos = launcherTransform.position + placementOffsetAmmo;

        // Determine facing from your own scale:
        float facingSign = Mathf.Sign(transform.localScale.x);

        // Rotate the acorn sprite to point "forward":
        float zRot = (facingSign > 0f) ? 90f : -90f;
        Quaternion ammoRotation = Quaternion.Euler(0f, 0f, zRot);
        var currAmmo = Instantiate(ammo, spawnPos, ammoRotation);

        // Give it initial velocity purely along world X, left or right:
        var rb = currAmmo.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 launchDir = Vector2.right * facingSign;
            rb.velocity = launchDir * launchSpeed;
        }

        playerController?.animator?.SetTrigger("isShooting");

        // Cooldown and HUD
        canUseLauncher = false;
        launcherTimer = launcherCooldown;
        RefreshHUD();
    }


    private void TryUseSwipe()
    {
        var gm = GameManager.Instance;
        if (!gm.IsEquipped("TailSwipe", playerIndex) || !canUseSwipe)
            return;

        playerController?.animator?.SetTrigger("isSwiping");
        Collider2D[] hits = Physics2D.OverlapCircleAll(playerTransform.position, swipeRadius, swipeLayerMask);
        foreach (var hit in hits)
        {
            var go = hit.attachedRigidbody != null ? hit.attachedRigidbody.gameObject : hit.gameObject;
            if (go.CompareTag(selfTag)) continue;
            var pc = go.GetComponent<PlayerController>();
            var npc = go.GetComponent<NPCPatrol>();
            var mp = go.GetComponent<MovingPlatform>();
            if (pc != null) StartCoroutine(StunComponent(pc, stunDuration));
            if (npc != null) StartCoroutine(StunComponent(npc, stunDuration));
            if (mp != null) StartCoroutine(StunComponent(mp, stunDuration));
        }

        canUseSwipe = false;
        swipeTimer = swipeCooldown;
        RefreshHUD();
    }

    private void TryUseInvisibility()
    {
        var gm = GameManager.Instance;
        if (!gm.IsEquipped("InvisibilityCloak", playerIndex) || !canUseInvisibility)
            return;

        StartCoroutine(InvisibilityRoutine());
        RefreshHUD();
    }

    private void TryUseShield()
    {
        var gm = GameManager.Instance;
        if (!gm.IsEquipped("Shield", playerIndex) || !canUseShield)
            return;

        StartCoroutine(ShieldRoutine());
        RefreshHUD();
    }

    private void TryUseBoots()
    {
        var gm = GameManager.Instance;
        if (!gm.IsEquipped("Boots", playerIndex) || !canUseBoots)
            return;

        canUseBoots = false;
        playerController.isBoots = true;
        bootsTimer = bootsCooldown;
        RefreshHUD();
    }

    private IEnumerator InvisibilityRoutine()
    {
        canUseInvisibility = false;
        playerController.isInvisible = true;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 185f / 255f);

        yield return new WaitForSeconds(invisibilityDuration);

        playerController.isInvisible = false;
        spriteRenderer.color = originalColor;
        invisibilityTimer = invisibilityCooldown;
    }

    private IEnumerator ShieldRoutine()
    {
        canUseShield = false;
        playerController.isShield = true;
        shield.SetActive(true);

        yield return new WaitForSeconds(invisibilityDuration);

        playerController.isShield = false;
        shield.SetActive(false);
        shieldTimer = shieldCooldown;
    }

    private IEnumerator StunComponent(Behaviour comp, float duration)
    {
        comp.enabled = false;
        yield return new WaitForSeconds(duration);
        comp.enabled = true;
    }

    public bool IsOnCooldown(string itemID)
    {
        switch (itemID)
        {
            case "Checkpoint": return !canUseTeleporter;
            case "Acorn": return !canUseAcorn;
            case "Launcher": return !canUseLauncher;
            case "TailSwipe": return !canUseSwipe;
            case "InvisibilityCloak": return !canUseInvisibility;
            case "Shield": return !canUseShield;
            case "Boots": return !canUseBoots;
            default: return false;
        }
    }

    private void RefreshHUD()
    {
        foreach (var hud in FindObjectsOfType<EquipmentHUD>())
            if (hud.PlayerIndex == playerIndex)
                hud.RefreshForPlayer(playerIndex);
    }

    public bool IsTeleporterPlaced => teleporterInstance != null;
    public Vector3 TeleporterPosition => teleporterInstance != null ? teleporterInstance.transform.position : Vector3.zero;
}
