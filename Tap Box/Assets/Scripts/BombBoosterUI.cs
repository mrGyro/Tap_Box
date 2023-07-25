using System.Linq;
using Boxes;
using Currency;
using Managers;
using TMPro;
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
    [SerializeField] private EventTrigger _eventTrigger;

    private Vector2 _offset = Vector2.up * Screen.width / 8;
    private int _layerMask;

    public void Initialize()
    {
        GameManager.Instance.CurrencyController.OnCurrencyCountChanged += CurrencyCountChanged;
        _bombButton.interactable = GameManager.Instance.CurrencyController.GetCurrency(CurrencyController.Type.Coin) >= _bombCost;

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

    private void OnPointerDrag(BaseEventData arg0)
    {
        _bombCross.transform.position = arg0.currentInputModule.input.mousePosition + _offset;
    }

    private void OnPointerUp(BaseEventData arg0)
    {
        _bombCross.gameObject.SetActive(false);
        GameManager.Instance.InputController.SetActiveAllInput(true);
        var x = GameManager.Instance.InputController.RaycastBox(arg0.currentInputModule.input.mousePosition+ _offset, _layerMask);
        if (x.collider != null)
        {
            BaseBox box = x.transform.GetComponent<BaseBox>();
            GameManager.Instance.GameField.BombBox(box, x.point, Vector3.one * 2);
        }
    }

    private void OnPointerDown(BaseEventData arg0)
    {
        _bombCross.transform.position = arg0.currentInputModule.input.mousePosition + _offset;
        _bombCross.gameObject.SetActive(true);
        GameManager.Instance.InputController.SetActiveAllInput(false);
    }


    private void CurrencyCountChanged(CurrencyController.Type arg1, int arg2)
    {
        if (arg1 != CurrencyController.Type.Coin)
        {
            return;
        }

        _bombButton.interactable = arg2 >= _bombCost;
    }

    private void ClickOnBombButton()
    {
        _bombInputObject.SetActive(!_bombInputObject.activeSelf);
        _bombIcon.gameObject.SetActive(!_bombInputObject.activeSelf);
        _canselIcon.gameObject.SetActive(_bombInputObject.activeSelf);
    }
}