using UnityEngine;
using static MonsterStats;

public sealed class NormalMonster : Monster
{
    public sealed override void Init()
    {
        type = MonsterType.NORMAL_MONSTER;

        hp = NORMAL_MONSTER_HP;
        attackDamage = NORMAL_MONSTER_ATTACK_DAMAGE;
        attackSpeed = NORMAL_MONSTER_ATTACK_SPEED;

        isDead = false;
    }

    void Update()
    {
        // ...
    }
}