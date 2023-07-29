using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class CheatManager : MonoBehaviour, IInitializable
    {
        [SerializeField] private Button _cheatButton;
        [SerializeField] private GameObject _debugConsole;
        [SerializeField] private GameObject _cheatRemoveButton;

        private const int CountToOpenCheatPanel = 20;
        private int _currentCountToOpenCheatPanel;
        public void Initialize()
        {
            _debugConsole.SetActive(false);
            _cheatRemoveButton.SetActive(false);
            _cheatButton.onClick.AddListener(OnCheatClick);
        }

        private void OnCheatClick()
        {
            _currentCountToOpenCheatPanel++;

            if (_currentCountToOpenCheatPanel < CountToOpenCheatPanel)
            {
                return;
            }
            
            Show();
            _currentCountToOpenCheatPanel = CountToOpenCheatPanel;
        }

        private void Show()
        {
            _debugConsole.SetActive(true);
            _cheatRemoveButton.SetActive(true);
        }
    }
}
