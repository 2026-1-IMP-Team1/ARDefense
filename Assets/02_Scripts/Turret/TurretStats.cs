using UnityEngine;

public class TurretStats
{
    // 새대에 따라 따로 포탑의 스탯을 관리 안해도 됨
    // type을 어떻게 해야할지 아직 미정이라 일단 이렇게 구현했습니다.
    [Header("Turret HP")]
    // Phase당 추가 체력과 공격력을 넣어봤습니다. 가중치 조정은 대강 1/4를 추가해놨습니다.
    public static float TURRET1_HP => 10.0f * (GameManager.Instance.phase * 1.25f);
    public static float TURRET2_HP => 50.0f * (GameManager.Instance.phase * 1.25f);
    public static float TURRET3_HP => 200.0f * (GameManager.Instance.phase * 1.25f);
    
    // phase에 따라 능력치 상향하는 디자인이면 그냥 TURRET_HP 하나로 해도 괜찮지 않을까요? (임시)
    // 물론 시대별 포탑 스탯들을 각각 지정해줘도 상관 없을듯 합니다.
    public static float TURRET_HP => 50.0f * (GameManager.Instance.phase * 1.25f);

    [Header("Turret Attack Damage")]
    public static float TURRET1_ATTACK_DAMAGE => 5.0f * (GameManager.Instance.phase * 1.25f);
    public static float TURRET2_ATTACK_DAMAGE => 15.0f * (GameManager.Instance.phase * 1.25f);
    public static float TURRET3_ATTACK_DAMAGE => 30.0f * (GameManager.Instance.phase * 1.25f);

    // phase에 따라 능력치 상향하는 디자인이면 그냥 TURRET_ATTACK_DAMAGE 하나로 해도 괜찮지 않을까요? (임시)
    // 물론 시대별 포탑 스탯들을 각각 지정해줘도 상관 없을듯 합니다.
    public static float TURRET_ATTACK_DAMAGE => 50.0f * (GameManager.Instance.phase * 1.25f);

    [Header("Turret Attack Speed")]
    // 포탑의 공격 속도 수치 자체에 대해서는 추가적인 고민이 필요할 거 같습니다.
    // int로 해서 단순히 수치 자체가 1초에 공격하는 횟수로 할 지, 아니면 다른 게임에서처럼 특정 공식을 통해 공격 속도를 산출할지 등등...
    // 같이 생각해보면 좋을 거 같아요! 초반부 설계에서만 봤을 때는, 단순하게 하려면 수치 자체를 1초에 공격하는 횟수로 지정해도 괜찮을 거 같습니다...
    public static float TURRET1_ATTACK_SPEED => 2;
    public static float TURRET2_ATTACK_SPEED => 5;
    public static float TURRET3_ATTACK_SPEED => 8;

    // phase에 따라 능력치 상향하는 디자인이면 그냥 TURRET_ATTACK_SPEED 하나로 해도 괜찮지 않을까요? (임시)
    // 물론 시대별 포탑 스탯들을 각각 지정해줘도 상관 없을듯 합니다.
    public static float TURRET_ATTACK_SPEED => 2.0f * (GameManager.Instance.phase * 1.25f);

    [Header("Turret Attack Range")]
    public static float TURRET_ATTACK_RANGE => 5.0f;
}