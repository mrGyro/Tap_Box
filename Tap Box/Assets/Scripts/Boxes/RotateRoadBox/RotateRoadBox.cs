using System.Collections.Generic;
using Boxes.Reactions;
using UnityEngine;

namespace Boxes.RotateRoadBox
{
    public class RotateRoadBox : BaseBox
    {
        [SerializeField] private BoxConnections _connections;
        [SerializeField] private BaseReaction _reaction;

        public override async void BoxReaction()
        {
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
            CheckNearest();
        }

        public BoxConnections GetConnections()
        {
            return _connections;
        }

        private void CheckNearest()
        {
            var nearestBoxes = GameField.Instance.GetNearestBoxesLine(this, Data.Type);
            List<RotateRoadBox> list = new List<RotateRoadBox> { this };

            for (var index = 0; index < list.Count; index++)
            {
                foreach (var box in nearestBoxes)
                {
                    var rotateRoadBox = box as RotateRoadBox;

                    if (rotateRoadBox == null)
                    {
                        continue;
                    }

                    if (list[index].GetConnections().HasConnection(rotateRoadBox.GetConnections()))
                    {
                        if (!list.Exists(x => x == rotateRoadBox))
                        {
                            list.Add(rotateRoadBox);
                        }
                    }
                }
            }

            if (nearestBoxes.Count == list.Count)
            {
                foreach (var VARIABLE in list)
                {
                    VARIABLE.gameObject.SetActive(false);
                    Destroy(VARIABLE.gameObject,0.2f);
                }
                
                foreach (var VARIABLE in list)
                {
                    GameField.Instance.RemoveBox(VARIABLE);
                }
            }
            Debug.LogError(GameField.Instance.GetBoxesCount());
        }
    }
}