using System;
using System.Collections.Generic;
using Boxes.Reactions;
using UnityEngine;

namespace Boxes.RotateRoadBox
{
    public class RotateRoadBox : BaseBox
    {
        public BoxConnections Connections;
        [SerializeField] private BaseReaction _reaction;

        private void Start()
        {
            
        }

        public override async void BoxReaction()
        {
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
            CheckNearest();
        }

        private void CheckNearest()
        {
            var nearestBoxes = GameField.Instance.GetNearestBoxes(this);
            GameField.Instance.GetNearestBoxesLine(this, Data.Type);
            foreach (var nearestBox in nearestBoxes)
            {
                if (nearestBox.Data.Type != BlockType.RotateRoadBox)
                {
                    continue;
                }

                var box = nearestBox as RotateRoadBox;

                if (box == null)
                {
                    continue;
                }

                if (Connections.HasConnection(box.Connections))
                {
                    Debug.LogError(nearestBox.Data.Type + " " + nearestBox.Data.ArrayPosition);
                }
            }
        }
    }
}