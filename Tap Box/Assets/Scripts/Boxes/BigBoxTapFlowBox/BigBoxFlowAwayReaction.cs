using System.Collections.Generic;
using Boxes.Reactions;
using Core.MessengerStatic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

namespace Boxes.BigBoxTapFlowBox
{
    public class BigBoxFlowAwayReaction : BaseReaction
    {
        [SerializeField] private BaseBox _box;
        [SerializeField] private Transform _parent;
        [SerializeField] private float _speed;
        [SerializeField] private float _distanse;
        [SerializeField] private float _distanseToDieAction;
        [SerializeField] private List<Collider> colliders;

        private bool _isMove;

        public override async UniTask ReactionStart()
        {
            if (_isMove)
                return;

            _isMove = true;

            var bigBoxTapFlowBox = _box as BigBoxTapFlowBox;
            Vector3[] array = bigBoxTapFlowBox.GetBoxPositionsAsVectors();

            var box = GameManager.Instance.GameField.GetNearestBoxInDirection(array, GetDirection(), _box);
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
            foreach (var boxCollider in colliders)
            {
                boxCollider.enabled = false;
            }

            Vector3 size = ((BoxCollider)colliders[0]).size;
            Messenger<Transform, Vector3>.Broadcast(Constants.Events.OnTailStart, transform.GetChild(0), size);

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

        private async UniTask MoveToAndBack(BaseBox box)
        {
            var startPos = _parent.position;
            var bigBox = box as BigBoxTapFlowBox;
            var currentBox = _box as BigBoxTapFlowBox;
            BigBoxPart nearestBox = null;
            float minDistance = float.MaxValue;
            float distance;
            foreach (var currentBoxPart in currentBox.GetBoxPositions())
            {
                switch (box.Data.Type)
                {
                    case BaseBox.BlockType.None:
                    case BaseBox.BlockType.TapFlowBox:
                    case BaseBox.BlockType.RotateRoadBox:
                    case BaseBox.BlockType.SwipedBox:
                        distance = Vector3.Distance(box.transform.position, currentBoxPart.transform.position);
                        if (minDistance > distance)
                        {
                            minDistance = distance;
                            nearestBox = currentBoxPart;
                        }

                        break;
                    case BaseBox.BlockType.BigBoxTapFlowBox:
                        foreach (var currentBigBoxPart in bigBox.GetBoxPositions())
                        {
                            distance = Vector3.Distance(currentBigBoxPart.transform.position, currentBoxPart.transform.position);
                            if (minDistance > distance)
                            {
                                minDistance = distance;
                                nearestBox = currentBoxPart;
                            }
                        }

                        break;
                }
            }

            while (true)
            {
                distance = Vector3.Distance(nearestBox.transform.position, box.transform.position);
                if (distance < 1.03f)
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
    }
}