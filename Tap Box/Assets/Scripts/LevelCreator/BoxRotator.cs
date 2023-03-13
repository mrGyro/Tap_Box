using System;
using Boxes;
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

            _currentTargetBox.transform.Rotate(Vector3.up, -90);
            _currentTargetBox.Data.Rotation = _currentTargetBox.transform.rotation.eulerAngles;
        }

        private void RotateRight()
        {
            if (_currentTargetBox == null)
                return;
            _currentTargetBox.transform.Rotate(Vector3.up, 90);
            _currentTargetBox.Data.Rotation = _currentTargetBox.transform.rotation.eulerAngles;
        }

        private void RotateUp()
        {
            if (_currentTargetBox == null)
                return;
            _currentTargetBox.transform.Rotate(Vector3.left, 90);
            _currentTargetBox.Data.Rotation = _currentTargetBox.transform.rotation.eulerAngles;
        }

        private void RotateDown()
        {
            if (_currentTargetBox == null)
                return;
            _currentTargetBox.transform.Rotate(Vector3.left, -90);
            _currentTargetBox.Data.Rotation = _currentTargetBox.transform.rotation.eulerAngles;
        }
    }
}