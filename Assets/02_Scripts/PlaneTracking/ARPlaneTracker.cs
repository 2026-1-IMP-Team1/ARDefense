using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaneTracker : MonoBehaviour
{
    [Header("AR 관련하여 참조가 필요한 컴포넌트들")]
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Plane의 최소 가로/세로 길이")]
    private float minWidth = 1.0f; // 가로 1m
    private float minHeight = 1.5f; // 세로 1.5m

    public event Action<ARPlane> OnPlaneFound;

    public ARPlane activePlane { get; private set; }

    void OnEnable() 
    {
        planeManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    void OnDisable()
    {
        planeManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (activePlane != null) return;

        foreach (var plane in args.added) CheckPlaneCondition(plane);
        foreach (var plane in args.updated) CheckPlaneCondition(plane);
    }

    private void CheckPlaneCondition(ARPlane plane_to_check)
    {
        if (activePlane != null) return;

        if (plane_to_check.alignment == PlaneAlignment.HorizontalUp)
        {
            if (plane_to_check.size.x >= minWidth && plane_to_check.size.y >= minHeight)
            {
                activePlane = plane_to_check;
                Debug.Log("Plane 탐지");
                
                // 나중에 UI 띄우는 로직 여기에 넣으면 될 거 같습니다.
                // "게임을 진행할 수 있는 Plane을 찾았습니다! 게임을 진행합니다."

                planeManager.enabled = false;

                foreach (var rest_of_plane in planeManager.trackables)
                {
                    if (rest_of_plane != activePlane) rest_of_plane.gameObject.SetActive(false);
                }

                OnPlaneFound?.Invoke(activePlane);
            }
        }
    }
}
