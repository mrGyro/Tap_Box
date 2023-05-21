using Boxes;
using Boxes.BigBoxTapFlowBox;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator2
{
    public class BoxMover : MonoBehaviour
    {
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;

        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        [SerializeField] private Button forwardButton;
        [SerializeField] private Button beckButton;

        private BaseBox _currentTargetBox;
        private float _size = 1.03f;
        private LevelCreator _creator;
        private bool _isPossibleToMove;

        private void Start()
        {
            upButton.onClick.AddListener(MoveUp);
            downButton.onClick.AddListener(MoveDown);
            leftButton.onClick.AddListener(MoveLeft);
            rightButton.onClick.AddListener(MoveRight);

            forwardButton.onClick.AddListener(MoveForward);
            beckButton.onClick.AddListener(MoveBack);
            _isPossibleToMove = true;
        }

        public void SetBox(BaseBox box, LevelCreator creator)
        {
            _creator = creator;
            _currentTargetBox = box;
        }

        private void MoveLeft()
        {
            if (_currentTargetBox == null)
                return;
            if (!_isPossibleToMove)
                return;
            _isPossibleToMove = false;
            _currentTargetBox.transform.localPosition += Vector3.left * _size;
            UpdatePosition();
            CheckForRevert(Vector3.left * _size);
            _isPossibleToMove = true;
        }

        private void MoveRight()
        {
            if (_currentTargetBox == null)
                return;
            if (!_isPossibleToMove)
                return;
            _isPossibleToMove = false;
            _currentTargetBox.transform.localPosition += Vector3.right * _size;

            UpdatePosition();
            CheckForRevert(Vector3.right * _size);
            _isPossibleToMove = true;
        }

        private void MoveUp()
        {
            if (_currentTargetBox == null)
                return;

            if (!_isPossibleToMove)
                return;
            _isPossibleToMove = false;

            _currentTargetBox.transform.localPosition += Vector3.up * _size;

            UpdatePosition();
            CheckForRevert(Vector3.up * _size);
            _isPossibleToMove = true;
        }

        private void MoveDown()
        {
            if (_currentTargetBox == null)
                return;
            if (!_isPossibleToMove)
                return;
            _isPossibleToMove = false;
            _currentTargetBox.transform.localPosition += Vector3.down * _size;

            UpdatePosition();
            CheckForRevert(Vector3.down * _size);
            _isPossibleToMove = true;
        }

        private void MoveForward()
        {
            if (_currentTargetBox == null)
                return;
            
            if (!_isPossibleToMove)
                return;
            _isPossibleToMove = false;
            _currentTargetBox.transform.position += Vector3.forward * _size;

            UpdatePosition();
            CheckForRevert(Vector3.forward * _size);
            _isPossibleToMove = true;
        }

        private void MoveBack()
        {
            if (_currentTargetBox == null)
                return;
            if (!_isPossibleToMove)
                return;
            _isPossibleToMove = false;
            _currentTargetBox.transform.position += Vector3.back * _size;
            UpdatePosition();
            CheckForRevert(Vector3.back * _size);
            _isPossibleToMove = true;
        }

        private void CheckForRevert(Vector3 direction)
        {
            var bigBox = _currentTargetBox as BigBoxTapFlowBox;
            bool needToRevert = false;
            if (bigBox != null)
            {
                var arrayPositions = bigBox.GetBoxPositions();
                foreach (var bigBoxPart in arrayPositions)
                {
                    if (_creator.IsBoxInPosition(bigBoxPart.ArrayPosition, _currentTargetBox))
                    {
                        needToRevert = true;
                        break;
                    }
                }
            }
            else
            {
                needToRevert = _creator.IsBoxInPosition(_currentTargetBox.Data.ArrayPosition, _currentTargetBox);
            }

            Debug.LogError(needToRevert);
            if (!needToRevert)
                return;

            _currentTargetBox.transform.localPosition -= direction;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (_currentTargetBox.Data.Type == BaseBox.BlockType.BigBoxTapFlowBox)
            {
                (_currentTargetBox as BigBoxTapFlowBox)?.UpdatePositions();
                LevelCreator.OnLevelChanged?.Invoke();
                return;
            }

            var position = _currentTargetBox.transform.position;
            _currentTargetBox.Data.ArrayPosition = new Vector3(Mathf.Round((position.x / _size)),
                Mathf.Round((position.y / _size)),
                Mathf.Round((position.z / _size)));
            LevelCreator.OnLevelChanged?.Invoke();
        }
    }
}