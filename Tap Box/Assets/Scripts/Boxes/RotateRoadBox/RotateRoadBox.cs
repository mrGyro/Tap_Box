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
            var nearestBoxes = GetNearestBoxesLine(this, Data.Type);
            var list = GetNearestAndCanConnected(nearestBoxes);

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
            var transform1 = box.transform;
            var up = transform1.up;
            var forward = transform1.forward;
            var right = transform1.right;
            var positions = new List<Vector3>
            {
                // box.Data.ArrayPosition + up,
                // box.Data.ArrayPosition - up,
                box.Data.ArrayPosition + forward,
                box.Data.ArrayPosition - forward,
                box.Data.ArrayPosition + right,
                box.Data.ArrayPosition - right
            };

            var result = new List<RotateRoadBox>();
            foreach (var pos in positions)
            {
                var nearBox = GameField.Instance.GetBoxFromArrayPosition(pos) as RotateRoadBox;
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