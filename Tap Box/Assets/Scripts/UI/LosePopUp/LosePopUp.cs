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
        [SerializeField] private SmileLosePopup _smile;
        [SerializeField] private Button _closePopup;
        [SerializeField] private Button _addMoreTurns;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _subTitle;
        [SerializeField] private TMP_Text _tapToCloseText;
        [SerializeField] private TMP_Text _buttonText;

        public override void Initialize()
        {
            ID = Constants.PopUps.LosePopUp;
            Priority = 100;
            _closePopup.onClick.RemoveAllListeners();
            _closePopup.onClick.AddListener(() =>
            {
                GameManager.Instance.Progress.LastSavedLevelDataID = string.Empty;
                GameManager.Instance.LoadLevelById(GameManager.Instance.Progress.LastStartedLevelID);
                GameManager.Instance.UIManager.ClosePopUp(ID);
            });

            _addMoreTurns.onClick.RemoveAllListeners();
            _addMoreTurns.onClick.AddListener((() =>
            {
                if (GameManager.Instance.Mediation.IsReady(Constants.Ads.Interstitial))
                {
                    GameManager.Instance.Mediation.Show(Constants.Ads.Interstitial, ID);
                }

                GameManager.Instance.GameField.GetTurnsCount = GameManager.Instance.GameField.GetTurnsCountAfterLoose();
                GameManager.Instance.UIManager.ShowTurns();
                GameManager.Instance.UIManager.ClosePopUp(ID);
            }));
        }

        public override void Show()
        {
            GameManager.Instance.SoundManager.Play(new ClipDataMessage() { Id = Constants.Sounds.UI.LoseWindowShow, SoundType = SoundData.SoundType.UI });
            gameObject.SetActive(true);
            _buttonText.text = $"+{GameManager.Instance.GameField.GetTurnsCountAfterLoose()} Moves";
            _smile.Play();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
            _smile.Stop();
        }
    }
}