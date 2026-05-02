using UnityEngine;
using static MonsterStats;

public sealed class BossMonster : Monster
{
    // Initializes stats according to the BossMonster type.
    public sealed override void Init()
    {
        base.Init();

        type = MonsterType.BOSS_MONSTER;

        hp = BOSS_MONSTER_HP;
        attackDamage = BOSS_MONSTER_ATTACK_DAMAGE;
        attackSpeed = BOSS_MONSTER_ATTACK_SPEED;

        // Initializes the isDead variable, which determines survival status, to false (kwj)
        isDead = false;
    }

    private void Update()
    {
        // You can add unique logic for boss monsters (e.g., using special skills) here.
    }
}
