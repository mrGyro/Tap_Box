using System.Collections.Generic;
using Boxes.Reactions;
using Cysharp.Threading.Tasks;
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

            var box = Managers.Instance.GameField.GetNearestBoxInDirection(new[] { _box.Data.ArrayPosition.ToVector3() }, _parent.forward, _box);
            Debug.LogError("reaction");
            if (box == null)
            {
                Managers.Instance.GameField.RemoveBox(_box);
                Managers.Instance.GameField.CheckForWin();
                await MoveOut();
            }
            else
            {
                await MoveToAndBack(box);
            }
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
            bool isMove = true;
            while (isMove)
            {
                foreach (var VARIABLE in bigBox.GetBoxPositions())
                {
                    if (Vector3.Distance(_parent.position, VARIABLE.transform.position) > 1.03f)
                    {
                        isMove = false;
                        break;
                    }
                }

                _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.WaitForEndOfFrame(this);
            }

            while (Vector3.Distance(_parent.position, startPos) > 1.03f)
            {
                _parent.Translate(-_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.WaitForEndOfFrame(this);
            }

            _parent.position = startPos;
            _isMove = false;
        }
    }
}