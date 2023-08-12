using UnityEngine;

public class IgnoreSafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    public void Recalculate(Vector2 screenSize)
    {
        _rectTransform.sizeDelta = screenSize + Vector2.one * 50;
        _rectTransform.localPosition = new Vector3(0, (Screen.safeArea.height - Screen.height) / 2);
    }
}