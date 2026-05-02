using UnityEngine;
using static MonsterStats;

public sealed class EliteMonster : Monster
{
    // Initializes stats according to the EliteMonster type.
    public sealed override void Init()
    {
        base.Init();

        type = MonsterType.ELITE_MONSTER;

        hp = ELITE_MONSTER_HP;
        attackDamage = ELITE_MONSTER_ATTACK_DAMAGE;
        attackSpeed = ELITE_MONSTER_ATTACK_SPEED;
    }

    private void Update()
    {
        // You can add unique logic for elite monsters here.
    }
}
