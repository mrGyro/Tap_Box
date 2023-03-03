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
            Managers.Instance.LoadLevelById(Managers.Instance.Progress.LastStartedLevelID);
        }
    }
}