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
    }

    public void SpendGold(int gold_to_spend)
    {
        // 골드 소비 로직 구현
    }
}
