using UnityEngine;

public static class Gold
{
    [Header("스타팅 골드의 양")]
    public static readonly int GAME_START_GOLD = 100;

    [Header("각 몬스터 타입별로 처치 시 수급하는 골드의 양")]
    public static int NORMAL_MONSTER_GOLD => 5 + GameManager.Instance.WaveWeight;
    public static int ELITE_MONSTER_GOLD => 20 + GameManager.Instance.WaveWeight;
    public static int BOSS_MONSTER_GOLD => 50 + GameManager.Instance.WaveWeight;
}