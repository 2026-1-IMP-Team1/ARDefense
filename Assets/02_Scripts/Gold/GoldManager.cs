using System;
using UnityEngine;
using static Gold;

public class GoldManager : MonoBehaviour
{
    // Singleton Instance: Allows access to GoldManager.Instance from anywhere.
    public static GoldManager Instance { get; private set; }

    [Tooltip("Event-related settings")]
    // Event triggered when the current gold amount changes
    public event Action OnGoldChanged;
    // Event triggered when there is insufficient gold for a transaction
    public event Action OnGoldInsufficient;
    

    /// <summary>
    /// Resets the gold amount to the default starting value.
    /// </summary>
    public void ResetGold()
    {
        Gold = GAME_START_GOLD;
        OnGoldChanged?.Invoke(); // Trigger event to update UI, etc.
    }

    [Header("Gold Data Management")]

    private int currentGold;

    // Gold Property: Automatically triggers an event whenever the value is modified.
    public int Gold
    {
        get
        {
            return currentGold;
        }

        set
        {
            currentGold = value;
            OnGoldChanged?.Invoke(); // Execute UI refresh event whenever value is updated
        }
    }

    void Awake()
    {
        // Singleton Pattern Implementation: Ensures only one instance exists by destroying duplicates.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Keep the object alive when switching between scenes
        DontDestroyOnLoad(gameObject);

        // Set initial starting gold
        currentGold = GAME_START_GOLD;
    }

    /// <summary>
    /// Adds a specified amount of gold.
    /// </summary>
    /// <param name="goldToAdd">Amount of gold to add</param>
    public void AddGold(int goldToAdd)
    {
        // Logic for adding gold
        Gold += goldToAdd;
        
        // Debug log to track income and current balance
        Debug.Log($"{goldToAdd} Gold acquired. Current Gold: {Gold}");
    }

    /// <summary>
    /// Spends a specified amount of gold. Returns whether the transaction was successful.
    /// </summary>
    /// <param name="goldToSpend">Amount of gold to spend</param>
    /// <returns>True if successful, false if insufficient funds</returns>
    public bool SpendGold(int goldToSpend)
    {
        // Check for invalid input values
        if (goldToSpend == -1)
        {
            Debug.Log("Accessed via an invalid gold consumption path.");
            return false;
        }

        // Check for insufficient balance
        if (currentGold < goldToSpend)
        {
            Debug.Log($"Insufficient gold: Current - {Gold} / Attempting to spend - {goldToSpend}");
            // Trigger insufficient gold event (e.g., to display an "Insufficient Funds" popup)
            OnGoldInsufficient?.Invoke();
            return false;
        }

        // Logic for deducting gold
        Gold -= goldToSpend;
        return true;
    }
}