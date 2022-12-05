using System.Collections.Generic;
using Cinemachine;
using Lean.Touch;
using UnityEngine;

namespace DefaultNamespace
{
    public class LeanCameraZoom : MonoBehaviour
    {
        [Tooltip("The camera that will be zoomed (None = MainCamera)")]
        public Camera Camera;

        [Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")] [Range(-1.0f, 1.0f)]
        public float WheelSensitivity;

        [Tooltip("The current FOV/Size")] public float Zoom = 50.0f;

        [Tooltip("Limit the FOV/Size?")] public bool ZoomClamp;

        [Tooltip("The minimum FOV/Size we want to zoom to")]
        public float ZoomMin = 10.0f;

        [Tooltip("The maximum FOV/Size we want to zoom to")]
        public float ZoomMax = 60.0f;

        public CinemachineVirtualCamera _camera;

        public void SetZoom(List<LeanFinger> fingers)
        {
            Zoom *= LeanGesture.GetPinchRatio(fingers, WheelSensitivity);

            if (ZoomClamp)
            {
                Zoom = Mathf.Clamp(Zoom, ZoomMin, ZoomMax);
            }

            Camera.fieldOfView = Zoom;

            _camera.m_Lens.FieldOfView = Zoom;
        }
    }
}