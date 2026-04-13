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

    public void AddGold(int goldToAdd)
    {
        // 골드 추가 로직 구현
        Gold += goldToAdd;
        // 골드 수급 확인 및 현재 골드량 UI와 중복 체크용용
        Debug.Log($"{goldToAdd} 골드 획득, 현재 골드: {Gold}");
    }

    public bool SpendGold(int goldToSpend)
    {
        // 골드 소비 로직 구현
        if (currentGold < goldToSpend)
        {
            Debug.Log($"골드가 부족합니다: 현재 골드 - {Gold} / 사용하려는 골드 - {goldToSpend}");
            return false;
        }

        Gold -= goldToSpend;
        return true;
    }
}
