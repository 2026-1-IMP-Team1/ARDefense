using UnityEngine;

public class Tile : MonoBehaviour
{
    // scale이 1이면 현실에서 1m입니다!
    public static readonly float TILE_SIZE = 0.1f;

    public int x;
    public int y;

    private bool isTurretInstalled;
    public bool IsTurretInstalled => isTurretInstalled;

    private GameObject installedTurret = null;
    public GameObject InstalledTurret => installedTurret;
}
