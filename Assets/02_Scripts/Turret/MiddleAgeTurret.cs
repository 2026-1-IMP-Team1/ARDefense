using UnityEngine;
using static TurretStats;

public class MiddleAgeTurret : Turret
{
    protected override void Init()
    {
        base.Init();

        type         = TurretType.MIDDLE_AGE_TURRET;
        hp           = MIDDLE_AGE_TURRET_HP;
        maxHp        = hp;
        attackDamage = MIDDLE_AGE_TURRET_ATTACK_DAMAGE;
        attackSpeed  = MIDDLE_AGE_TURRET_ATTACK_SPEED;

        Debug.Log($"{name}: hp - {hp}, damage - {attackDamage}, range - {attackRange}, speed - {attackSpeed}");
    }
}
