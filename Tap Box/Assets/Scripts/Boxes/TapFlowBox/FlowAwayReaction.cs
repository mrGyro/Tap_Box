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
        private const string GameFieldElement = "GameFieldElement";

        public override async UniTask ReactionStart()
        {
            if (_isMove)
                return;

            _isMove = true;

            var box = Game.Instance.GameField.GetNearestBoxInDirection(_box.Data.ArrayPosition, _parent.forward);
            if (box == null)
            {
                Game.Instance.GameField.RemoveBox(_box);
                Game.Instance.GameField.CheckForWin();
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

        private async UniTask MoveToAndBack(BaseBox box)
        {
            Vector3 startPos = _parent.position;
            while (Vector3.Distance(_parent.position, box.transform.position) > 1.03f)
            {
                _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.Yield();
            }
            
            while (Vector3.Distance(_parent.position, startPos) > 1.03f)
            {
                _parent.Translate(-_parent.forward * Time.deltaTime * _speed, Space.World);
                await UniTask.Yield();
            }

            _parent.position = startPos;
            _isMove = false;
        }
    }
}