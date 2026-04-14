using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch; // 추가됨
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

    [Header("포탑 스케일 배율 (1이면 TILE_SIZE와 동일)")]
    [SerializeField, Range(0.1f, 1.5f)] private float turretScaleValue = 0.9f;

    [Header("타일 Layer Mask")]
    [SerializeField] private LayerMask tileLayerMask = ~0;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    // EnhancedTouch 활성화
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
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

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        if (tile.IsTurretInstalled)
        {
            Debug.Log($"[TurretInstaller] 이미 설치됨: ({tile.x}, {tile.y})");
            return;
        }

        // 골드 소모 체크
        if (GoldManager.Instance != null)
        {
            if (!GoldManager.Instance.SpendGold(TurretCost.turretCost)) return;
        }

        InstallTurret(tile);
    }

    private void InstallTurret(Tile tile)
    {
        // 1. 부모(Tile)의 회전값 가져오기
        Quaternion boardRotation = gameBoardGenerator.GameBoard != null
            ? gameBoardGenerator.GameBoard.transform.rotation
            : Quaternion.identity;

        // 2. 포탑 생성 (일단 위치는 타일 중심으로)
        GameObject turret = Instantiate(turretPrefab, tile.transform.position, boardRotation);
        
        // 3. 부모 설정 (스케일 왜곡 방지를 위해 false 권장 혹은 설정 후 스케일 재조정)
        turret.transform.SetParent(tile.transform);
        
        // 4. 스케일 설정 (부모가 매우 작다면 분모로 나누어 보정해야 함)
        // 여기서는 TILE_SIZE 상수를 기준으로 계산
        float finalScale = (TILE_SIZE * turretScaleValue);
        turret.transform.localScale = new Vector3(
            finalScale / tile.transform.localScale.x,
            finalScale / tile.transform.localScale.y,
            finalScale / tile.transform.localScale.z
        );

        // 5. Y축 위치 보정 (타일 위로 올리기)
        // 타일의 localScale.y가 매우 낮으므로 world 기준으로 살짝 올림
        float tileTopY = tile.transform.position.y + (tile.transform.localScale.y * 0.5f);
        turret.transform.position = new Vector3(tile.transform.position.x, tileTopY, tile.transform.position.z);

        turret.name = $"Turret_{tile.x}_{tile.y}";

        // 6. 상태 업데이트
        tile.IsTurretInstalled = true;
        tile.InstalledTurret = turret;

        Debug.Log($"[TurretInstaller] 포탑 설치 완료: ({tile.x}, {tile.y})");
    }
}