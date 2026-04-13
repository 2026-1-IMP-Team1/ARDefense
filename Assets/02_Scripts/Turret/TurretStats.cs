public static class TurretStats
{
    public static float AttackDamage => 5f + GameManager.Instance.Wave * 2f;
    public static float AttackRange => 10f;
    public static float AttackSpeed => 2f;
}
