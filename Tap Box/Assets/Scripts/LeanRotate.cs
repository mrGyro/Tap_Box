using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

namespace DefaultNamespace
{
    public class LeanRotate : MonoBehaviour
    {
        [Tooltip("The camera we will be moving (None = MainCamera)")]
        public Camera Camera;
        public Transform rot;

        [SerializeField] Transform _centr;
        [SerializeField] private float distanceToTarget = 10;
        [SerializeField] private float decriceSpeed = 0.1f;

        private Vector3 maxVector = new Vector3(100000, 100000, 100000);
        private Vector3 _previousPosition = new Vector3(100000, 100000, 100000);
        private float _rotationAroundYAxis;
        private float _rotationAroundXAxis;

        private bool _isActive;

        private void Update()
        {
            if (_isActive)
            {
                Velocity();
                distanceToTarget = Vector3.Distance(rot.transform.position, _centr.position);
            }
        }

        public void SetActive(bool value)
        {
            _isActive = value;
        }

        public void SetStartPosition(Vector3 position)
        {
            float distance = Vector3.Distance(position, _centr.position);
            SetDistanceToTarget(distance);
            rot.position = position;
            rot.LookAt(_centr.position);
        }

        public virtual void Rotate(List<LeanFinger> fingers)
        {
            if (!_isActive)
                return;

            if (fingers[0].Age == 0)
            {
                _previousPosition = Camera.ScreenToViewportPoint(fingers[0].LastScreenPosition);
                //distanceToTarget = Vector3.Distance(rot.transform.position, _centr.position);
            }
            else
            {
                if (_previousPosition == maxVector)
                {
                    _previousPosition = Camera.ScreenToViewportPoint(fingers[0].LastScreenPosition);
                }

                SetAxises(fingers[0].ScreenPosition);
                Move();
            }
        }

        private void SetAxises(Vector3 position)
        {
            Vector3 newPosition2 = Camera.ScreenToViewportPoint(position);

            Vector3 direction = _previousPosition - newPosition2;
            _rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            _rotationAroundXAxis = direction.y * 180; // camera moves vertically
            _previousPosition = newPosition2;
        }

        public void SetTargetPosition(Vector3 position)
        {
            _centr.position = position;
        }

        public Vector3 GetTargetPosition()
        {
            return _centr.position;
        }

        private void SetDistanceToTarget(float distance)
        {
            distanceToTarget = distance;
        }

        private void Move()
        {
            rot.position = _centr.position;

            rot.Rotate(new Vector3(0, 1, 0), _rotationAroundYAxis);
            rot.Rotate(new Vector3(1, 0, 0), _rotationAroundXAxis);
            rot.Translate(new Vector3(0, 0, -distanceToTarget));
        }

        private void Velocity()
        {
            if (_rotationAroundYAxis != 0 || _rotationAroundXAxis != 0)
                Move();

            var xDirection = _rotationAroundXAxis < 0 ? decriceSpeed : -decriceSpeed;
            var yDirection = _rotationAroundYAxis < 0 ? decriceSpeed : -decriceSpeed;

            if (_rotationAroundXAxis != 0)
                _rotationAroundXAxis += xDirection;
            if (_rotationAroundYAxis != 0)
                _rotationAroundYAxis += yDirection;

            if (yDirection < 0 && _rotationAroundYAxis < 0)
                _rotationAroundYAxis = 0;
            if (xDirection < 0 && _rotationAroundXAxis < 0)
                _rotationAroundXAxis = 0;

            _rotationAroundXAxis = Mathf.Clamp(_rotationAroundXAxis, -10, 10);
            _rotationAroundYAxis = Mathf.Clamp(_rotationAroundYAxis, -10, 10);
        }
    }
}