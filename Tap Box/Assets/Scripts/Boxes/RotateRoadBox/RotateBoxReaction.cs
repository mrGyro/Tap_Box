using Boxes.Reactions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Boxes.RotateRoadBox
{
    public class RotateBoxReaction : BaseReaction
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private float _speed;

        private bool _isMove;
    
        public override async UniTask ReactionStart()
        {
            if (_isMove)
                return;

            _isMove = true;
            await RotateBox();
        }

        public override async UniTask ReactionEnd()
        {
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(_parent.position, _parent.forward * 50, Color.red);
            Debug.DrawRay(_parent.position, _parent.right * 50, Color.green);
            Debug.DrawRay(_parent.position, _parent.up * 50, Color.blue);
        }

        private async UniTask RotateBox()
        {
            var target = Quaternion.LookRotation(_parent.right, _parent.up);
            while (_parent.localRotation != target)
            {
                var rot = Quaternion.RotateTowards(transform.localRotation, target, _speed * Time.deltaTime);
                _parent.localRotation = rot;
                await UniTask.Delay(30);
            }

            _isMove = false;
        }
    }
}
