using UnityEngine;
using static TurretStats;

public class ModernAgeTurret : Turret
{
    protected override void Init()
    {
        base.Init();

        type         = TurretType.MODERN_AGE_TURRET;
        hp           = MODERN_AGE_TURRET_HP;
        maxHp        = hp;
        attackDamage = MODERN_AGE_TURRET_ATTACK_DAMAGE;
        attackSpeed  = MODERN_AGE_TURRET_ATTACK_SPEED;

        Debug.Log($"{name}: hp - {hp}, damage - {attackDamage}, range - {attackRange}, speed - {attackSpeed}");
    }
}
