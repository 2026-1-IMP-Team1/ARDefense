using UnityEngine;
using static Gold;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [Header("골드 변수 / 프로퍼티")]

    private int currentGold;

    public int Gold
    {
        get
        {
            return currentGold;
        }

        set
        {
            currentGold = value;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentGold = GAME_START_GOLD;
    }

    public void AddGold(int gold_to_add)
    {
        // 골드 추가 로직 구현
        Gold += gold_to_add;
        // 골드 수급 확인 및 현재 골드량 UI와 중복 체크용용
        Debug.Log($"{gold_to_add} 골드 획득, 현재 골드: {Gold}");
    }

    public void SpendGold(int gold_to_spend)
    {
        // 골드 소비 로직 구현
    }
}
