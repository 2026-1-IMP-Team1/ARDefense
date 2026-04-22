using UnityEngine;
using static TurretStats;

public class FutureAgeTurret : Turret
{
    protected override void Init()
    {
        base.Init();
        
        type = TurretType.FUTURE_AGE_TURRET;
    }
}
