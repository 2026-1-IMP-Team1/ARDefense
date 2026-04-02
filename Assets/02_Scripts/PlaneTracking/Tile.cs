using UnityEngine;

public class Tile : MonoBehaviour
{
    public static readonly float TILE_SIZE = 0.1f;

    public int x;
    public int y;

    private bool isTurretInstalled;
    public bool IsTurretInstalled => isTurretInstalled;

    private GameObject installedTurret = null;
    public GameObject InstalledTurret => installedTurret;
}
