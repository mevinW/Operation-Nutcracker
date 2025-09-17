using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EquipmentHUD : MonoBehaviour
{
    [Tooltip("0 = Player 1, 1 = Player 2")]
    [SerializeField] private int playerIndex = 0;

    [Tooltip("Exactly three UI Images for equip slots")]
    [SerializeField] private Image[] equipSlots;

    [Header("Item Icons")]
    [SerializeField] private Sprite tailSwipeIcon;
    [SerializeField] private Sprite climberClawsIcon;
    [SerializeField] private Sprite checkpointIcon;
    [SerializeField] private Sprite invisibilityCloakIcon;
    [SerializeField] private Sprite shieldIcon;
    [SerializeField] private Sprite bootsIcon;
    [SerializeField] private Sprite wingsuitIcon;
    [SerializeField] private Sprite acornIcon;
    [SerializeField] private Sprite launcherIcon;

    [Header("Selection Tint")]
    [Tooltip("Multiplier for dimming the selected slot (0 = black, 1 = normal)")]
    [Range(0f, 1f)]
    [SerializeField] private float dimFactor = 0.5f;

    private Color[] originalColors;
    private GadgetController2D gadgetController;
    private int lastSelected = -1;

    public int PlayerIndex => playerIndex;

    void Awake()
    {
        // Cache original slot colors
        originalColors = new Color[equipSlots.Length];
        for (int i = 0; i < equipSlots.Length; i++)
            originalColors[i] = equipSlots[i].color;
    }

    void Start()
    {
        RefreshHUD();
    }

    void Update()
    {
        if (gadgetController != null)
        {
            int sel = gadgetController.SelectedIndex;
            if (sel != lastSelected)
            {
                lastSelected = sel;
                RefreshHUD();
            }
        }
    }

    /// <summary>
    /// Refresh and optionally switch player index
    /// </summary>
    public void RefreshForPlayer(int idx)
    {
        playerIndex = idx;

        gadgetController = FindObjectsOfType<GadgetController2D>()
            .FirstOrDefault(gc => gc.PlayerIndex == playerIndex);

        RefreshHUD();
    }

    private void RefreshHUD()
    {
        // 1) Populate icons based on equipped list
        List<string> equipped = GameManager.Instance.GetEquippedItems(playerIndex);

        // clear all slots
        for (int i = 0; i < equipSlots.Length; i++)
        {
            equipSlots[i].enabled = false;
            equipSlots[i].sprite = null;
            equipSlots[i].color = originalColors[i];
        }

        // fill equipped icons
        for (int i = 0; i < equipped.Count && i < equipSlots.Length; i++)
        {
            string id = equipped[i];
            bool onCooldown = gadgetController != null && gadgetController.IsOnCooldown(id);

            // only show if not on cooldown
            equipSlots[i].enabled = !onCooldown;
            if (!onCooldown)
                equipSlots[i].sprite = IconForID(id);
        }

        // 2) Tint selected slot darker
        int sel = gadgetController != null ? gadgetController.SelectedIndex : -1;
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i].enabled)
            {
                if (i == sel)
                {
                    Color c = originalColors[i];
                    equipSlots[i].color = new Color(
                        c.r * dimFactor,
                        c.g * dimFactor,
                        c.b * dimFactor,
                        c.a);
                }
                else
                {
                    equipSlots[i].color = originalColors[i];
                }
            }
        }
    }

    private Sprite IconForID(string id)
    {
        switch (id)
        {
            case "TailSwipe": return tailSwipeIcon;
            case "ClimberClaws": return climberClawsIcon;
            case "Checkpoint": return checkpointIcon;
            case "InvisibilityCloak": return invisibilityCloakIcon;
            case "Shield": return shieldIcon;
            case "Boots": return bootsIcon;
            case "Wingsuit": return wingsuitIcon;
            case "Acorn": return acornIcon;
            case "Launcher": return launcherIcon;
            default: return null;
        }
    }
}
