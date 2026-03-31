using UnityEngine;

public class Gold
{
    [Header("스타팅 골드의 양")]
    public static readonly int GAME_START_GOLD = 50;

    [Header("각 몬스터 타입별로 처치 시 수급하는 골드의 양")]
    public static readonly int NORMAL_MONSTER_GOLD = 5;
    public static readonly int ELITE_MONSTER_GOLD = 20;
    public static readonly int BOSS_MONSTER_GOLD = 50;
}