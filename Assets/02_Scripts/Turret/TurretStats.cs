using UnityEngine;

public class TurretStats
{
    // No need to manage turret stats separately by era for now.
    // The implementation remains as is since the 'type' system is yet to be finalized.
    
    [Header("Turret HP")]
    // Added additional health and attack power per Phase. 
    // A weight adjustment of approximately 1/4 (1.25x) has been added per Phase.
    public static float MIDDLE_AGE_TURRET_HP => 10.0f * (GameManager.Instance.Phase * 1.25f);
    public static float MODERN_AGE_TURRET_HP => 50.0f * (GameManager.Instance.Phase * 1.25f);
    public static float FUTURE_AGE_TURRET_HP => 200.0f * (GameManager.Instance.Phase * 1.25f);

    public static float TURRET_HP => 50.0f * (GameManager.Instance.Phase * 1.25f);

    [Header("Turret Attack Damage")]
    public static float MIDDLE_AGE_TURRET_ATTACK_DAMAGE => 5.0f * (GameManager.Instance.Phase * 1.25f);
    public static float MODERN_AGE_TURRET_ATTACK_DAMAGE => 15.0f * (GameManager.Instance.Phase * 1.25f);
    public static float FUTURE_AGE_TURRET_ATTACK_DAMAGE => 30.0f * (GameManager.Instance.Phase * 1.25f);

    public static float TURRET_ATTACK_DAMAGE => 50.0f * (GameManager.Instance.Phase * 1.25f);

    [Header("Turret Attack Speed")]
    // Further consideration is needed regarding the attack speed values.
    // Decisions are pending on whether to use an integer representing attacks per second 
    // or to calculate speed using specific formulas similar to other games.
    public static float MIDDLE_AGE_TURRET_ATTACK_SPEED => 2;
    public static float MODERN_AGE_TURRET_ATTACK_SPEED => 5;
    public static float FUTURE_AGE_TURRET_ATTACK_SPEED => 8;

    public static float TURRET_ATTACK_SPEED => 2.0f * (GameManager.Instance.Phase * 1.25f);

    [Header("Turret Attack Range")]
    public static float TURRET_ATTACK_RANGE => 5.0f;
}