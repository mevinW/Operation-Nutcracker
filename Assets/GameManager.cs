// GameManager.cs
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // at top of class
    private Vector3[] teleporterPositions = new Vector3[2];

    public void SetTeleportPosition(int playerIndex, Vector3 pos)
    {
        teleporterPositions[playerIndex] = pos;
    }

    public Vector3 GetTeleportPosition(int playerIndex)
    {
        return teleporterPositions[playerIndex];
    }

    // Per‑player purchase histories
    public List<string> purchasedItemsOne = new List<string>();
    public List<string> purchasedItemsTwo = new List<string>();

    // Per‑player currency
    public int acornsCollectedOne = 0;
    public int acornsCollectedTwo = 0;

    // Drives which map loads after Shop
    public int currentShop = 0;

    public int winner = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---- Currency access ----
    public int GetAcorns(int playerIndex)
    {
        return (playerIndex == 0) ? acornsCollectedOne : acornsCollectedTwo;
    }

    public void SetAcorns(int playerIndex, int amount)
    {
        if (playerIndex == 0) acornsCollectedOne = amount;
        else acornsCollectedTwo = amount;
    }

    // ---- Purchase queries ----
    public bool IsPurchased(string itemID, int playerIndex)
    {
        return (playerIndex == 0)
            ? purchasedItemsOne.Contains(itemID)
            : purchasedItemsTwo.Contains(itemID);
    }

    public void AddPurchase(string itemID, int playerIndex)
    {
        var list = (playerIndex == 0) ? purchasedItemsOne : purchasedItemsTwo;
        if (!list.Contains(itemID))
            list.Add(itemID);
    }

    // —— New: per‑player equipped lists —— 
    public List<string> equippedItemsOne = new List<string>();
    public List<string> equippedItemsTwo = new List<string>();

    /// <summary>Get the equipped‑list for the given player</summary>
    private List<string> GetEquippedList(int playerIndex) =>
        playerIndex == 0 ? equippedItemsOne : equippedItemsTwo;

    /// <summary>Return whether that player has this item equipped</summary>
    public bool IsEquipped(string itemID, int playerIndex) =>
        GetEquippedList(playerIndex).Contains(itemID);

    /// <summary>Return how many items that player currently has equipped</summary>
    public int EquippedCount(int playerIndex) =>
        GetEquippedList(playerIndex).Count;

    /// <summary>Try to equip: only if purchased and under the 3‑item cap</summary>
    public void Equip(string itemID, int playerIndex)
    {
        var eq = GetEquippedList(playerIndex);
        if (!eq.Contains(itemID)
            && IsPurchased(itemID, playerIndex)
            && eq.Count < 3)
        {
            eq.Add(itemID);
        }
    }

    public void ResetGameState()
    {
        // teleporters
        teleporterPositions = new Vector3[2];   // zeroed by default

        // purchases & equipment
        purchasedItemsOne.Clear();
        purchasedItemsTwo.Clear();
        equippedItemsOne.Clear();
        equippedItemsTwo.Clear();

        // currency
        acornsCollectedOne = 0;
        acornsCollectedTwo = 0;

        // shop progression
        currentShop = 1;

        // if you have other fields (lives, score, level flags, etc.), reset them here too:
        // playerLives = 3;
        // currentScore = 0;
        // levelsUnlocked = new bool[10]{ true, false, … };
    }

    /// <summary>Unequip if currently equipped</summary>
    public void Unequip(string itemID, int playerIndex)
    {
        GetEquippedList(playerIndex).Remove(itemID);
    }

    /// <summary>Expose a copy of the equipped list</summary>
    public List<string> GetEquippedItems(int playerIndex) =>
        new List<string>(GetEquippedList(playerIndex));
}