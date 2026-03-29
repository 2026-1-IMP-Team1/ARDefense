using UnityEngine;
using static MonsterStats;

public sealed class EliteMonster : Monster
{
    public sealed override void Init()
    {
        type = MonsterType.ELITE_MONSTER;

        hp = ELITE_MONSTER_HP;
        attackDamage = ELITE_MONSTER_ATTACK_DAMAGE;
        attackSpeed = ELITE_MONSTER_ATTACK_SPEED;
    }

    void Update()
    {
        // ...
    }
}
