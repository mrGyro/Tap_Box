using System.Linq;
using Boxes;
using Currency;
using Managers;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BombBoosterUI : MonoBehaviour
{
    private const string GameFieldElement = "GameFieldElement";

    [SerializeField] private int _bombCost;
    [SerializeField] private Image _bombCross;
    [SerializeField] private Image _bombIcon;
    [SerializeField] private Image _canselIcon;
    [SerializeField] private Button _bombButton;
    [SerializeField] private TMP_Text _bombButtonText;

    [SerializeField] private GameObject _bombInputObject;
    [SerializeField] private GameObject _bombVfx;
    [SerializeField] private EventTrigger _eventTrigger;

    private Vector2 _offset;
    private int _layerMask;
    private bool _isActive = false;

    public void Initialize()
    {
        _bombButtonText.text = _bombCost.ToString();
        _bombInputObject.SetActive(false);
        _bombButton.onClick.AddListener(ClickOnBombButton);
        var pointerDown = _eventTrigger.triggers.FirstOrDefault(x => x.eventID == EventTriggerType.PointerDown);
        pointerDown?.callback.AddListener(OnPointerDown);

        var pointerUp = _eventTrigger.triggers.FirstOrDefault(x => x.eventID == EventTriggerType.PointerUp);
        pointerUp?.callback.AddListener(OnPointerUp);

        var pointerDrag = _eventTrigger.triggers.FirstOrDefault(x => x.eventID == EventTriggerType.Drag);
        pointerDrag?.callback.AddListener(OnPointerDrag);

        _layerMask = LayerMask.GetMask(GameFieldElement);
    }

    public int GetPrice()
    {
        return _bombCost;
    }

    public void ClickOnBombButton()
    {
        if (_isActive || GameManager.Instance.CurrencyController.GetCurrency(CurrencyController.Type.Coin) >= _bombCost)
        {
            _bombInputObject.SetActive(!_bombInputObject.activeSelf);
            _bombIcon.gameObject.SetActive(!_bombInputObject.activeSelf);
            _canselIcon.gameObject.SetActive(_bombInputObject.activeSelf);
            _isActive = _canselIcon.gameObject.activeSelf;
        }
        else
        {
            GameManager.Instance.UIManager.ShowPopUp(Constants.PopUps.NotEnoughCoinPopup);
        }
    }

    private void OnPointerDrag(BaseEventData arg0)
    {
        _bombCross.transform.position = arg0.currentInputModule.input.mousePosition + _offset;
    }

    private void OnPointerUp(BaseEventData arg0)
    {
        _bombCross.gameObject.SetActive(false);
        GameManager.Instance.InputController.SetActiveAllInput(true);
        var distance = Vector2.Distance(arg0.currentInputModule.input.mousePosition, _bombInputObject.transform.position);
        int minDistance = Screen.height / 6;

        if (distance < minDistance)
        {
            return;
        }

        var x = GameManager.Instance.InputController.RaycastBox(arg0.currentInputModule.input.mousePosition + _offset, _layerMask);
        if (x.collider != null)
        {
            GameObject g = Instantiate(_bombVfx, x.point, quaternion.identity);
            Destroy(g, 5);
            BaseBox box = x.transform.GetComponent<BaseBox>();
            GameManager.Instance.GameField.BombBox(box, x.point, Vector3.one);
        }
    }

    private void OnPointerDown(BaseEventData arg0)
    {
        //float size = Screen.width > Screen.height ? Screen.width : Screen.height;
        float size = Screen.width < Screen.height ? Screen.width : Screen.height;
        _offset = Vector2.up * size / 7;
        Debug.LogError(Screen.width + " " + Screen.height + " " + _offset);

        _bombCross.transform.position = arg0.currentInputModule.input.mousePosition + _offset;
        _bombCross.gameObject.SetActive(true);
        GameManager.Instance.InputController.SetActiveAllInput(false);
    }
}