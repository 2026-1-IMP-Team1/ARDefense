using UnityEngine;
using static MonsterStats;

public class BossMonster : Monster
{
    public override void Init()
    {
        type = MonsterType.BOSS_MONSTER;

        hp = BOSS_MONSTER_HP;
        attackDamage = BOSS_MONSTER_ATTACK_DAMAGE;
        attackSpeed = BOSS_MONSTER_ATTACK_SPEED;
    }

    void Update()
    {
        // ...
    }
}
