using DefaultNamespace;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CameraCentrateController : MonoBehaviour
    {
        [SerializeField] private LeanRotate _leanRotate;
        [SerializeField] private Image _defaultIcon;
        [SerializeField] private Image _closeIcon;

        [SerializeField] private Button _centrateButton;
        [SerializeField] private Button _upButton;
        [SerializeField] private Button _downButton;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _toCentr;

        public void Initialize()
        {
            _centrateButton.onClick.AddListener(OnCentrateButtonClick);

            _upButton.onClick.AddListener(Up);
            _downButton.onClick.AddListener(Down);
            _leftButton.onClick.AddListener(Left);
            _rightButton.onClick.AddListener(Right);
            _toCentr.onClick.AddListener(Centr);
        }

        private void Centr()
        {
            _leanRotate.SetTargetPosition(GameManager.Instance.GameField.GetNewCenter());
        }

        private void OnCentrateButtonClick()
        {
            if (GameManager.Instance.UIManager.HasBlocker() && !_closeIcon.gameObject.activeSelf)
            {
                return;
            }
            _upButton.gameObject.SetActive(!_upButton.gameObject.activeSelf);
            _downButton.gameObject.SetActive(!_downButton.gameObject.activeSelf);
            _leftButton.gameObject.SetActive(!_leftButton.gameObject.activeSelf);
            _rightButton.gameObject.SetActive(!_rightButton.gameObject.activeSelf);
            _toCentr.gameObject.SetActive(!_toCentr.gameObject.activeSelf);
            
            _defaultIcon.gameObject.SetActive(!_upButton.gameObject.activeSelf);
            _closeIcon.gameObject.SetActive(_upButton.gameObject.activeSelf);

            GameManager.Instance.InputController.SetActiveTouchInput(!_upButton.gameObject.activeSelf);
        }

        private void Up()
        {
            var center = _leanRotate.GetTargetPosition();
            center += _leanRotate.transform.up;
            if (ClampedVector(center))
            {
                _leanRotate.SetTargetPosition(center);
            }
        }

        private void Down()
        {
            var center = _leanRotate.GetTargetPosition();
            center -= _leanRotate.transform.up;
            if (ClampedVector(center))
            {
                _leanRotate.SetTargetPosition(center);
            }
        }

        private void Left()
        {
            var center = _leanRotate.GetTargetPosition();
            center -= _leanRotate.transform.right;
            if (ClampedVector(center))
            {
                _leanRotate.SetTargetPosition(center);
            }
        }

        private void Right()
        {
            var center = _leanRotate.GetTargetPosition();
            center += _leanRotate.transform.right;
            if (ClampedVector(center))
            {
                _leanRotate.SetTargetPosition(center);
            }
        }

        private bool ClampedVector(Vector3 position)
        {
            Vector3 max = GameManager.Instance.GameField.GetMaxLevelSize() + Vector3.one * 10;
            Vector3 min = GameManager.Instance.GameField.GetMinLevelSize() - Vector3.one * 10;
            if (position.x < min.x || position.x > max.x)
            {
                return false;
            }

            if (position.y < min.y || position.y > max.y)
            {
                return false;
            }

            if (position.z < min.z || position.z > max.z)
            {
                return false;
            }

            return true;
        }
    }
}