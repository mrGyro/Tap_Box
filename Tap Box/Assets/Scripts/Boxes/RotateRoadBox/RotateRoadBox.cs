using System.Collections.Generic;
using Boxes.Reactions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Boxes.RotateRoadBox
{
    public class RotateRoadBox : BaseBox
    {
        [SerializeField] private BoxConnections _connections;
        [SerializeField] private BaseReaction _reaction;
        [SerializeField] private Collider _collider;

        private IDieAction[] _dieAction;

        private void Start()
        {
            _dieAction = GetComponents<IDieAction>();
        }

        public override async UniTask BoxReactionStart()
        {
            _collider.enabled = false;
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
            CheckNearest();

        }

        private async void Die()
        {
            _collider.enabled = false;
            List<UniTask> tasks = new List<UniTask>();
            foreach (var VARIABLE in _dieAction)
            {
                tasks.Add(VARIABLE.DieAction());
            }

            await UniTask.WhenAll(tasks);
            Destroy(gameObject);
        }

        private BoxConnections GetConnections()
        {
            return _connections;
        }

        private void CheckNearest()
        {
            var nearestBoxes = GameField.Instance.GetNearestBoxesLine(this, Data.Type);
            var list = new List<RotateRoadBox> { this };

            for (var index = 0; index < list.Count; index++)
            {
                foreach (var box in nearestBoxes)
                {
                    var rotateRoadBox = box as RotateRoadBox;

                    if (!rotateRoadBox)
                    {
                        continue;
                    }

                    if (!list[index].GetConnections().HasConnection(rotateRoadBox.GetConnections()))
                        continue;

                    if (!list.Exists(x => x == rotateRoadBox))
                    {
                        list.Add(rotateRoadBox);
                    }
                }
            }

            if (nearestBoxes.Count == list.Count)
            {
                foreach (var box in list)
                {
                    box.Die();
                }

                foreach (var box in list)
                {
                    GameField.Instance.RemoveBox(box);
                }
            }
            else
            {
                _collider.enabled = true;
            }

            GameField.Instance.CheckForWin();
        }
    }
}