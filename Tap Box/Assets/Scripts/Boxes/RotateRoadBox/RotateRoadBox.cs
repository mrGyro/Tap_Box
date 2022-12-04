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

        public override async UniTask BoxReactionStart()
        {
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
            CheckNearest();
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
                    var boxGameObject = box.gameObject;
                    boxGameObject.SetActive(false);
                    Destroy(boxGameObject, 0.2f);
                }

                foreach (var box in list)
                {
                    GameField.Instance.RemoveBox(box);
                }
            }

            Debug.LogError(GameField.Instance.GetBoxesCount());
        }
    }
}