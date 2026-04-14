using UnityEngine;

public class TurretCost
{
    [Header("포탑 설치 비용")]
    public static int turretCost => 30 + GameManager.Instance.phase;
}
