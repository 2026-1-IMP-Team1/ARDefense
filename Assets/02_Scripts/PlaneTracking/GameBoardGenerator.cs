using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using static Tile;
using TMPro;
using System.Collections;

public class GameBoardGenerator : MonoBehaviour
{
    [Header("GameBoard Tile Condition Settings")]
    [SerializeField] private int columns; // Number of tiles horizontally
    [SerializeField] private int rows;    // Number of tiles vertically
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private bool alignBoardToCamera = true;

    [Header("AR Plane Manager")]
    [SerializeField] private ARPlaneManager planeManager;

    [Header("AR Session")]
    [SerializeField] private ARSession arSession;

    [Header("Chaos Gate")]
    [SerializeField] private GameObject chaosGate;

    [Header("Game Board Set UI")]
    [SerializeField] private GameObject gameBoardSetUI;
    [SerializeField] private GameObject window;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private TextMeshProUGUI findPlaceText;

    [Header("Player")]
    [SerializeField] private GameObject player;

    /// ================================================================= ///
    public bool IsGameBoardGenerated { get; private set; } = false;
    public GameObject GameBoard { get; private set; } = null;
    public ARPlane PlaneOfGameBoard { get; private set; } = null;

    private GameObject chaosGateInstance;

    private float MinWidthOfPlane => columns * TILE_SIZE; // Minimum horizontal length required to generate the game board
    private float MinHeightOfPlane => rows * TILE_SIZE;   // Minimum vertical length required to generate the game board
    private bool isGameBoardSet; // Whether the player has confirmed the selection of the game board
    private bool hasUserSelected; // Whether the player has clicked Yes or No
    private bool isWaitingUserSelect; // Whether the system is currently waiting for player input

    private Coroutine dotAnimCoroutine; // Coroutine for the "Searching for plane..." text dot animation
    private string findPlaceBaseText;   // Variable to store the original base text (excluding dots)

    private void OnEnable()
    {
        planeManager.trackablesChanged.AddListener(OnTrackablesChanged);
        gameBoardSetUI.SetActive(true);
        window.SetActive(false);

        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);

