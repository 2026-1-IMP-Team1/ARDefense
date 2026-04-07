using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using static Tile;

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
        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        // Tile이여도 이미 포탑이 설치된 tile이면 무시합니다.
        if (tile.IsTurretInstalled)
        {
            Debug.Log($"[TurretInstaller] Tile ({tile.x}, {tile.y})에는 이미 포탑이 설치되어 있습니다.");
            return;
        }

        InstallTurret(tile);
    }

    private void InstallTurret(Tile tile)
    {
        // 타워 설치 비용 계산
        // Turret 스크립트를 가져와서 turretCost를 확인합니다.
        Turret turretScript = turretPrefab.GetComponent<Turret>();
        // 타워 설치시, 골드 상태 파악
        if (GoldManager.Instance.Gold < turretScript.turretCost){
            if (GoldManager.Instance.Gold < turretScript.turretCost)
            {
                // 골드 부족 메시지 출력 (디버그 로그 또는 UI 팝업)
                // 설치 중단
                Debug.Log($"[TurretInstaller] 골드가 부족합니다. 현재 골드: {GoldManager.Instance.Gold}, 필요 골드: {turretScript.turretCost}");
                return;
            }
            GoldManager.Instance.SpendGold(turretScript.turretCost);
        }
        // 설치 골드 보유하고 있을 때 


        // ── 위치 계산 ────────────────────────────────────
        // 타일 표면 위에 배치 (타일 Y 스케일 = 0.002m이므로 0.001m 위)
        // 포탑 피벗이 하단 중심에 있다고 가정
        float tileHalfHeight = tile.transform.localScale.y * 0.5f;

        // turretPrefab의 localScale.y 값이 10배 뻥튀기되는 버그가 있어서
        // 일단은 0.05f로 상쇄시키는 하드 코딩을 해놓긴 했습니다 ㅠ
        // 나중에 진짜 포탑 에셋 사용하면서 해당 버그 해결해볼게요
        float turretHalfHeight = turretPrefab.transform.localScale.y * 0.05f;

        Vector3 spawnPos = tile.transform.position + Vector3.up * (tileHalfHeight + turretHalfHeight);

        // ── 회전 계산 ────────────────────────────────────
        // 보드의 Y 회전을 따라가도록 설정 (포탑이 보드 방향 기준으로 정렬)
        Quaternion boardRotation = gameBoardGenerator.GameBoard != null
            ? gameBoardGenerator.GameBoard.transform.rotation
            : Quaternion.identity;

        // ── 포탑 생성 ────────────────────────────────────
        GameObject turret = Instantiate(turretPrefab, spawnPos, boardRotation);

        // 포탑 스케일을 tileSize 기준으로 설정
        float turretSize = TILE_SIZE * turretScaleValue;
        turret.transform.localScale = Vector3.one * turretSize;

        // 포탑을 타일의 자식으로 설정 (보드 이동 시 함께 이동)
        turret.transform.SetParent(tile.transform, worldPositionStays: true);
        turret.name = $"Turret_{tile.x}_{tile.y}";

        // ── 타일 상태 업데이트 ───────────────────────────
        tile.IsTurretInstalled = true;
        tile.InstalledTurret = turret;

        Debug.Log($"[TurretInstaller] 포탑 설치됨: Tile ({tile.x}, {tile.y}), " +
                  $"Position {spawnPos}, Scale {turretSize:F3}m");
    }
}