using DefaultNamespace.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LosePopUp : PopUpBase
    {
        [SerializeField] private Button button;
        public override void Initialize()
        {
            ID = Constants.PopUps.LosePopUp;
            Priority = 100;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Managers.Instance.LoadLevelById(Managers.Instance.Progress.LastStartedLevelID);
                Managers.Instance.UIManager.ClosePopUp(ID);
            });
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