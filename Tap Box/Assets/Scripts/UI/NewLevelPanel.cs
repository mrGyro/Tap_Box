using DefaultNamespace.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NewLevelPanel : PopUpBase
    {
        [SerializeField] private Button button;
        public override void Initialize()
        {
            ID = Constants.PopUps.NewLevelPopUp;
            Priority = 100;
            Managers.Instance.PlayerLevelManager.OnLevelChanged += LevelChanged;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener((() => Managers.Instance.UIManager.ClosePopUp(ID)));
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
            
        }

        private void LevelChanged(int obj)
        {
            Managers.Instance.UIManager.ShowPopUp(ID);
        }

        private void OnDestroy()
        {
            Managers.Instance.PlayerLevelManager.OnLevelChanged -= LevelChanged;
        }
    }
}