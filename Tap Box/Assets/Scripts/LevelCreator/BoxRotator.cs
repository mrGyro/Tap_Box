using Boxes;
using Boxes.BigBoxTapFlowBox;
using UnityEngine;
using UnityEngine.UI;

namespace LevelCreator
{
    public class BoxRotator : MonoBehaviour
    {
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        
        private BaseBox _currentTargetBox;

        private void Start()
        {
            upButton.onClick.AddListener(RotateUp);
            downButton.onClick.AddListener(RotateDown);
            leftButton.onClick.AddListener(RotateLeft);
            rightButton.onClick.AddListener(RotateRight);
        }

        public void SetBox(BaseBox box)
        {
            _currentTargetBox = box;
        }
        
        private void RotateLeft()
        {
            if (_currentTargetBox == null)
                return;
            
            _currentTargetBox.Rotate(Vector3.up, -90);
            UpdatePosition();
        }

        private void RotateRight()
        {
            if (_currentTargetBox == null)
                return;
            
            _currentTargetBox.Rotate(Vector3.up, 90);
            UpdatePosition();
        }

        private void RotateUp()
        {
            if (_currentTargetBox == null)
                return;
            
            _currentTargetBox.Rotate(Vector3.left, 90);
            UpdatePosition();

        }

        private void RotateDown()
        {
            if (_currentTargetBox == null)
                return;
            
            _currentTargetBox.Rotate(Vector3.left, -90);
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