using DefaultNamespace.Managers;
using Managers;
using Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LosePopUp : PopUpBase
    {
        [SerializeField] private Image _background;
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
                GameManager.Instance.LoadLevelById(GameManager.Instance.Progress.LastStartedLevelID);
                GameManager.Instance.UIManager.ClosePopUp(ID);
            });
            
            _addMoreTurns.onClick.RemoveAllListeners();
            _addMoreTurns.onClick.AddListener((() =>
            {
                GameManager.Instance.GameField.GetTurnsCount += 10;
                GameManager.Instance.UIManager.ShowTurns();
                GameManager.Instance.UIManager.ClosePopUp(ID);
            }));
        }

        public override void Show()
        {
            GameManager.Instance.SoundManager.Play(new ClipDataMessage() { Id = Constants.Sounds.UI.LoseWindowShow, SoundType = SoundData.SoundType.UI });
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}