using Boxes.Reactions;
using UnityEngine;

namespace Boxes.TapFlowBox
{
    public class TapFlowBox : BaseBox
    {
        [SerializeField] private BaseReaction _reaction;
        public override async void BoxReaction()
        {
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
        }
    
    }
}