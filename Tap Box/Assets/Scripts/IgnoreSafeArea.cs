using UnityEngine;

public class IgnoreSafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    public void Recalculate(Vector2 screenSize)
    {
        _rectTransform.sizeDelta += new Vector2(Screen.width / 6, Screen.height / 6);
        _rectTransform.localPosition = new Vector3(0, (Screen.safeArea.height - Screen.height) / 2);
    }
}