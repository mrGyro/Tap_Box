using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Boxes.Reactions
{
    public class BaseReaction : MonoBehaviour
    {
        public bool IsReactionOnProcess { get; protected set; }

        public async virtual UniTask ReactionStart()
        {

        }
        
        public async virtual UniTask ReactionProcess()
        {

        }

        public async virtual UniTask ReactionEnd()
        {
        
        }
    }
}