        if (findPlaceText != null) findPlaceBaseText = findPlaceText.text;
        StartDotAnimation();
    }

    private void OnDisable()
    {
        planeManager.trackablesChanged.RemoveListener(OnTrackablesChanged);

        yesButton.onClick.RemoveListener(Yes);
        noButton.onClick.RemoveListener(No);

        StopDotAnimation();
    }

    /// <summary>
    /// Starts the dot animation where "." characters appear sequentially in the text.
    /// </summary>
    private void StartDotAnimation()
    {
        if (findPlaceText == null) return;
        StopDotAnimation();
        findPlaceText.gameObject.SetActive(true);
        dotAnimCoroutine = StartCoroutine(AnimateDots());
    }

    /// <summary>
    /// Stops the dot animation coroutine and hides the text.
    /// </summary>
    private void StopDotAnimation()
    {
        if (dotAnimCoroutine != null)
        {
            StopCoroutine(dotAnimCoroutine);
            dotAnimCoroutine = null;
        }
        if (findPlaceText != null) findPlaceText.gameObject.SetActive(false);
    }

    IEnumerator AnimateDots()
    {
        string[] dots = { ".", "..", "..." };
        int index = 0;
        while (true)
        {
            findPlaceText.text = findPlaceBaseText + dots[index];
            index = (index + 1) % dots.Length;
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Event callback that checks if a board can be placed whenever AR planes are added or updated.
    /// </summary>
    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (IsGameBoardGenerated || isWaitingUserSelect) return;

        // Retry Bug fix
        // Only place the GameBoard once AR tracking has fully stabilized.
        // If a plane is detected while the coordinate system is still unstable immediately after a reset, 
        //a bug occurs where the GameBoard gets stuck to the camera and follows it.
        if (ARSession.state != ARSessionState.SessionTracking) return;

        foreach (ARPlane plane in args.added)
        {
            if (IsValidPlaneToPlaceGameBoard(plane)) return;
        }
        foreach (ARPlane plane in args.updated)
        {
            if (IsValidPlaneToPlaceGameBoard(plane)) return;
        }
    }

    /// <summary>
    /// Validates whether the detected plane meets the minimum size requirements for the game board.
    /// </summary>
    private bool IsValidPlaneToPlaceGameBoard(ARPlane plane)
    {
        if (plane.alignment != PlaneAlignment.HorizontalUp) return false;

        // True if Plane width >= Min width AND height >= Min height
        bool isFitNormal  = plane.size.x >= MinWidthOfPlane && plane.size.y >= MinHeightOfPlane;

        // True if Plane width >= Min height AND height >= Min width (rotated fit)
        bool isFitRotated = plane.size.x >= MinHeightOfPlane && plane.size.y >= MinWidthOfPlane;

        if (!isFitNormal && !isFitRotated) return false;

        // If a suitable plane is found, create the GameBoard on that plane
        PlaceGameBoard(plane);
        return true;
    }

    /// <summary>
    /// Instantiates and positions the game board (grid) and tile objects on the suitable plane.
    /// </summary>
    private void PlaceGameBoard(ARPlane plane)
    {
        isWaitingUserSelect = true;

        PlaneOfGameBoard = plane;

        GameBoard = new GameObject("GameBoard");
        Vector3 boardPos = plane.transform.position;

        // 1. Align the GameBoard relative to the camera direction (adjust Rotation), then position the GameBoard.
        Quaternion boardRot;
        if (alignBoardToCamera && Camera.main != null)
        {
            Vector3 camForwardFlat = Vector3.ProjectOnPlane(
                Camera.main.transform.forward, Vector3.up
            ).normalized;

            if (camForwardFlat.sqrMagnitude < 0.001f) camForwardFlat = Vector3.forward;

            boardRot = Quaternion.LookRotation(camForwardFlat, Vector3.up);
        }
        else
        {
            boardRot = Quaternion.Euler(0f, plane.transform.rotation.eulerAngles.y, 0f);
        }

        GameBoard.transform.SetPositionAndRotation(boardPos, boardRot);

        // 2. Generate the unit tile grid (Horizontal count: columns, Vertical count: rows)
        // Total GameBoard width: columns * TILE_SIZE
        // Total GameBoard height: rows * TILE_SIZE
        float halfW = (columns * TILE_SIZE) * 0.5f;
        float halfD = (rows    * TILE_SIZE) * 0.5f;
        float renderSize = TILE_SIZE;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                // Calculate tile center position in the GameBoard's local coordinate system
                Vector3 localPos = new Vector3(
                    -halfW + c * TILE_SIZE + TILE_SIZE * 0.5f,
                    0f,
                    -halfD + r * TILE_SIZE + TILE_SIZE * 0.5f
                );

                GameObject tile = Instantiate(tilePrefab, GameBoard.transform);
                tile.transform.localPosition = localPos;
                tile.transform.localRotation = Quaternion.identity;

                tile.transform.localScale = new Vector3(renderSize, 0.01f, renderSize);
                tile.name = $"Tile_{c}_{r}";

                Tile tileComponent = tile.AddComponent<Tile>();
                tileComponent.x = c;
                tileComponent.y = r;
            }
        }

        // Show selection UI to the user
        StartCoroutine(ShowGameBoardSelectUI());
    }


    /// <summary>
    /// Coroutine that controls the popup UI asking the player whether to use the currently placed game board.
    /// </summary>
    IEnumerator ShowGameBoardSelectUI()
    {
        hasUserSelected = false;
        isWaitingUserSelect = true;

        StopDotAnimation();
        window.SetActive(true);

        yield return new WaitUntil (() => hasUserSelected);

        // Logic after selection... 

        window.SetActive(false);

        if (isGameBoardSet)
        {
            // 3. Create Chaos Gate aligned with the GameBoard.
            // PlaceChaosGate(halfD);

            // 4. GameBoard generation complete, cleanup plane tracking.
            IsGameBoardGenerated = true;

            isWaitingUserSelect = false;

            gameBoardSetUI.SetActive(false);

            planeManager.enabled = false;

            Debug.Log($"[GameBoardGenerator] GameBoard Created: {columns} x {rows} tile grid, " +
                    $"Tile size {TILE_SIZE * 100f:F1}cm, " +
                    $"Total Board Size {MinWidthOfPlane * 100f:F1}cm x {MinHeightOfPlane * 100f:F1}cm");

            GameManager.Instance.CurrentState = GameFlowState.BEFORE_GATE_OPEN;

            SetPlayerPos();
            SetChaosGatePos();
        }
        else
        {
            IsGameBoardGenerated = false;

            Destroy(GameBoard);
            GameBoard = null;
            PlaneOfGameBoard = null;

            arSession.Reset();
            Debug.Log("[GameBoardGenerator] Restarting GameBoard search.");

            isWaitingUserSelect = false;
            StartDotAnimation();
        }
    }

    /// <summary>
    /// Positions the player (AR Camera proxy) at the left end of the board after confirmation.
    /// </summary>
    private void SetPlayerPos()
    {
        GameObject playerObj = Instantiate(player, GameBoard.transform, false);
        playerObj.transform.localPosition = new Vector3(-MinWidthOfPlane / 2.0f, 0, 0);

        // Add a tile under the player's feet so they don't appear to be floating.
        // No Tile component (cannot install turrets), Collider removed (no interference with touch detection).
        GameObject playerTile = Instantiate(tilePrefab, GameBoard.transform);
        playerTile.transform.localPosition = new Vector3(-MinWidthOfPlane / 2.0f - 0.05f, 0f, 0f);
        playerTile.transform.localRotation = Quaternion.identity;
        playerTile.transform.localScale    = new Vector3(TILE_SIZE, 0.01f, TILE_SIZE);
        playerTile.name = "Tile_Player";

        Collider playerTileCollider = playerTile.GetComponent<Collider>();
        if (playerTileCollider != null) Destroy(playerTileCollider);
    }

    /// <summary>
    /// Positions the monster spawn point (Chaos Gate) at the right end of the board after confirmation.
    /// </summary>
    private void SetChaosGatePos()
    {
        chaosGateInstance = Instantiate(chaosGate);
        GameObject gateObj = chaosGateInstance;

        Vector3 gateWorldPos   = GameBoard.transform.TransformPoint(new Vector3( MinWidthOfPlane / 2.0f, 0, 0));
        Vector3 playerWorldPos = GameBoard.transform.TransformPoint(new Vector3(-MinWidthOfPlane / 2.0f, 0, 0));

        gateObj.transform.position = gateWorldPos;

        Vector3 dirToPlayer = playerWorldPos - gateWorldPos;
        dirToPlayer.y = 0f;
        if (dirToPlayer.sqrMagnitude > 0.001f)
        {
            // The prefab root has a -90° Y offset baked in, so it is composited here.
            gateObj.transform.rotation = Quaternion.LookRotation(dirToPlayer, Vector3.up)
                                         * Quaternion.Euler(0, -90, 0);
        }

        EnemySpawner spawner = gateObj.GetComponent<EnemySpawner>();
        spawner.Initialize(this);
    }

    /// <summary>
    /// Event handler for the confirmation button (Yes).
    /// </summary>
    private void Yes()
    {
        isGameBoardSet = true;
        hasUserSelected = true;
    }

    /// <summary>
    /// Event handler for the cancellation button (No).
    /// </summary>
    private void No()
    {
        isGameBoardSet = false;
        hasUserSelected = true;
    }

    /// <summary>
    /// Called by GameManager.RestartGame(). Applies the same cleanup as the "No" flow:
    /// destroys all in-game objects and resets AR board search without reloading the scene.
    /// </summary>
    public void ResetForRestart()
    {
        // Destroy ChaosGate / EnemySpawner
        if (chaosGateInstance != null)
        {
            Destroy(chaosGateInstance);
            chaosGateInstance = null;
        }

        // Destroy all remaining monsters
        foreach (Monster monster in FindObjectsOfType<Monster>())
            Destroy(monster.gameObject);

        // Destroy GameBoard (also destroys tiles, turrets, player)
        IsGameBoardGenerated = false;

        if (GameBoard != null)
        {
            Destroy(GameBoard);
            GameBoard = null;
        }
        PlaneOfGameBoard = null;
        isGameBoardSet   = false;
        hasUserSelected  = false;
        isWaitingUserSelect = false;

        // Re-enable plane tracking and reset AR session — same as the "No" flow
        planeManager.enabled = true;
        arSession.Reset();

        // Restart the board-search UI
        gameBoardSetUI.SetActive(true);
        window.SetActive(false);
        StartDotAnimation();
    }
}