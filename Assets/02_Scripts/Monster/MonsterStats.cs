// This is the MonsterStats.cs class where monster stats such as HP, attack power, attack speed, movement speed, etc., are specified.
// Instead of hard-coding the values every time you use them in external scripts, you can refer to the readonly constants in the classes of this script.
// This is to ensure that when you need to change the stats of a specific type of monster for balance reasons, you only need to change one constant value here!

// How to use: For example, if you want to reference the HP value of a normal monster from an external script,
// write 'using static MonsterStats;' at the top of the script and feel free to use NORMAL_MONSTER_HP.

// Note that the values are just arbitrary placeholders.

using UnityEngine;

public static class MonsterStats
{

    [Header("Monster HP")]
    // Added extra health and attack power per wave. The weight adjustment is roughly an additional 1/4.
    public static float NORMAL_MONSTER_HP => 10.0f * (GameManager.Instance.Phase * 1.5f);
    public static float ELITE_MONSTER_HP => 70.0f * (GameManager.Instance.Phase * 1.5f);
    public static float BOSS_MONSTER_HP => 120.0f * (GameManager.Instance.Phase * 1.8f);

    [Header("Monster Attack Damage")]
    public static float NORMAL_MONSTER_ATTACK_DAMAGE => 5.0f * (GameManager.Instance.Phase * 1.25f);
    public static float ELITE_MONSTER_ATTACK_DAMAGE => 20.0f * (GameManager.Instance.Phase * 1.25f);
    public static float BOSS_MONSTER_ATTACK_DAMAGE => 50.0f * (GameManager.Instance.Phase * 2.0f);

    // Roughly defines how many monsters appear per wave
    [Header("Monster Number")]
    public static readonly int NORMAL_MONSTER_Number = 20;
    public static readonly int ELITE_MONSTER_Number = 4;
    public static readonly int BOSS_MONSTER_Number = 1;

    [Header("Monster Attack Speed")]
    // Additional thought seems to be needed regarding the monster's attack speed value itself.
    // Whether to make it an int and simply have the value be the number of attacks per second, or to calculate attack speed through a specific formula like in other games, etc...
    // It would be good to think about it together! Looking at the initial design, it seems okay to simply designate the value as the number of attacks per second for simplicity...
    public static readonly int NORMAL_MONSTER_ATTACK_SPEED = 10;
    public static readonly int ELITE_MONSTER_ATTACK_SPEED = 15;
    public static readonly int BOSS_MONSTER_ATTACK_SPEED = 20;
}