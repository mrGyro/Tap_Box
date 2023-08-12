using UnityEngine;

public class IgnoreSafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    public void Recalculate(Vector2 screenSize)
    {
        _rectTransform.sizeDelta = screenSize;
        _rectTransform.transform.TransformPoint(Vector3.zero);
    }
}
