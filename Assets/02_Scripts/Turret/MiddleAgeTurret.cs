using UnityEngine;

public class MiddleAgeTurret : Turret
{
    protected override void Init()
    {
        base.Init();

        type = TurretType.MIDDLE_AGE_TURRET;
    }
}
