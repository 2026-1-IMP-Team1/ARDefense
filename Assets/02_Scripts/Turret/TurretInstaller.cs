using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using static Tile;
using static TurretCost;

public class TurretInstaller : MonoBehaviour
{
    [Header("Game Board Generator")]
    [SerializeField] private GameBoardGenerator gameBoardGenerator;

    [Tooltip("AR Camera")]
    [SerializeField] private Camera cam;

    [Header("Turret Prefab")]
    [SerializeField] private GameObject turretPrefab;

    [Header("포탑 스케일 배율 (1이면 tileSize와 동일한 크기로 설정)")]
    [SerializeField, Range(0.5f, 1f)] private float turretScaleValue = 0.9f;

    [Header("타일 Layer Mask (특정 레이어만 터치 감지할 때 설정)")]
    [SerializeField] private LayerMask tileLayerMask = ~0;

    //포탑 설치 UI 저장 변수[lyh]
    public GameObject PlantUI;

    // 터치한 타일 정보 저장 변수[lyh]
    Tile tile;
    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        // GameBoard가 생성되기 전에는 터치 입력을 받지 않습니다.
        if (!gameBoardGenerator.IsGameBoardGenerated) return;

        if (!TryGetTouchBeganPosition(out Vector2 screenPos)) return;
        HandleTileTouch(screenPos);
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

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
        if (Input.GetMouseButtonDown(0))
        {
            screenPos = Input.mousePosition;
            return true;
        }
#endif
        screenPos = default;
        return false;
    }

    private void HandleTileTouch(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayerMask)) return;

        // hit한 collider가 Tile인지 확인합니다.
        tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        // Tile이여도 이미 포탑이 설치된 tile이면 무시합니다.
        if (tile.IsTurretInstalled)
        {
            Debug.Log($"[TurretInstaller] Tile ({tile.x}, {tile.y})에는 이미 포탑이 설치되어 있습니다.");
            return;
        }

        // 포탑 설치 UI 열기[lyh]
        PlantUI.SetActive(true);
    }

    private int MeasureTurretCost()
    {
        switch (GameManager.Instance.CurrentAge)
        {
            case GameAge.MIDDLE_AGE :
                return MIDDLE_AGE_TURRET_COST;
            
            case GameAge.MODERN_AGE :
                return MODERN_AGE_TURRET_COST;
            
            case GameAge.FUTURE_AGE :
                return FUTURE_AGE_TURRET_COST;

            default :
                return -1;
        }
    }

    public void InstallTurret()
    {
        
        // ── 골드 차감 ───────────────────────────────────────[lyh]
        if (!GoldManager.Instance.SpendGold(MeasureTurretCost())) return;

        // ── 포탑 월드 크기 계산 ──────────────────────────────
        float turretSize = TILE_SIZE * turretScaleValue;

        // ── 위치 계산 ────────────────────────────────────────
        // lossyScale: 부모 포함 실제 월드 스케일
        // tile.transform.up: AR 환경에서 보드가 기울어져도 올바른 방향 유지
        float tileHalfHeight  = tile.transform.lossyScale.y * 0.5f;
        float turretHalfHeight = turretSize * 0.5f; // 피벗이 오브젝트 중심에 있다고 가정

        Vector3 spawnPos = tile.transform.position
                        + tile.transform.up * (tileHalfHeight + turretHalfHeight);

        // ── 회전 계산 ────────────────────────────────────────
        Quaternion boardRotation = gameBoardGenerator.GameBoard != null
            ? gameBoardGenerator.GameBoard.transform.rotation
            : Quaternion.identity;

        // ── 포탑 생성 ────────────────────────────────────────
        GameObject turret = Instantiate(turretPrefab, spawnPos, boardRotation);
        turret.transform.localScale = Vector3.one * turretSize;

        // ── 비균등 스케일 상속 방지: Tile이 아닌 GameBoard의 자식으로 설정 ──
        // Tile의 scale이 (0.1, 0.01, 0.1)처럼 비균등하면
        // SetParent 시 Unity가 world scale 유지를 위해 local scale을 역산해 Y가 뻥튀기됨
        Transform parentTransform = gameBoardGenerator.GameBoard != null
            ? gameBoardGenerator.GameBoard.transform
            : tile.transform.parent;

        turret.transform.SetParent(parentTransform, worldPositionStays: true);
        turret.name = $"Turret_{tile.x}_{tile.y}";

        // ── 타일 상태 업데이트 ───────────────────────────────
        tile.IsTurretInstalled = true;
        tile.InstalledTurret = turret;

        Debug.Log($"[TurretInstaller] 포탑 설치됨: Tile ({tile.x}, {tile.y}), " +
                $"Position {spawnPos}, Scale {turretSize:F3}m");
    }
}