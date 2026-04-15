using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static Tile;

public class GameBoardGenerator : MonoBehaviour
{
    [Header("GameBoard 타일 Condition 세팅")]
    [SerializeField] private int columns; // 가로 타일 개수
    [SerializeField] private int rows; // 세로 타일 개수
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private bool alignBoardToCamera = true;

    [Header("AR Plane Manager")]
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Chaos Gate")]
    [SerializeField] private GameObject chaosGate;

    /// ================================================================= ///
    public bool IsGameBoardGenerated { get; private set; } = false;
    public GameObject GameBoard { get; private set; } = null;
    public ARPlane PlaneOfGameBoard { get; private set; } = null;

    private float MinWidthOfPlane => columns * TILE_SIZE;
    private float MinHeightOfPlane => rows * TILE_SIZE;

    private void OnEnable()
    {
        planeManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    private void OnDisable()
    {
        planeManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (IsGameBoardGenerated) return;

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
        IsGameBoardGenerated = true;
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

        // 3. Chaos Gate를 GameBoard에 맞추어 생성한다.
        // PlaceChaosGate(halfD);

        // 4. GameBoard를 다 생성했으므로 Plane Tracking을 정리한다.
        SetAllPlanesVisible(false);
        planeManager.enabled = false;

        Debug.Log($"[GameBoardGenerator] GameBoard 생성: {columns} x {rows} 타일 그리드, " +
                  $"타일 크기 {TILE_SIZE * 100f:F1}cm, " +
                  $"총 보드 크기 {MinWidthOfPlane * 100f:F1}cm x {MinHeightOfPlane * 100f:F1}cm");
    }

    private void PlaceChaosGate(float halfD)
    {
        // Vector3 gateLocalPos = new Vector3(0f, 0f, halfD);
        GameObject gate = Instantiate(chaosGate, GameBoard.transform);
    }

    private void SetAllPlanesVisible(bool visible)
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(visible);
            Debug.Log($"{plane.name}");    
        }
        
    }
}