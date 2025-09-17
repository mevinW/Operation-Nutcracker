using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [Header("Shop-Scene Equip HUD")]
    public EquipmentHUD playerOneHUD;
    public EquipmentHUD playerTwoHUD;

    [Header("Purchased-Tint")]
    public Color purchasedTint = new Color(1f, 1f, 1f, 0.5f);
    public Color normalTint = Color.white;

    [Header("Player Selector Toggles")]
    public Toggle playerOneToggle;
    public Toggle playerTwoToggle;

    [Header("Item Buttons")]
    public Button tailSwipeButton;
    public Button climberClawsButton;
    public Button checkpointButton;
    public Button invisibilityCloakButton;
    public Button shieldButton;
    public Button bootsButton;
    public Button wingsuitButton;
    public Button acornButton;
    public Button launcherButton;

    [Header("Description Panel")]
    public GameObject descriptionPanel;
    public Text titleText;
    public Text descriptionText;
    public Text costText;
    public Button purchaseButton;

    [Header("Equip Button")]
    public Button equipButton;
    public Text equipButtonText;

    [Header("Currency Display")]
    public Text acornTextOne;
    public Text acornTextTwo;

    private int currentPlayer = 0; // 0 = P1, 1 = P2
    private string currentItemID;
    private int currentCost;
    private System.Action onPurchase;

    private EquipmentHUD ActiveHUD => (currentPlayer == 0)
        ? playerOneHUD
        : playerTwoHUD;

    void Awake()
    {
        // toggles
        playerOneToggle.onValueChanged.AddListener(on => { if (on) SwitchPlayer(0); });
        playerTwoToggle.onValueChanged.AddListener(on => { if (on) SwitchPlayer(1); });
        playerOneToggle.isOn = true;

        // item buttons
        tailSwipeButton.onClick.AddListener(() => ShowItem(
            "TailSwipe", "Tail Swipe",
            "Temporarily stun opponents or NPCs with a quick swipe of your tail.\n(Cooldown: 8 s)",
            5,
            () => Debug.Log($"Player {currentPlayer + 1} granted TailSwipe!")
        ));

        climberClawsButton.onClick.AddListener(() => ShowItem(
            "ClimberClaws", "Climber Claws",
            "Scale climbable objects with these claws (vines, ladders, ropes, etc).",
            3,
            () => Debug.Log($"Player {currentPlayer + 1} granted ClimberClaws!")
        ));

        checkpointButton.onClick.AddListener(() => ShowItem(
            "Checkpoint", "Checkpoint",
            "Set your checkpoint using this gadget.\n(Cooldown: 13 s)",
            7,
            () => Debug.Log($"Player {currentPlayer + 1} granted Checkpoint!")
        ));

        invisibilityCloakButton.onClick.AddListener(() => ShowItem(
            "InvisibilityCloak", "Invisibility Cloak",
            "Temporarily hide from all hostile enemies. Effects last 3 seconds.\n(Cooldown: 8 s)",
            4,
            () => Debug.Log($"Player {currentPlayer + 1} granted InvisibilityCloak!")
        ));
        shieldButton.onClick.AddListener(() => ShowItem(
            "Shield", "Shield",
            "Temporarily recieve immunity from all projectile. Effects last 3 seconds.\n(Cooldown: 8 s)",
            4,
            () => Debug.Log($"Player {currentPlayer + 1} granted Shield!")
        ));
        bootsButton.onClick.AddListener(() => ShowItem(
            "Boots", "Spring Boots",
            "Next jump is boosted, allowing you to scale greater heights with ease.\n(Cooldown: 6 s)",
            3,
            () => Debug.Log($"Player {currentPlayer + 1} granted SpringBoots!")
        ));
        wingsuitButton.onClick.AddListener(() => ShowItem(
            "Wingsuit", "Wingsuit",
            "Navigate air currents with greater control. Allows for movement up down, left, and right.",
            3,
            () => Debug.Log($"Player {currentPlayer + 1} granted Wingsuit!")
        ));
        acornButton.onClick.AddListener(() => ShowItem(
            "Acorn", "Dummy Acorn",
            "Trick your opponent by sneakily placing dummy acorns. Temporarily stun unsuspecting collector. Effect lasts 3 seconds.\n(Cooldown: 7s)",
            4,
            () => Debug.Log($"Player {currentPlayer + 1} granted Acorn!")
        ));
        launcherButton.onClick.AddListener(() => ShowItem(
            "Launcher", "Acorn Launcher",
            "Reset your opponent, stun NPCs, and hit levers with this all in one acorn launcher!.\n(Cooldown: 5s)",
            6,
            () => Debug.Log($"Player {currentPlayer + 1} granted Acorn Launcher!")
        ));

        purchaseButton.onClick.AddListener(HandlePurchase);
        equipButton.onClick.AddListener(HandleEquip);
        equipButton.gameObject.SetActive(false);

        descriptionPanel.SetActive(false);
        OnEnable();
    }

    void OnEnable()
    {
        UpdateAllCurrencyDisplay();
        UpdateAllItemButtons();
        RefreshPurchaseButton();
        RefreshEquipButton();
        descriptionPanel.SetActive(false);
        ActiveHUD.RefreshForPlayer(currentPlayer);
    }

    private void SwitchPlayer(int playerIdx)
    {
        currentPlayer = playerIdx;
        UpdateAllCurrencyDisplay();
        UpdateAllItemButtons();
        RefreshPurchaseButton();
        RefreshEquipButton();
        descriptionPanel.SetActive(false);
        ActiveHUD.RefreshForPlayer(currentPlayer);
    }

    private void UpdateAllCurrencyDisplay()
    {
        var gm = GameManager.Instance;
        acornTextOne.text = gm.acornsCollectedOne.ToString();
        acornTextTwo.text = gm.acornsCollectedTwo.ToString();
    }

    private void UpdateAllItemButtons()
    {
        var gm = GameManager.Instance;
        bool boughtTail = gm.IsPurchased("TailSwipe", currentPlayer);
        tailSwipeButton.interactable = true;
        tailSwipeButton.image.color = boughtTail ? purchasedTint : normalTint;

        bool boughtClaws = gm.IsPurchased("ClimberClaws", currentPlayer);
        climberClawsButton.interactable = true;
        climberClawsButton.image.color = boughtClaws ? purchasedTint : normalTint;

        bool boughtCheckpoint = gm.IsPurchased("Checkpoint", currentPlayer);
        checkpointButton.interactable = true;
        checkpointButton.image.color = boughtCheckpoint ? purchasedTint : normalTint;

        bool boughtInvisibility = gm.IsPurchased("InvisibilityCloak", currentPlayer);
        invisibilityCloakButton.interactable = true;
        invisibilityCloakButton.image.color = boughtInvisibility ? purchasedTint : normalTint;

        bool boughtShield = gm.IsPurchased("Shield", currentPlayer);
        shieldButton.interactable = true;
        shieldButton.image.color = boughtShield ? purchasedTint : normalTint;

        bool boughtBoots = gm.IsPurchased("Boots", currentPlayer);
        bootsButton.interactable = true;
        bootsButton.image.color = boughtBoots ? purchasedTint : normalTint;

        bool boughtWingsuit = gm.IsPurchased("Wingsuit", currentPlayer);
        wingsuitButton.interactable = true;
        wingsuitButton.image.color = boughtWingsuit ? purchasedTint : normalTint;

        bool boughtAcorn = gm.IsPurchased("Acorn", currentPlayer);
        acornButton.interactable = true;
        acornButton.image.color = boughtAcorn ? purchasedTint : normalTint;

        bool boughtLauncher = gm.IsPurchased("Launcher", currentPlayer);
        launcherButton.interactable = true;
        launcherButton.image.color = boughtLauncher ? purchasedTint : normalTint;
    }

    private void ShowItem(string itemID, string title, string desc, int cost, System.Action purchaseAction)
    {
        if (descriptionPanel.activeSelf && currentItemID == itemID)
        {
            descriptionPanel.SetActive(false);
            return;
        }

        currentItemID = itemID;
        titleText.text = title;
        descriptionText.text = desc;
        currentCost = cost;
        onPurchase = purchaseAction;
        costText.text = $"{cost} Acorns";

        descriptionPanel.SetActive(true);
        RefreshPurchaseButton();
        RefreshEquipButton();
    }

    private void RefreshPurchaseButton()
    {
        var gm = GameManager.Instance;
        bool already = gm.IsPurchased(currentItemID, currentPlayer);
        int acorns = gm.GetAcorns(currentPlayer);
        purchaseButton.interactable = !already && (acorns >= currentCost);
    }

    private void HandlePurchase()
    {
        var gm = GameManager.Instance;
        int acorns = gm.GetAcorns(currentPlayer);
        if (acorns < currentCost || gm.IsPurchased(currentItemID, currentPlayer))
            return;

        // Record purchase
        gm.AddPurchase(currentItemID, currentPlayer);
        gm.SetAcorns(currentPlayer, acorns - currentCost);

        // Refresh UI
        UpdateAllCurrencyDisplay();
        UpdateAllItemButtons();
        RefreshPurchaseButton();
        RefreshEquipButton();
        // keep panel open so equipButton shows immediately
        ActiveHUD.RefreshForPlayer(currentPlayer);

        // Grant effect
        onPurchase?.Invoke();
    }

    private void RefreshEquipButton()
    {
        var gm = GameManager.Instance;
        if (!gm.IsPurchased(currentItemID, currentPlayer))
        {
            equipButton.gameObject.SetActive(false);
            return;
        }

        equipButton.gameObject.SetActive(true);
        bool isEq = gm.IsEquipped(currentItemID, currentPlayer);
        equipButtonText.text = isEq ? "Unequip" : "Equip";
        equipButton.interactable = isEq
            ? true
            : (gm.EquippedCount(currentPlayer) < 3);
    }

    private void HandleEquip()
    {
        var gm = GameManager.Instance;
        if (gm.IsEquipped(currentItemID, currentPlayer))
            gm.Unequip(currentItemID, currentPlayer);
        else
            gm.Equip(currentItemID, currentPlayer);

        RefreshEquipButton();
        ActiveHUD.RefreshForPlayer(currentPlayer);
    }
}
