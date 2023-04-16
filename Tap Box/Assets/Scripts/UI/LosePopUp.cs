using DefaultNamespace.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LosePopUp : PopUpBase
    {
        [SerializeField] private Image _smileImage;
        [SerializeField] private Button _closePopup;
        [SerializeField] private Button _addMoreTurns;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _subTitle;
        [SerializeField] private TMP_Text _tapToCloseText;
        public override void Initialize()
        {
            ID = Constants.PopUps.LosePopUp;
            Priority = 100;
            _closePopup.onClick.RemoveAllListeners();
            _closePopup.onClick.AddListener(() =>
            {
                Managers.Instance.LoadLevelById(Managers.Instance.Progress.LastStartedLevelID);
                Managers.Instance.UIManager.ClosePopUp(ID);
            });
            
            _addMoreTurns.onClick.RemoveAllListeners();
            _addMoreTurns.onClick.AddListener((() =>
            {
                Managers.Instance.GameField.GetTurnsCount += 10;
                Managers.Instance.UIManager.ShowTurns();
                Managers.Instance.UIManager.ClosePopUp(ID);
            }));
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}