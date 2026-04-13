// 몬스터들의 HP, 공격력, 공격 속도, 이동 속도 등등 몬스터 스탯 수치가 명시되어 있는 MonsterStats.cs 클래스입니다.
// 수치 자체를 일일이 외부 스크립트에서 사용할 때마다 하드 코딩하는 것보다, 해당 스크립트에 있는 클래스들의 readonly 상수들을 참조하시면 되겠습니다.
// 나중에 밸런스 등의 이유로 특정 타입의 몬스터의 스탯을 변경해야 할 때, 여기 있는 상수 값 하나만 바꾸면 되도록 하기 위함입니다!

// 사용하는 법 : 예를 들어 외부 스크립트에서 일반 몬스터의 HP 값을 참조하고 싶을 경우,
// 스크립트 상단에 using static MonsterStats; 를 작성하시고 NOMRAL_MONSTER_HP를 마음껏 사용하시면 됩니다.

// 참고로 수치는 진짜 임의대로 아무거나 넣어놓은 것입니다.

using UnityEngine;

public class MonsterStats
{

    [Header("Monster HP")]
    // 웨이브당 추가 체력과 공격력을 넣어봤습니다. 가중치 조정은 대강 1/4를 추가해놨습니다.
    public static readonly float NORMAL_MONSTER_HP = 10.0f * (GameManager.Instance.phase * 1.25f);
    public static readonly float ELITE_MONSTER_HP = 50.0f * (GameManager.Instance.phase * 1.25f);
    public static readonly float BOSS_MONSTER_HP = 200.0f * (GameManager.Instance.phase * 1.25f);

    [Header("Monster Attack Damage")]
    public static readonly float NORMAL_MONSTER_ATTACK_DAMAGE = 5.0f * (GameManager.Instance.phase * 1.25f);
    public static readonly float ELITE_MONSTER_ATTACK_DAMAGE = 15.0f * (GameManager.Instance.phase * 1.25f);
    public static readonly float BOSS_MONSTER_ATTACK_DAMAGE = 30.0f * (GameManager.Instance.phase * 1.25f);

    // 몬스터 웨이브마다 몇마리씩 나오는지 대강 정의
    [Header("Monster Number")]
    public static readonly int NORMAL_MONSTER_Number = 10;
    public static readonly int ELITE_MONSTER_Number = 2;
    public static readonly int BOSS_MONSTER_Number = 1;

    [Header("Monster Attack Speed")]
    // 몬스터의 공격 속도 수치 자체에 대해서는 추가적인 고민이 필요할 거 같습니다.
    // int로 해서 단순히 수치 자체가 1초에 공격하는 횟수로 할 지, 아니면 다른 게임에서처럼 특정 공식을 통해 공격 속도를 산출할지 등등...
    // 같이 생각해보면 좋을 거 같아요! 초반부 설계에서만 봤을 때는, 단순하게 하려면 수치 자체를 1초에 공격하는 횟수로 지정해도 괜찮을 거 같습니다...
    public static readonly int NORMAL_MONSTER_ATTACK_SPEED = 2;
    public static readonly int ELITE_MONSTER_ATTACK_SPEED = 5;
    public static readonly int BOSS_MONSTER_ATTACK_SPEED = 8;
}