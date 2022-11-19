using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Boxes.Reactions
{
    public class BaseReaction : MonoBehaviour
    {
        public async virtual UniTask ReactionStart()
        {

        }

        public async virtual UniTask ReactionEnd()
        {
        
        }
    }
}
