using System;
using Boxes.Reactions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Boxes.TapFlowBox
{
    public class FlowAwayReaction : BaseReaction
    {
        [SerializeField] private BaseBox _box;
        [SerializeField] private Transform _parent;
        [SerializeField] private float _speed;

        private bool _isMove;
        private int _layerMask;
        private const string GameFieldElement = "GameFieldElement";

        public override async UniTask ReactionStart()
        {
            if (_isMove)
                return;

            _isMove = true;
            _layerMask = LayerMask.GetMask(GameFieldElement);

            var box = GetNearestForwardBox();
            if (box == null)
            {
                GameField.Instance.RemoveBox(_box);
                await MoveOut();
            }
            else
            {
                await MoveTo(box);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(_parent.position, _parent.forward * 50, Color.red);
        }

        private async UniTask MoveOut()
        {
            var x = _parent.forward * 50;
            while (Vector3.Distance(_parent.position, x) > 1.03f)
            {
                _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.Delay(30);
            }

            _isMove = false;
        }

        private async UniTask MoveTo(BaseBox box)
        {
            while (Vector3.Distance(_parent.position, box.transform.position) > 1.03f)
            {
                _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.Delay(30);
            }

            var nearestPosition = GetNearestPosition(box);
            _box.Data.ArrayPosition = GameField.Instance.GetIndexByWorldPosition(nearestPosition);
            _parent.position = nearestPosition;
            _isMove = false;
        }

        private Vector3 GetNearestPosition(BaseBox box)
        {
            var direction = _box.Data.ArrayPosition - box.Data.ArrayPosition;
            return GameField.Instance.GetWorldPosition(box.Data.ArrayPosition + direction.normalized);
        }

        private BaseBox GetNearestForwardBox()
        {
            var ray = new Ray(_parent.position, _parent.forward * 1000);
            if (!Physics.Raycast(ray, out var hit, 1000, _layerMask))
            {
                return null;
            }

            var box = hit.transform.GetComponent<BaseBox>();

            return box;
        }
    }
}