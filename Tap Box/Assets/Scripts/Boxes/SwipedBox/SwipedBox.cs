using Boxes.Reactions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Boxes.SwipableBox
{
    public class SwipedBox : BaseBox
    {
        [SerializeField] private BaseReaction _reaction;

        public override async UniTask BoxReactionStart()
        {
            if (!_reaction.IsReactionOnProcess)
                await _reaction.ReactionStart();
        }

        public override async UniTask BoxReactionProcess()
        {
            if (_reaction.IsReactionOnProcess)
                await _reaction.ReactionProcess();
        }

        public override async UniTask BoxReactionEnd()
        {
            if (_reaction.IsReactionOnProcess)
                await _reaction.ReactionEnd();
        }
    }
}