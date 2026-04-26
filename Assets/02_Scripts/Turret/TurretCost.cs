using UnityEngine;

public static class TurretCost
{
    [Header("Turret Cost")]
    public static int MIDDLE_AGE_TURRET_COST => 10;
    public static int MODERN_AGE_TURRET_COST => 25;
    public static int FUTURE_AGE_TURRET_COST => 50;

    [Header("Upgrade Cost")]
    public const int TURRET_UPGRADE_COST = 50;
}
