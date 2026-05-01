using UnityEngine;

public class Tile : MonoBehaviour
{
    // If scale is 1, it's 1m in real life!
    public static readonly float TILE_SIZE = 0.1f;

    public int x;
    public int y;

    private bool isTurretInstalled;
    public bool IsTurretInstalled
    {
        get
        {
            return isTurretInstalled;
        }

        set
        {
            isTurretInstalled = value;
        }
    }

    private GameObject installedTurret = null;
    public GameObject InstalledTurret
    {
        get
        {
            return installedTurret;
        }

        set
        {
            installedTurret = value;
        }
    }
}
