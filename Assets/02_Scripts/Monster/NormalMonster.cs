using UnityEngine;
using static MonsterStats;

public class NormalMonster : Monster
{
    public override void Init()
    {
        type = MonsterType.NORMAL_MONSTER;

        hp = NORMAL_MONSTER_HP;
        attackDamage = NORMAL_MONSTER_ATTACK_DAMAGE;
        attackSpeed = NORMAL_MONSTER_ATTACK_SPEED;
    }

    void Update()
    {
        // ...
    }
}
