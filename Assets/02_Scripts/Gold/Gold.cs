using UnityEngine;

public static class Gold
{
    [Header("스타팅 골드의 양")]
    public static readonly int GAME_START_GOLD = 100;

    [Header("각 몬스터 타입별로 처치 시 수급하는 골드의 양")]

    public static readonly int PhaseWeight = GameManager.Instance.Phase;
    // 페이즈마다 추가 골드 가중치를 넣어봤습니다. (세대진화)
    // 가중치 조정은 대강 1/3를 추가해놨습니다.
    public static readonly int NORMAL_MONSTER_GOLD = 5 + PhaseWeight * 3;
    public static readonly int ELITE_MONSTER_GOLD = 20 + PhaseWeight * 4;
    public static readonly int BOSS_MONSTER_GOLD = 50 + PhaseWeight * 5;


}