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
        private List<RotateRoadBox> _nearestBoxes;

        private void Start()
        {
            _dieAction = GetComponents<IDieAction>();
        }

        public async override UniTask Init()
        {
            _nearestBoxes = GetNearestBoxesLine(this, Data.Type);   
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
            var list = GetNearestAndCanConnected(_nearestBoxes);

            if (_nearestBoxes.Count == list.Count)
            {
                foreach (var box in list)
                {
                    box.Die();
                }

                foreach (var box in list)
                {
                    Managers.Instance.GameField.RemoveBox(box);
                }
            }
            else
            {
                _collider.enabled = true;
            }

            Managers.Instance.GameField.CheckForWin();
        }

        private List<RotateRoadBox> GetNearestAndCanConnected(List<RotateRoadBox> nearestBoxes)
        {
            var list = new List<RotateRoadBox> { this };

            for (var index = 0; index < list.Count; index++)
            {
                foreach (var box in nearestBoxes)
                {
                    var rotateRoadBox = box;

                    if (!rotateRoadBox)
                        continue;

                    if (!list[index].GetConnections().HasConnection(rotateRoadBox.GetConnections()))
                        continue;

                    if (!list.Exists(x => x == rotateRoadBox))
                        list.Add(rotateRoadBox);
                }
            }

            return list;
        }

        private List<RotateRoadBox> GetNearestBoxesLine(RotateRoadBox box, BlockType type)
        {
            var line = new List<RotateRoadBox> { box };
            var nearestBoxes = GetNearestBoxes(box, type);
            var buffer = new List<RotateRoadBox>(nearestBoxes);

            while (buffer.Count > 0)
            {
                List<RotateRoadBox> list = new List<RotateRoadBox>();
                for (var index = 0; index < buffer.Count; index++)
                {
                    if (!nearestBoxes.Exists(x => x == buffer[index]))
                        nearestBoxes.Add(buffer[index]);

                    var currentNearestBoxes = GetNearestBoxes(buffer[index], buffer[index].Data.Type);
                    foreach (var VARIABLE in currentNearestBoxes)
                    {
                        if (!line.Exists(x => x == VARIABLE))
                            line.Add(VARIABLE);

                        if (!nearestBoxes.Exists(x => x == VARIABLE))
                            list.Add(VARIABLE);
                    }
                }

                buffer = new List<RotateRoadBox>(list);
            }

            return line;
        }

        private List<RotateRoadBox> GetNearestBoxes(RotateRoadBox box, BlockType type = BlockType.None)
        {
            var thisTransform = box.transform;
            var forward = thisTransform.forward;
            var right = thisTransform.right;
            var positions = new List<Vector3>
            {
                box.Data.ArrayPosition + forward,
                box.Data.ArrayPosition - forward,
                box.Data.ArrayPosition + right,
                box.Data.ArrayPosition - right
            };

            var result = new List<RotateRoadBox>();
            foreach (var pos in positions)
            {
                var nearBox = Managers.Instance.GameField.GetBoxFromArrayPosition(pos) as RotateRoadBox;
                if (nearBox == null)
                    continue;

                if (type != BlockType.None && nearBox.Data.Type != type)
                    continue;

                result.Add(nearBox);
            }

            return result;
        }
    }
}