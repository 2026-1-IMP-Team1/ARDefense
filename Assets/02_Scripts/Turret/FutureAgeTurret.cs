using UnityEngine;
using static TurretStats;

public class FutureAgeTurret : Turret
{
    protected override void Init()
    {
        base.Init();

        type         = TurretType.FUTURE_AGE_TURRET;
        hp           = FUTURE_AGE_TURRET_HP;
        maxHp        = hp;
        attackDamage = FUTURE_AGE_TURRET_ATTACK_DAMAGE;
        attackSpeed  = FUTURE_AGE_TURRET_ATTACK_SPEED;

        Debug.Log($"{name}: hp - {hp}, damage - {attackDamage}, range - {attackRange}, speed - {attackSpeed}");
    }
}
