using UnityEngine;
using static MonsterStats;

public sealed class BossMonster : Monster
{
    public sealed override void Init()
    {
        base.Init();

        type = MonsterType.BOSS_MONSTER;

        hp = BOSS_MONSTER_HP;
        attackDamage = BOSS_MONSTER_ATTACK_DAMAGE;
        attackSpeed = BOSS_MONSTER_ATTACK_SPEED;

        // 생존상태를 결정하는 isDead 변수를 false로 초기화 (kwj)
        isDead = false;
    }

    void Update()
    {
        // ...
    }
}
