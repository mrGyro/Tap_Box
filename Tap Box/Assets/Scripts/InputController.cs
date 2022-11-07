using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Camera camera;
    void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerTap += HandleFingerTap;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
       
    }
    
    void HandleFingerTap(Lean.Touch.LeanFinger finger)
    {
        var layerMask = LayerMask.GetMask($"GameFieldElement");
        var ray = camera.ScreenPointToRay(finger.ScreenPosition);
        if (!Physics.Raycast(ray, out var hit, 1000, layerMask))
        {
            Debug.DrawRay(camera.transform.position, ray.direction  * 1000, Color.yellow, 5);
            return;
        }

        var box = hit.transform.GetComponent<BaseBox>();
        
        if(box == null)
            return;
        
        Debug.DrawRay(camera.transform.position, ray.direction  * 1000, Color.red, 5);
        box.BoxReaction();
    }
}
