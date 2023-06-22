using System.Collections.Generic;
using Boxes.Reactions;
using Core.MessengerStatic;
using Cysharp.Threading.Tasks;
using Managers;
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
        [SerializeField] private Collider _collider;

        private bool _isMove;

        public override async UniTask ReactionStart()
        {
            if (_isMove)
                return;

            _isMove = true;

            var box = GameManager.Instance.GameField.GetNearestBoxInDirection(new[] { _box.Data.ArrayPosition.ToVector3() } , _parent.forward, _box);
            if (box == null)
            {
                GameManager.Instance.GameField.RemoveBox(_box);
                GameManager.Instance.GameField.CheckForWin();
                await MoveOut();
            }
            else
            {
                await MoveToAndBack(box);
            }
        }
        
        public Vector3 GetDirection()
        {
            return _parent.forward;
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(_parent.position, _parent.forward * _distanse, Color.red);
        }

        private async UniTask MoveOut()
        {
            Messenger<Transform, Vector3>.Broadcast(Constants.Events.OnTailStart, transform.GetChild(0), ((BoxCollider)_collider).size);

            _collider.enabled = false;
            var isPlayDie = false;
            var startPos = _parent.position;
            while (Vector3.Distance(_parent.position, startPos) < _distanse)
            {
                if (!isPlayDie && Vector3.Distance(_parent.position, startPos) > _distanseToDieAction)
                {
                    isPlayDie = true;
                    GetComponent<IDieAction>()?.DieAction();
                }

                _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.WaitForEndOfFrame(this);
            }

            _isMove = false;
            Destroy(_parent.gameObject);
        }

        private async UniTask MoveToAndBack(BaseBox targetBaseBox)
        {
            var startPos = _parent.position;
            var nearestBoxPosition = GetNearestPositionMoveAndBack(targetBaseBox, out var contactBox);

            while (true)
            {
                var distance = Vector3.Distance(nearestBoxPosition, contactBox.position);
                if (distance < GameField.Size)
                {
                    break;
                }

                _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.WaitForEndOfFrame(this);
            }

            while (Vector3.Distance(_parent.position, startPos) > 0.2f)
            {
                _parent.Translate(-_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.WaitForEndOfFrame(this);
            }

            _parent.position = startPos;
            _isMove = false;
        }
        
        private Vector3 GetNearestPositionMoveAndBack(BaseBox targetBaseBox, out Transform contactBox)
        {
            float minDistance = float.MaxValue;
            float distance = 0;
            Vector3 result = Vector3.zero;
            List<Vector3> targetPosition = new List<Vector3>();

            switch (targetBaseBox.Data.Type)
            {
                case BaseBox.BlockType.None:
                case BaseBox.BlockType.TapFlowBox:
                case BaseBox.BlockType.RotateRoadBox:
                case BaseBox.BlockType.SwipedBox:
                    targetPosition.Add(targetBaseBox.transform.position);
                    break;
                case BaseBox.BlockType.BigBoxTapFlowBox:
                    var targetBigBox = targetBaseBox as BigBoxTapFlowBox.BigBoxTapFlowBox;
                    var targetBoxParts = targetBigBox.GetBoxPositions();
                    foreach (var VARIABLE in targetBoxParts)
                    {
                        targetPosition.Add(VARIABLE.ArrayPosition * GameField.Size);
                    }

                    break;
            }

            contactBox = null;
            {
                foreach (var targetBigBoxPart in targetPosition)
                {
                    distance = Vector3.Distance(targetBigBoxPart, _box.transform.position);
                    if (minDistance > distance)
                    {
                        contactBox = _box.transform;
                        minDistance = distance;
                        result = targetBigBoxPart;
                    }
                }
            }

            return result;
        }
    }
}