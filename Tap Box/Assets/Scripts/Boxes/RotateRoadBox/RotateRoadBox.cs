using Boxes.Reactions;
using UnityEngine;

namespace Boxes.RotateRoadBox
{
    public class RotateRoadBox : BaseBox
    {
        [SerializeField] private BaseReaction _reaction;

        public override async void BoxReaction()
        {
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
            CheckNearest();
        }

        private void CheckNearest()
        {
            var nearestBoxes = GameField.Instance.GetNearestBoxes(this);

            foreach (var VARIABLE in nearestBoxes)
            {
                Debug.LogError(VARIABLE.Data.Type + " " + VARIABLE.Data.ArrayPosition);
            }
        }
    }
}