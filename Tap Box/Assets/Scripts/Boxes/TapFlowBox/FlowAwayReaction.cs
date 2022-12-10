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
        [SerializeField] private float _distanse;
        [SerializeField] private float _distanseToDieAction;


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
                GameField.Instance.CheckForWin();
                await MoveOut();
            }
            else
            {
                await MoveTo(box);
            }
        }


        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(_parent.position, _parent.forward * _distanse, Color.red);
        }

        private async UniTask MoveOut()
        {
            bool isPlayDie = false;
            Vector3 startPos = _parent.position;
            while (Vector3.Distance(_parent.position, startPos) < _distanse)
            {
                if (!isPlayDie && Vector3.Distance(_parent.position, startPos) > _distanseToDieAction)
                {
                    isPlayDie = true;
                    GetComponent<IDieAction>()?.DieAction();
                }
                _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.Yield();
            }

            _isMove = false;
            Destroy(_parent.gameObject);
        }

        private async UniTask MoveTo(BaseBox box)
        {
            while (Vector3.Distance(_parent.position, box.transform.position) > 1.03f)
            {
                _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.Yield();
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