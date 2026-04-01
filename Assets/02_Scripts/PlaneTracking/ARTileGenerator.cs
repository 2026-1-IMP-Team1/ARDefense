using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARTileGenerator : MonoBehaviour
{
    [Header("AR Plane Tracker")]
    [SerializeField] private ARPlaneTracker planeTracker;

    [Header("타일(정사각형)의 가로/세로 길이")]
    [SerializeField] private float tileSize = 0.2f; // 가로-세로 : 0.2m
}
