using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // This allows the GameManager object to be accessed globally, ensuring that only one instance exists.
    // This is the Instance variable. For more details, search for the 'Singleton Pattern'!
    public static GameManager Instance { get; private set; }

    [Header("Game State Change Events")]
    public event Action IsGameStateBeforeGateOpen; // Event called when the state is before the gate opens (maintenance time)
    public event Action OnGameOver;                // Event called when the game over state is reached
    public event Action OnGameClear;               // Event called when the game clear state is reached
    public event Action OnAgeChanged;              // Event called when the age changes

    private const int MAX_WAVE = 9; // The maximum number of waves in the game (Phase 3 boss = final wave)

    private GameFlowState currentState; // The current progress state of the game
    private GameAge currentAge;         // The current age of the game

    // It seems we can create waves and add "additional gold weight per wave", "additional monster attack power and health", and "age changes".
    private int wave = 0; // current wave
    // Property representing the current wave. When the value changes, the game state (Normal/Elite/Boss) also changes.
    public int Wave
    {
        get
        {
            return wave;
        }

        set
        {
            wave = value;
            if (wave == 0) return; // Prevents the boss phase from starting when wave is 0, as 0 % 3 == 0

            // Made it 1 phase per 3 waves.
            // Created using the remainder of 3. [kwj]
            if (wave % 3 == 1)
            {
                CurrentState = GameFlowState.NORMAL_MONSTER_SPAWN;
            }
            else if (wave % 3 == 2) {
                CurrentState = GameFlowState.ELITE_MONSTER_SPAWN;
            }
            else if (wave % 3 == 0) {
                CurrentState = GameFlowState.BOSS_MONSTER_SPAWN;
            }

            Debug.Log($"{currentState}, {wave}");
        }
    }
    
    // Made 'phase' a property to be calculated automatically based on the 'wave' value, instead of manually incrementing it (perfectly solves sync issues)
    public int Phase
    {
        get
        {
            return (wave == 0) ? 1 : ((wave - 1) / 3) + 1;
        }
    }

    // Variables for tracking the number of monsters and waiting for clear status
    public int aliveMonsterCount = 0;      // Number of monsters currently alive on the map
    public bool IsWaitingForClear = false; // Whether it's in a waiting state before moving to the next phase after defeating a boss

    // Increases the number of living monsters by 1. (Called on monster spawn)
    public void AddMonster()
    {
        aliveMonsterCount++;
    }

    // Decreases the number of living monsters by 1. (Called on monster death)
    public void RemoveMonster()
    {
        aliveMonsterCount--;
        // If all monsters are eliminated, it attempts to proceed to the next stage.
        if (aliveMonsterCount <= 0)
        {
            aliveMonsterCount = 0;
            TryAdvanceAfterBoss();
        }
    }

    // Checks the conditions to proceed to the next stage after clearing the boss and proceeds.
    // (Transitions to the next phase when in IsWaitingForClear state and there are no living monsters)
    public void TryAdvanceAfterBoss()
    {
        if (!IsWaitingForClear || aliveMonsterCount > 0) return;
        IsWaitingForClear = false;

        if (wave >= MAX_WAVE)
        {
            CurrentState = GameFlowState.GAME_CLEAR;
            return;
        }

        // Current Phase complete -> Switch to the next age and then to the preparation stage
        switch (Phase)
        {
            case 1: CurrentAge = GameAge.MODERN_AGE; break;
            case 2: CurrentAge = GameAge.FUTURE_AGE;  break;
        }

        CurrentState = GameFlowState.BEFORE_GATE_OPEN;
    }
    
    // When you want to reference currentState from an external script, you can reference the CurrentState property.
    // Example: When the number of normal monsters in EnemySpawner becomes 0
    // GameManager.Instance.CurrentState = GameFlowState.BOSS_MONSTER_SPAWN
    // You can write logic to transition to the boss monster phase like this.
    // You can also write the logic to conditionally set currentState in this property.
    public GameFlowState CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            currentState = value;
            
            if (currentState == GameFlowState.BEFORE_GATE_OPEN)
            {
                IsGameStateBeforeGateOpen?.Invoke();
            }

            if (currentState == GameFlowState.GAME_OVER)
            {
                OnGameOver?.Invoke();
            }

            if (currentState == GameFlowState.GAME_CLEAR)
            {
                OnGameClear?.Invoke();
            }

            Debug.Log($"{currentState}, {wave}");
        }
    }

    public GameAge CurrentAge
    {
        get
        {
            return currentAge;
        }

        set
        {
            currentAge = value;
            OnAgeChanged?.Invoke();
            Debug.Log($"{CurrentAge}");
        }
    }

    private void Awake()
    {
        // Lines 31 to 38 are also related to the Singleton pattern.
        // Ensures that only one GameManager object exists and is not destroyed when the scene changes.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // When starting a game completely from the beginning, it starts in the GAME_START state.
        CurrentState = GameFlowState.GAME_START;

        // When starting a game completely from the beginning, it starts in the MIDDLE_AGE state.
        CurrentAge = GameAge.MIDDLE_AGE;
    }

    // This method is called to restart the game from the beginning and initialize the scene.
    // (Call from UI button events or game over restart logic)
    public void RestartGame()
    {
        // 1. Initialize game manager variables
        wave = 0;
        aliveMonsterCount = 0;
        IsWaitingForClear = false;
        CurrentAge = GameAge.MIDDLE_AGE;
        CurrentState = GameFlowState.GAME_START;

        // Add gold reset logic for GoldManager
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.ResetGold();
        }

        // 2. Reload the currently active scene to delete all maps, turrets, monsters, etc., and return to the initial state
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Handles subscription/unsubscription for the scene loaded event.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Handles subscription/unsubscription for the scene loaded event.
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the state is GAME_START after the scene has finished loading, it's set to proceed to the next phase
        // (Because the GameManager that survives with DontDestroyOnLoad does not have Start() called again on restart)
        if (currentState == GameFlowState.GAME_START)
        {
            CurrentState = GameFlowState.BEFORE_GATE_OPEN;
        }
    }

    private void Start()
    {
        if (currentState == GameFlowState.GAME_START)
        {
            // ...

            // After finishing the processing for the GAME_START phase in Start,
            // it transitions to the BEFORE_GATE_OPEN phase. The game officially begins.
            // Using gold, placing turrets, etc...
            CurrentState = GameFlowState.BEFORE_GATE_OPEN;
        }
    }

    private void Update()
    {
        // Refactoring ::
        // Instead of continuously updating currentState in Update, it's now updated only when the wave value changes.
        // This logic is implemented in the set block of the Wave property that focuses on the wave variable!! Please refer to it.

        if (currentState == GameFlowState.BEFORE_GATE_OPEN)
        {
            // ...
        }

        if (currentState == GameFlowState.NORMAL_MONSTER_SPAWN)
        {
            // ...
        }

        if (currentState == GameFlowState.BOSS_MONSTER_SPAWN)
        {
            // ...
        }

        if (currentState == GameFlowState.GAME_OVER)
        {
            // ...
        }
    }
}
