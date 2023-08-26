using Boxes.Reactions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Boxes.TapFlowBox
{
    public class TapFlowBox : BaseBox
    {
        [SerializeField] private BaseReaction _reaction;

        public override async UniTask BoxReactionStart()
        {
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
        }
    }
}