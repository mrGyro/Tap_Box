using System.Collections.Generic;
using Boxes;
using DefaultNamespace;
using Lean.Touch;
using UnityEngine;


public class InputController : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] LeanCameraZoom _zoom;
    [SerializeField] LeanRotate _rotate;

    void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleFingerTap;
       // LeanTouch.OnGesture += Swipe;
    }

    private void Swipe(List<LeanFinger> fingers)
    {
        if (fingers.Count == 1)
        {
            _rotate.Rotate(fingers);
        }

        if (fingers.Count == 2)
        {
            _zoom.SetZoom(fingers);
        }
    }

    void HandleFingerTap(LeanFinger finger)
    {
        var layerMask = LayerMask.GetMask($"GameFieldElement");
        var ray = camera.ScreenPointToRay(finger.ScreenPosition);
        if (!Physics.Raycast(ray, out var hit, 1000, layerMask))
        {
            Debug.DrawRay(camera.transform.position, ray.direction * 1000, Color.yellow, 5);
            return;
        }

        var box = hit.transform.GetComponent<BaseBox>();

        if (box == null)
            return;

        Debug.DrawRay(camera.transform.position, ray.direction * 1000, Color.red, 5);
        box.BoxReaction();
    }
}