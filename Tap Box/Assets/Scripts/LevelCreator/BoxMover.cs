using Boxes;
using Boxes.BigBoxTapFlowBox;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
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

        private void Start()
        {
            upButton.onClick.AddListener(MoveUp);
            downButton.onClick.AddListener(MoveDown);
            leftButton.onClick.AddListener(MoveLeft);
            rightButton.onClick.AddListener(MoveRight);

            forwardButton.onClick.AddListener(MoveForward);
            beckButton.onClick.AddListener(MoveBack);
        }

        public void SetBox(BaseBox box)
        {
            _currentTargetBox = box;
        }

        private void MoveLeft()
        {
            if (_currentTargetBox == null)
                return;

            _currentTargetBox.transform.localPosition += Vector3.left * _size;

            UpdatePosition();
        }

        private void MoveRight()
        {
            if (_currentTargetBox == null)
                return;

            _currentTargetBox.transform.localPosition += Vector3.right * _size;

            UpdatePosition();
        }

        private void MoveUp()
        {
            if (_currentTargetBox == null)
                return;

            _currentTargetBox.transform.localPosition += Vector3.up * _size;

            UpdatePosition();
        }

        private void MoveDown()
        {
            if (_currentTargetBox == null)
                return;

            _currentTargetBox.transform.localPosition += Vector3.down * _size;

            UpdatePosition();
        }

        private void MoveForward()
        {
            if (_currentTargetBox == null)
                return;

            _currentTargetBox.transform.position += Vector3.forward * _size;

            UpdatePosition();
        }

        private void MoveBack()
        {
            if (_currentTargetBox == null)
                return;

            _currentTargetBox.transform.position += Vector3.back * _size;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (_currentTargetBox.Data.Type == BaseBox.BlockType.BigBoxTapFlowBox)
            {
                (_currentTargetBox as BigBoxTapFlowBox)?.UpdatePositions();
            }
        }
    }
}