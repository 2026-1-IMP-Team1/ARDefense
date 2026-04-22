using UnityEngine;

public class ModernAgeTurret : Turret
{
    protected override void Init()
    {
        base.Init();

        type = TurretType.MODERN_AGE_TURRET;
    }
}
