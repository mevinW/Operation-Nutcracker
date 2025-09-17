using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopImageToggle : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [Tooltip("The small shop icon you click on")]
    [SerializeField] private Image iconImage;

    [Tooltip("The full‑screen (or larger) image that starts hidden")]
    [SerializeField] private Image enlargedImage;

    [Header("Per‑Shop Sprite (one sprite used for both)")]
    [Tooltip("Index 0 → shop 1, index 1 → shop 2, etc.")]
    [SerializeField] private Sprite[] shopSprites;

    private bool isOpen = false;

    void Start()
    {
        RefreshForCurrentShop();
        enlargedImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Syncs both icon and enlarged image to the currentShop sprite.
    /// </summary>
    public void RefreshForCurrentShop()
    {
        int shop = GameManager.Instance.currentShop;   // 1‑based
        int idx = shop - 1;                          // convert to 0‑based

        if (idx < 0 || idx >= shopSprites.Length)
        {
            Debug.LogWarning($"No sprite configured for shop {shop}");
            return;
        }

        var s = shopSprites[idx];
        iconImage.sprite = s;
        enlargedImage.sprite = s;
    }

    /// <summary>
    /// Toggle enlargedImage on click.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        isOpen = !isOpen;
        enlargedImage.gameObject.SetActive(isOpen);

        if (isOpen)
            enlargedImage.transform.SetAsLastSibling(); // optional: bring to top in this Canvas
    }
}
