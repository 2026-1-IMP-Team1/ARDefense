using System;
using UnityEngine;
using static Gold;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [Tooltip("Event")]
    public event Action OnGoldChanged;
    public event Action OnGoldInsufficient;
    

    // 골드를 기본값으로 다시 세팅해주는 메서드
    public void ResetGold()
    {
        Gold = GAME_START_GOLD;
        OnGoldChanged?.Invoke(); // UI 업데이트 이벤트 발생
    }

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
            OnGoldChanged?.Invoke();
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
        if (goldToSpend == -1)
        {
            Debug.Log("잘못된 골드 소비 경로로 접근하였습니다.");
            return false;
        }

        // 골드 소비 로직 구현
        if (currentGold < goldToSpend)
        {
            Debug.Log($"골드가 부족합니다: 현재 골드 - {Gold} / 사용하려는 골드 - {goldToSpend}");
            OnGoldInsufficient?.Invoke();
            return false;
        }

        Gold -= goldToSpend;
        return true;
    }
}
