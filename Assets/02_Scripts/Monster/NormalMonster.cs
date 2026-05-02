using UnityEngine;
using static MonsterStats;

public sealed class NormalMonster : Monster
{
    // Initializes stats according to the NormalMonster type.
    public sealed override void Init()
    {
        base.Init();

        type = MonsterType.NORMAL_MONSTER;

        hp = NORMAL_MONSTER_HP;
        attackDamage = NORMAL_MONSTER_ATTACK_DAMAGE;
        attackSpeed = NORMAL_MONSTER_ATTACK_SPEED;

        isDead = false;
    }

    private void Update()
    {
        // You can add unique logic for normal monsters here.
    }
}