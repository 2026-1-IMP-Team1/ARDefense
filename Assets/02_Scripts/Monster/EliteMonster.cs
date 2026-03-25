using UnityEngine;
using static MonsterHP;
using static MonsterAttackDamage;
using static MonsterAttackSpeed;

public class EliteMonster : Monster
{
    public override void Init()
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
