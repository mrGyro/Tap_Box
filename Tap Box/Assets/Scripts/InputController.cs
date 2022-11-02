using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Camera camera;

    public void OnPointerClick(PointerEventData eventData)
    {
        int layerMask = LayerMask.GetMask($"GameFieldElement");

        Ray ray = camera.ScreenPointToRay(eventData.position);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1000, layerMask))
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
