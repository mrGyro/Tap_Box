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

        private Vector3 _previousPosition = Vector3.positiveInfinity;
        private float _rotationAroundYAxis;
        private float _rotationAroundXAxis;

        private Vector3 newPosition;
        private void Update()
        {
            Velocity();
            _centr.position = Vector3.Lerp(_centr.position, newPosition, 0.01f);
        }

        public void SetDistanceToTarget(float distance)
        {
            distanceToTarget = distance;
        }
        public virtual void Rotate(List<LeanFinger> fingers)
        {
            if (fingers[0].Age == 0)
            {
                _previousPosition = Camera.ScreenToViewportPoint(fingers[0].LastScreenPosition);
                distanceToTarget = Vector3.Distance(rot.transform.position, _centr.position);
            }
            else
            {
                Vector3 newPosition2 = Camera.ScreenToViewportPoint(fingers[0].ScreenPosition);
                Vector3 direction = _previousPosition - newPosition2;

                _rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
                _rotationAroundXAxis = direction.y * 180; // camera moves vertically

                Move();
                _previousPosition = newPosition2;
            }
        }

        public void SetTargetPosition(Vector3 position)
        {
            newPosition = position;
            _centr.position = position;
        }
        
        public void SetTargetPositionMove(Vector3 position)
        {
            newPosition = position;
        }

        private void Move()
        {
            rot.transform.position = _centr.position;

            rot.transform.Rotate(new Vector3(0, 1, 0), _rotationAroundYAxis);
            rot.transform.Rotate(new Vector3(1, 0, 0), _rotationAroundXAxis);
            rot.transform.Translate(new Vector3(0, 0, -distanceToTarget));
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