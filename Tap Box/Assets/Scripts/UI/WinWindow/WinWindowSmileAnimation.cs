using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.WinWindow
{
    public class WinWindowSmileAnimation : MonoBehaviour
    {
        [SerializeField] private float _distanceForJump;
        [SerializeField] private float _timeForJump;
        [SerializeField] private AnimationCurve _topDownSpeed;

        [SerializeField] private float _angleForSideMove;

        [SerializeField] private float _timeForRotate;

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Transform _transform;
        private bool _isPlaying;

        public void Play()
        {
            if (_transform == null)
            {
                _transform = transform;
            }

            _startPosition = _transform.position;
            _startRotation = _transform.rotation;
            _isPlaying = true;
            DoAnimation();
        }

        public void Stop()
        {
            if (_transform == null)
            {
                _transform = transform;
            }

            _isPlaying = false;
            _transform.position = _startPosition;
            _transform.rotation = _startRotation;
        }

        private void DoAnimation()
        {
            TopDownMove();
        }

        private async UniTask TopDownMove()
        {
            float currentJumpTime = 0;
            float currentRotateTime = 0;
            bool isUp = true;
            while (_isPlaying)
            {
                currentJumpTime += Time.deltaTime;
                if (currentJumpTime >= _timeForJump)
                {
                    currentJumpTime = 0;
                }

                currentRotateTime += Time.deltaTime;

                if (currentRotateTime >= _timeForRotate)
                {
                    isUp = !isUp;
                    currentRotateTime = 0;
                }

                float speed = _topDownSpeed.Evaluate(currentJumpTime / _timeForJump);
                Vector3 cur = _startPosition + new Vector3(0, _distanceForJump * speed, 0);
                _transform.position = cur;

                float time = isUp ? 1f - (currentRotateTime / _timeForRotate) : currentRotateTime / _timeForRotate;
                float angle = 2 * _angleForSideMove * time;
                _transform.rotation = Quaternion.Euler(_startRotation.eulerAngles + new Vector3(0, 0, -_angleForSideMove + angle));


                await UniTask.WaitForEndOfFrame(this);
            }

            Stop();
        }
    }
}

// }
// else
// {
//     if (_transform.localRotation.eulerAngles.z is < 1 and > -1)
//     {
//         _transform.localRotation = Quaternion.Euler(Vector3.zero);
//     }
//     else
//     {
//         float speeds = 250 * Time.deltaTime;
//         float angle = _transform.localRotation.eulerAngles.z > 200 ? _transform.localRotation.eulerAngles.z + speeds : _transform.localRotation.eulerAngles.z - speeds;
//         _transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
//     }
//
//     currentRotateTime = _timeForRotate / 2;
// }