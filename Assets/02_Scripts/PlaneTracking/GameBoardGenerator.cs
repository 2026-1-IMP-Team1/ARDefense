using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using static Tile;
using TMPro;
using System.Collections;

public class GameBoardGenerator : MonoBehaviour
{
    [Header("GameBoard 타일 Condition 세팅")]
    [SerializeField] private int columns; // 가로 타일 개수
    [SerializeField] private int rows; // 세로 타일 개수
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

    private float MinWidthOfPlane => columns * TILE_SIZE;
    private float MinHeightOfPlane => rows * TILE_SIZE;
    private bool isGameBoardSet; // 플레이어가 플레이할 게임 보드 선택했는지 유무
    private bool hasUserSelected; // 플레이어가 Yes, NO 선ㅇ택했는지 유무
    private bool isWaitingUserSelect; // 플레이어가 선택하는 중인지 유무 (사실 !hasUserSelected긴 합니다. )

    private Coroutine dotAnimCoroutine;
    private string findPlaceBaseText;

    private void OnEnable()
    {
        arSession.Reset();

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

    private void StartDotAnimation()
    {
        if (findPlaceText == null) return;
        StopDotAnimation();
        findPlaceText.gameObject.SetActive(true);
        dotAnimCoroutine = StartCoroutine(AnimateDots());
    }

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

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (IsGameBoardGenerated || isWaitingUserSelect) return;

        foreach (ARPlane plane in args.added)
        {
            if (IsValidPlaneToPlaceGameBoard(plane)) return;
        }
        foreach (ARPlane plane in args.updated)
        {
            if (IsValidPlaneToPlaceGameBoard(plane)) return;
        }
    }

    private bool IsValidPlaneToPlaceGameBoard(ARPlane plane)
    {
        if (plane.alignment != PlaneAlignment.HorizontalUp) return false;

        // 찾은 Plane의 가로 >= 최소 가로 길이, 세로 >= 최소 세로 길이일 때 true
        bool isFitNormal  = plane.size.x >= MinWidthOfPlane && plane.size.y >= MinHeightOfPlane;

        // 찾은 Plane의 가로 >= 최소 세로 길이, 세로 >= 최소 가로 길이일 때도 상관 없으므로 true
        bool isFitRotated = plane.size.x >= MinHeightOfPlane && plane.size.y >= MinWidthOfPlane;

        if (!isFitNormal && !isFitRotated) return false;

        // 적합한 Plane을 찾았으면 해당 Plane에 GameBoard 생성
        PlaceGameBoard(plane);
        return true;
    }

    private void PlaceGameBoard(ARPlane plane)
    {
        isWaitingUserSelect = true;

        PlaneOfGameBoard = plane;

        GameBoard = new GameObject("GameBoard");
        Vector3 boardPos = plane.transform.position;

        // 1. 카메라가 있는 방향 기준으로 GameBoard를 정렬(Rotation 조정) 후, GameBoard를 위치시킨다.
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

        // 2. 단위 타일 그리드를 생성한다. (가로 개수: columns, 세로 개수 : rows)
        // 생성된 GameBoard의 가로 길이 : columns * TILE_SIZE
        // 생성된 GameBoard의 세로 길이 : rows * TILE_SIZE
        float halfW = (columns * TILE_SIZE) * 0.5f;
        float halfD = (rows    * TILE_SIZE) * 0.5f;
        float renderSize = TILE_SIZE;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                // GameBoard의 로컬 좌표계에서 타일 중심 위치를 계산
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

        // 유저에게 UI 띄우기
        StartCoroutine(ShowGameBoardSelectUI());
    }

    /*private void PlaceChaosGate(float halfD)
    {
        // Vector3 gateLocalPos = new Vector3(0f, 0f, halfD);
        GameObject gate = Instantiate(chaosGate, GameBoard.transform);
    }*/

    /*private void SetAllPlanesVisible(bool visible)
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(visible);
            Debug.Log($"{plane.name}");    
        }
    }*/

    IEnumerator ShowGameBoardSelectUI()
    {
        hasUserSelected = false;
        isWaitingUserSelect = true;

        StopDotAnimation();
        window.SetActive(true);

        yield return new WaitUntil (() => hasUserSelected);

        // 선택 이후 .. 

        window.SetActive(false);

        if (isGameBoardSet)
        {
            // 3. Chaos Gate를 GameBoard에 맞추어 생성한다.
            // PlaceChaosGate(halfD);

            // 4. GameBoard를 다 생성했으므로 Plane Tracking을 정리한다.
            IsGameBoardGenerated = true;

            isWaitingUserSelect = false;

            gameBoardSetUI.SetActive(false);

            planeManager.enabled = false;

            Debug.Log($"[GameBoardGenerator] GameBoard 생성: {columns} x {rows} 타일 그리드, " +
                    $"타일 크기 {TILE_SIZE * 100f:F1}cm, " +
                    $"총 보드 크기 {MinWidthOfPlane * 100f:F1}cm x {MinHeightOfPlane * 100f:F1}cm");

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
            Debug.Log("[GameBoardGenerator] GameBoard를 다시 탐색합니다.");

            isWaitingUserSelect = false;
            StartDotAnimation();
        }
    }

    private void SetPlayerPos()
    {
        GameObject playerObj = Instantiate(player, GameBoard.transform, false);
        playerObj.transform.localPosition = new Vector3(-MinWidthOfPlane / 2.0f, 0, 0);
    }

    private void SetChaosGatePos()
    {
        GameObject gateObj = Instantiate(chaosGate);

        Vector3 gateWorldPos   = GameBoard.transform.TransformPoint(new Vector3( MinWidthOfPlane / 2.0f, 0, 0));
        Vector3 playerWorldPos = GameBoard.transform.TransformPoint(new Vector3(-MinWidthOfPlane / 2.0f, 0, 0));

        gateObj.transform.position = gateWorldPos;

        Vector3 dirToPlayer = playerWorldPos - gateWorldPos;
        dirToPlayer.y = 0f;
        if (dirToPlayer.sqrMagnitude > 0.001f)
        {
            // 프리팹 루트에 -90° Y 오프셋이 베이크되어 있어 함께 합성
            gateObj.transform.rotation = Quaternion.LookRotation(dirToPlayer, Vector3.up)
                                         * Quaternion.Euler(0, -90, 0);
        }

        EnemySpawnerClone spawner = gateObj.GetComponent<EnemySpawnerClone>();
        spawner.Initialize(this);
    }

    private void Yes()
    {
        isGameBoardSet = true;
        hasUserSelected = true;
    }

    private void No()
    {
        isGameBoardSet = false;
        hasUserSelected = true;
    }
}