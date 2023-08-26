using Cysharp.Threading.Tasks;
using DefaultNamespace.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Loading
{
    public class LoadingPopup : PopUpBase
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _icon;
        private bool _isStarted = false;
        private Color _startColor = Color.black;
        private Color _endColor = Color.black;
        
        public override void Initialize()
        {
            ID = Constants.PopUps.LoadingPopup;
            Priority = 100;
            _endColor.a = 0.3f;
            _startColor.a = 0f;
        }

        public override void Show()
        {
            _background.color = _startColor;
            gameObject.SetActive(true);
            _isStarted = true;
            Rotate();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
            _isStarted = false;
        }

        private async UniTask Rotate()
        {
            while (_isStarted)
            {
                _background.color = Color.Lerp(_background.color, _endColor, 0.01f);
                _icon.transform.Rotate(new Vector3(0, 0, -100 * Time.deltaTime));
                await UniTask.WaitForEndOfFrame(this);
            }
        }
    }
}