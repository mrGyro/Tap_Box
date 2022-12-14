using System;
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

    private bool _isEnable = true;
    private int _layerMask;
    private const string GameFieldElement = "GameFieldElement";

    public void SetActiveInput(bool value)
    {
        LeanTouch.Fingers.Clear();
        _isEnable = value;
    }

    private void Start()
    {
        _layerMask = LayerMask.GetMask(GameFieldElement);
    }

    void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleFingerTap;
        LeanTouch.OnGesture += Swipe;
    }

    private void Swipe(List<LeanFinger> fingers)
    {
        switch (fingers.Count)
        {
            case 1:
            {
                _rotate.Rotate(fingers);
                break;
            }
            case 2:
                _zoom.SetZoom(fingers);
                break;
        }
    }

    private async void HandleFingerTap(LeanFinger finger)
    {
        if (!_isEnable)
            return;
        
        var box = RaycastBox(finger.ScreenPosition);

        if (box == null)
            return;

        await box.BoxReactionStart();
    }
    
    private BaseBox RaycastBox(Vector2 screenPosition)
    {
        var hit = RaycastBox(screenPosition, _layerMask);
        if (hit.collider == null)
            return null;
        
        var box = hit.collider.GetComponent<BaseBox>();

        return box == null ? null : box;
    }

    public RaycastHit RaycastBox(Vector2 screenPosition, int layerMask)
    {
        var ray = camera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out var hit, 1000, layerMask))
        {
            Debug.DrawRay(camera.transform.position, ray.direction * 1000, Color.yellow, 5);
        }

        return hit;
    }

    private void OnDestroy()
    {
        LeanTouch.OnFingerTap -= HandleFingerTap;
        LeanTouch.OnGesture -= Swipe;
    }
}