using UnityEngine;

public class TurretStates
{
    // 새대에 따라 따로 포탑의 스탯을 관리 안해도 됨
    // type을 어떻게 해야할지 아직 미정이라 일단 이렇게 구현했습니다.
    [Header("Turret HP")]
    // Phase당 추가 체력과 공격력을 넣어봤습니다. 가중치 조정은 대강 1/4를 추가해놨습니다.
    public static readonly float NORMAL_TURRET_HP = 10.0f * (GameManager.Instance.phase * 1.25f);
    public static readonly float ELITE_TURRET_HP = 50.0f * (GameManager.Instance.phase * 1.25f);
    public static readonly float BOSS_TURRET_HP = 200.0f * (GameManager.Instance.phase * 1.25f);

    [Header("Turret Attack Damage")]
    public static readonly float NORMAL_TURRET_ATTACK_DAMAGE = 5.0f * (GameManager.Instance.phase * 1.25f);
    public static readonly float ELITE_TURRET_ATTACK_DAMAGE = 15.0f * (GameManager.Instance.phase * 1.25f);
    public static readonly float BOSS_TURRET_ATTACK_DAMAGE = 30.0f * (GameManager.Instance.phase * 1.25f);

    // 포탑 웨이브마다 몇마리씩 나오는지 대강 정의
    [Header("Turret Number")]
    public static readonly int NORMAL_TURRET_Number = 10;
    public static readonly int ELITE_TURRET_Number = 2;
    public static readonly int BOSS_TURRET_Number = 1;

    [Header("Turret Attack Speed")]
    // 포탑의 공격 속도 수치 자체에 대해서는 추가적인 고민이 필요할 거 같습니다.
    // int로 해서 단순히 수치 자체가 1초에 공격하는 횟수로 할 지, 아니면 다른 게임에서처럼 특정 공식을 통해 공격 속도를 산출할지 등등...
    // 같이 생각해보면 좋을 거 같아요! 초반부 설계에서만 봤을 때는, 단순하게 하려면 수치 자체를 1초에 공격하는 횟수로 지정해도 괜찮을 거 같습니다...
    public static readonly int NORMAL_TURRET_ATTACK_SPEED = 2;
    public static readonly int ELITE_TURRET_ATTACK_SPEED = 5;
    public static readonly int BOSS_TURRET_ATTACK_SPEED = 8;
}