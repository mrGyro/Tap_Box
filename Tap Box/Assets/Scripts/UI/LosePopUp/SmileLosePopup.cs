using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class SmileLosePopup : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetScale;
        [SerializeField] private float _timeForJump;
        [SerializeField] private AnimationCurve _topDownSpeed;


        private Vector3 _startScale;
        private Transform _transform;
        private bool _isPlaying;

        public void Play()
        {
            if (_transform == null)
            {
                _transform = transform;
            }

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
            _transform.localScale = Vector3.one;
        }

        private void DoAnimation()
        {
            TopDownMove();
        }

        private async UniTask TopDownMove()
        {
            _startScale = transform.localScale;
            Vector3 differance = _targetScale - _startScale;
            float currentJumpTime = 0;
            while (_isPlaying)
            {
                currentJumpTime += Time.deltaTime;
                if (currentJumpTime >= _timeForJump)
                {
                    currentJumpTime = 0;
                }

                float speed = _topDownSpeed.Evaluate(currentJumpTime / _timeForJump);
                Vector3 cur = _startScale + differance * speed;
                _transform.localScale = cur;

                await UniTask.WaitForEndOfFrame(this);
            }

            Stop();
        }
    }
}