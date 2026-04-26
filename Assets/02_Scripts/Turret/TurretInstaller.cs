using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private GameObject middleAgeTurretPrefab;
    [SerializeField] private GameObject modernAgeTurretPrefab;
    [SerializeField] private GameObject futureAgeTurretPrefab;

    [Header("포탑 스케일 배율 (1이면 tileSize와 동일한 크기로 설정)")]
    [SerializeField, Range(0.5f, 1f)] private float turretScaleValue = 0.9f;

    [Header("타일 Layer Mask (특정 레이어만 터치 감지할 때 설정)")]
    [SerializeField] private LayerMask tileLayerMask = ~0;

    //포탑 설치 UI 저장 변수[lyh]
    public GameObject PlantUI;

    [Header("포탑 선택 UI")]
    [SerializeField] private GameObject turretSelectUI;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI upgradeCountText;

    // 터치한 타일 정보 저장 변수[lyh]
    Tile tile;

    private Turret selectedTurret;
    private Tile selectedTile;
    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        // GameBoard가 생성되기 전에는 터치 입력을 받지 않습니다.
        if (!gameBoardGenerator.IsGameBoardGenerated) return;

        if (!TryGetTouchBeganPosition(out Vector2 screenPos)) return;

        // PlantUI 버튼 등 UI 위의 터치는 무시
        if (IsPointerOverUI(screenPos)) return;

        HandleTileTouch(screenPos);
    }

    private bool IsPointerOverUI(Vector2 screenPos)
    {
        var eventData = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
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

        // 레이어 마스크 없이 캐스트 → Turret·Tile 모두 감지
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            CloseBothUI();
            return;
        }

        // ① hit한 오브젝트(또는 부모)에 Turret이 있으면 → 포탑 선택 UI
        Turret hitTurret = hit.collider.GetComponentInParent<Turret>();
        if (hitTurret != null)
        {
            selectedTurret = hitTurret;
            selectedTile   = FindTileForTurret(hitTurret);
            RefreshTurretInfoUI(hitTurret);
            PlantUI.SetActive(false);
            turretSelectUI.SetActive(true);
            return;
        }

        // ② Tile이면 → 설치 UI (빈 타일) 또는 fallback 포탑 선택
        tile = hit.collider.GetComponent<Tile>();
        if (tile == null)
        {
            CloseBothUI();
            return;
        }

        if (tile.IsTurretInstalled)
        {
            selectedTile   = tile;
            selectedTurret = tile.InstalledTurret != null
                ? tile.InstalledTurret.GetComponent<Turret>()
                : null;
            if (selectedTurret != null) RefreshTurretInfoUI(selectedTurret);
            PlantUI.SetActive(false);
            turretSelectUI.SetActive(selectedTurret != null);
            return;
        }

        turretSelectUI.SetActive(false);
        PlantUI.SetActive(true);
    }

    // Tile의 InstalledTurret을 순회해서 해당 포탑이 설치된 타일을 찾는다
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

    private void RefreshTurretInfoUI(Turret turret)
    {
        if (hpText          != null) hpText.text           = $"HP : {turret.HP:F0} / {turret.MaxHP:F0}";
        if (damageText      != null) damageText.text       = $"Damage : {turret.AttackDamage:F0}";
        if (upgradeCountText != null) upgradeCountText.text = $"Upgrade : {turret.UpgradeCount}";
    }

    private void CloseBothUI()
    {
        PlantUI.SetActive(false);
        turretSelectUI.SetActive(false);
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

    public void UpgradeTurret()
    {
        if (selectedTurret == null) return;
        if (!GoldManager.Instance.SpendGold(TURRET_UPGRADE_COST)) return;

        selectedTurret.Upgrade();
        RefreshTurretInfoUI(selectedTurret);
    }

    public void DestroyTurret()
    {
        if (selectedTurret == null) return;

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
        GameObject turretPrefab = GetTurretPrefabForCurrentAge();
        if (turretPrefab == null)
        {
            Debug.LogWarning($"[TurretInstaller] {GameManager.Instance.CurrentAge} 프리팹이 설정되지 않았습니다.");
            return;
        }
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