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
        }
    }
}