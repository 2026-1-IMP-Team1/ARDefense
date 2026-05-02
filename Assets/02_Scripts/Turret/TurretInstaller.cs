using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using static Tile;
using static TurretCost;

/// <summary>
/// TurretInstaller Class:
/// This class handles user touch input in an AR environment to interact with the game board.
/// It manages the logic for displaying placement silhouettes, installing turrets, 
/// and accessing turret/tile information through Raycasting.
/// </summary>
public class TurretInstaller : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // [Serialized Fields - References & Settings]
    // -------------------------------------------------------------------------
    
    [Header("Dependencies")]
    [SerializeField] private GameBoardGenerator gameBoardGenerator; // Reference to the generator that creates the AR grid

    [Tooltip("Main camera used for converting screen touch to world Ray")]
    [SerializeField] private Camera cam;

    [Header("Turret Prefabs (By Era)")]
    [SerializeField] private GameObject middleAgeTurretPrefab;
    [SerializeField] private GameObject modernAgeTurretPrefab;
    [SerializeField] private GameObject futureAgeTurretPrefab;

    [Header("Scale Settings")]
    [Tooltip("Adjusts the turret's size relative to the Tile size (0.9 means 90% of tile width)")]
    [SerializeField, Range(0.1f, 2f)] private float middleAgeTurretScaleValue = 0.9f;
    [SerializeField, Range(0.1f, 2f)] private float modernAgeTurretScaleValue  = 0.9f;
    [SerializeField, Range(0.1f, 2f)] private float futureAgeTurretScaleValue  = 0.9f;

    [Header("Visual Feedback (Silhouette)")]
    [SerializeField] private GameObject middleAgeSilhouettePrefab;
    [SerializeField] private GameObject modernAgeSilhouettePrefab;
    [SerializeField] private GameObject futureAgeSilhouettePrefab;

    private GameObject currentSilhouette; // Instance of the semi-transparent preview object

    [Header("Input Settings")]
    [SerializeField] private LayerMask tileLayerMask = ~0; // Layer mask to filter specific objects if needed

    // -------------------------------------------------------------------------
    // [Events & UI References]
    // -------------------------------------------------------------------------
    
    public event Action OnTurretAlreadyInstalled; // Notifies other systems when a user tries to build on an occupied tile

    [Header("UI Panels")]
    public GameObject PlantUI;          // UI for "Install Turret" action
    [SerializeField] private GameObject turretSelectUI; // UI for "Upgrade/Destroy" existing turret

    [Header("Turret Info Display")]
    [SerializeField] private UnityEngine.UI.Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI upgradeCountText;

    // -------------------------------------------------------------------------
    // [State Variables]
    // -------------------------------------------------------------------------
    
    private Tile tile;               // Currently hit tile via raycast
    private Turret selectedTurret;   // Currently selected turret for info/upgrade
    private Tile selectedTile;       // The tile occupied by the selected turret

    private void Awake()
    {
        // Fallback to Camera.main if a specific camera isn't assigned in the Inspector
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        // Safety: Do not process input if the AR board hasn't been placed yet
        if (!gameBoardGenerator.IsGameBoardGenerated) return;

        // Step 1: Detect the beginning of a touch/click
        if (!TryGetTouchBeganPosition(out Vector2 screenPos)) return;

        // Step 2: Prevent world interaction if the user is clicking a UI button
        if (IsPointerOverUI(screenPos)) return;

        // Step 3: Handle the interaction logic (Raycasting)
        HandleTileTouch(screenPos);
    }

    /// <summary>
    /// Uses GraphicRaycaster to check if the touch position overlaps with any Canvas UI elements.
    /// This prevents "click-through" where a player accidentally builds a turret while clicking a button.
    /// </summary>
    private bool IsPointerOverUI(Vector2 screenPos)
    {
        var eventData = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private void OnEnable()
    {
        // Initialize EnhancedTouch for improved input handling in Unity
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    /// <summary>
    /// Unifies mobile touch (EnhancedTouch) and Editor mouse clicks for easier debugging.
    /// </summary>
    private bool TryGetTouchBeganPosition(out Vector2 screenPos)
    {
        var touches = Touch.activeTouches;

        if (touches.Count > 0)
        {
            Touch touch = touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                screenPos = touch.screenPosition;
                return true;
            }
        }

#if UNITY_EDITOR
        // Support for PC Editor testing
        if (Input.GetMouseButtonDown(0))
        {
            screenPos = Input.mousePosition;
            return true;
        }
#endif
        screenPos = default;
        return false;
    }

    /// <summary>
    /// Primary interaction logic:
    /// 1. Shoots a Ray from the camera into the 3D world.
    /// 2. Identifies if the user hit a Turret (Select mode) or a Tile (Build mode).
    /// 3. Toggles the appropriate UI and preview silhouette.
    /// </summary>
    private void HandleTileTouch(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            CloseBothUI(); // Close all windows if user clicks empty space
            return;
        }

        // Logic Branch A: User touched a Turret object
        Turret hitTurret = hit.collider.GetComponentInParent<Turret>();
        if (hitTurret != null)
        {
            selectedTurret = hitTurret;
            selectedTile   = FindTileForTurret(hitTurret);
            RefreshTurretInfoUI(hitTurret);
            HideSilhouette();
            PlantUI.SetActive(false);
            turretSelectUI.SetActive(true);
            return;
        }

        // Logic Branch B: User touched a Tile object
        tile = hit.collider.GetComponent<Tile>();
        if (tile == null)
        {
            CloseBothUI();
            return;
        }

        // Branch B-1: Tile already has a turret (Alternative selection method)
        if (tile.IsTurretInstalled)
        {
            selectedTile   = tile;
            selectedTurret = tile.InstalledTurret != null
                ? tile.InstalledTurret.GetComponent<Turret>()
                : null;
            if (selectedTurret != null) RefreshTurretInfoUI(selectedTurret);
            HideSilhouette();
            PlantUI.SetActive(false);
            turretSelectUI.SetActive(selectedTurret != null);
            return;
        }

        // Branch B-2: Tile is empty (Open placement UI and show preview)
        turretSelectUI.SetActive(false);
        PlantUI.SetActive(true);
        ShowSilhouette(tile);
    }

    /// <summary>
    /// Searches through the GameBoard hierarchy to link a Turret back to its parent Tile.
    /// Necessary for clean destruction and tile state management.
    /// </summary>
    private Tile FindTileForTurret(Turret turret)
    {
        if (gameBoardGenerator.GameBoard == null) return null;
        foreach (Transform child in gameBoardGenerator.GameBoard.transform)
        {
            Tile t = child.GetComponent<Tile>();
            if (t != null && t.InstalledTurret == turret.gameObject)
                return t;
        }
        return null;
    }

    /// <summary>
    /// Updates the UI elements with current stats from the selected turret.
    /// Uses string interpolation and formatting (F0) for clean display.
    /// </summary>
    private void RefreshTurretInfoUI(Turret turret)
    {
        if (hpText           != null) hpText.text           = $"{turret.HP:F0} / {turret.MaxHP:F0}";
        if (damageText       != null) damageText.text       = $"{turret.AttackDamage:F0}";
        if (upgradeCountText != null) upgradeCountText.text = $" + {turret.UpgradeCount}";

        if (hpSlider != null)
        {
            hpSlider.minValue = 0f;
            hpSlider.maxValue = turret.MaxHP;
            hpSlider.value = turret.HP;
        }
    }

    private void CloseBothUI()
    {
        PlantUI.SetActive(false);
        turretSelectUI.SetActive(false);
        HideSilhouette();
    }

    // Called by GameUIManager on restart to close panels and clear dangling references.
    public void ResetState()
    {
        CloseBothUI();
        tile           = null;
        selectedTurret = null;
        selectedTile   = null;
        currentSilhouette = null; // already destroyed with GameBoard
    }

    /// <summary>
    /// Visual Preview: Creates a semi-transparent version of the turret on the target tile.
    /// Uses 'transform.up' to ensure correct vertical placement in AR space.
    /// </summary>
    private void ShowSilhouette(Tile targetTile)
    {
        HideSilhouette();

        GameObject prefab = GetSilhouettePrefabForCurrentAge();
        if (prefab == null || gameBoardGenerator.GameBoard == null) return;

        float turretSize      = TILE_SIZE * GetTurretScaleForCurrentAge();
        float tileHalfHeight  = targetTile.transform.lossyScale.y * 0.5f;
        float turretHalfHeight = turretSize * 0.5f;
        
        // Spawn Position Calculation: Tile center + Upwards offset based on heights
        Vector3 spawnPos = targetTile.transform.position + targetTile.transform.up * (tileHalfHeight + turretHalfHeight);
        Quaternion boardRot = gameBoardGenerator.GameBoard.transform.rotation;

        currentSilhouette = Instantiate(prefab, spawnPos, boardRot);
        currentSilhouette.transform.localScale = Vector3.one * turretSize;
        
        // Parent to GameBoard for consistent coordinate space
        currentSilhouette.transform.SetParent(gameBoardGenerator.GameBoard.transform, worldPositionStays: true);
    }

    private void HideSilhouette()
    {
        if (currentSilhouette == null) return;
        Destroy(currentSilhouette);
        currentSilhouette = null;
    }

    // [Helper Functions] Era-based logic for Prefabs, Scale, and Cost
    private GameObject GetSilhouettePrefabForCurrentAge()
    {
        return GameManager.Instance.CurrentAge switch
        {
            GameAge.MIDDLE_AGE => middleAgeSilhouettePrefab,
            GameAge.MODERN_AGE => modernAgeSilhouettePrefab,
            GameAge.FUTURE_AGE => futureAgeSilhouettePrefab,
            _                  => middleAgeSilhouettePrefab
        };
    }

    private GameObject GetTurretPrefabForCurrentAge()
    {
        return GameManager.Instance.CurrentAge switch
        {
            GameAge.MIDDLE_AGE => middleAgeTurretPrefab,
            GameAge.MODERN_AGE => modernAgeTurretPrefab,
            GameAge.FUTURE_AGE => futureAgeTurretPrefab,
            _                  => middleAgeTurretPrefab
        };
    }

    private float GetTurretScaleForCurrentAge()
    {
        return GameManager.Instance.CurrentAge switch
        {
            GameAge.MIDDLE_AGE => middleAgeTurretScaleValue,
            GameAge.MODERN_AGE => modernAgeTurretScaleValue,
            GameAge.FUTURE_AGE => futureAgeTurretScaleValue,
            _                  => middleAgeTurretScaleValue
        };
    }

    private int MeasureTurretCost()
    {
        switch (GameManager.Instance.CurrentAge)
        {
            case GameAge.MIDDLE_AGE : return MIDDLE_AGE_TURRET_COST;
            case GameAge.MODERN_AGE : return MODERN_AGE_TURRET_COST;
            case GameAge.FUTURE_AGE : return FUTURE_AGE_TURRET_COST;
            default : return -1;
        }
    }

    public void UpgradeTurret()
    {
        if (selectedTurret == null) return;
        
        // Gold Management Integration: Verify funds before upgrading
        if (!GoldManager.Instance.SpendGold(TURRET_UPGRADE_COST)) return;

        selectedTurret.Upgrade();
        RefreshTurretInfoUI(selectedTurret);
    }

    public void DestroyTurret()
    {
        if (selectedTurret == null) return;

        // Clean up the Tile state so a new turret can be built here later
        if (selectedTile != null)
        {
            selectedTile.IsTurretInstalled = false;
            selectedTile.InstalledTurret   = null;
        }

        Destroy(selectedTurret.gameObject);
        selectedTurret = null;
        selectedTile   = null;
        turretSelectUI.SetActive(false);
    }

    /// <summary>
    /// Finalizes the installation of a turret. 
    /// Includes coordinate alignment for AR and handles parenting logic 
    /// to avoid non-uniform scale distortion inherited from tiles.
    /// </summary>
    public void InstallTurret()
    {
        if (tile == null) return;

        // Occupancy Check
        if (tile.IsTurretInstalled)
        {
            OnTurretAlreadyInstalled?.Invoke();
            return;
        }

        // Fund Check
        if (!GoldManager.Instance.SpendGold(MeasureTurretCost())) return;

        float turretSize = TILE_SIZE * GetTurretScaleForCurrentAge();
        float tileHalfHeight   = tile.transform.lossyScale.y * 0.5f;
        float turretHalfHeight = turretSize * 0.5f;

        // Position: Place the turret exactly on top of the tile surface
        Vector3 spawnPos = tile.transform.position + tile.transform.up * (tileHalfHeight + turretHalfHeight);

        // Rotation: Align with the GameBoard's rotation
        Quaternion boardRotation = gameBoardGenerator.GameBoard != null
            ? gameBoardGenerator.GameBoard.transform.rotation
            : Quaternion.identity;

        GameObject turretPrefab = GetTurretPrefabForCurrentAge();
        if (turretPrefab == null) return;

        GameObject turret = Instantiate(turretPrefab, spawnPos, boardRotation);
        turret.transform.localScale = Vector3.one * turretSize;

        // IMPORTANT: Parenting Logic
        // We parent to 'GameBoard' rather than 'Tile' because tiles often have non-uniform scales 
        // (e.g., flattened Y for the grid). If we parented to Tile, the Turret would inherit 
        // that distortion and appear flattened or stretched.
        Transform parentTransform = gameBoardGenerator.GameBoard != null
            ? gameBoardGenerator.GameBoard.transform
            : tile.transform.parent;

        turret.transform.SetParent(parentTransform, worldPositionStays: true);
        turret.name = $"Turret_{tile.x}_{tile.y}";

        // Update Tile state
        tile.IsTurretInstalled = true;
        tile.InstalledTurret = turret;

        HideSilhouette();
        PlantUI.SetActive(false);
    }
}