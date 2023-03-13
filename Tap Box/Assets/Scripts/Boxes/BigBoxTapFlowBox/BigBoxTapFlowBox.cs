using System.Collections.Generic;
using Boxes.Reactions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Boxes.BigBoxTapFlowBox
{
    [RequireComponent(typeof(BaseReaction))]
    public class BigBoxTapFlowBox : BaseBox
    {
        [SerializeField] private BaseReaction _reaction;
        [SerializeField] private List<BaseBox> boxes;

        public override async UniTask BoxReactionStart()
        {
            await _reaction.ReactionStart();
            await _reaction.ReactionEnd();
        }

        public override bool IsBoxInPosition(Vector3 position)
        {
            foreach (var baseBox in boxes)
            {
                if (baseBox.IsBoxInPosition(position))
                    return true;
            }
            
            return false;
        }
    }
}