using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ResetLevelButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        void Start()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            GameManager.Instance.Progress.LastSavedLevelDataID = string.Empty;
            GameManager.Instance.LoadLevelById(GameManager.Instance.Progress.LastStartedLevelID);
        }
    }
}