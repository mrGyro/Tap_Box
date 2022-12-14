using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Boxes
{
    public class BaseBox : MonoBehaviour
    {
        public enum BlockType
        {
            None = 0,
            TapFlowBox = 10,
            RotateRoadBox = 20,
            SwipedBox = 30
        }

        public BoxData Data;

        public virtual async UniTask BoxReactionStart()
        {
        }
        
        public virtual async UniTask BoxReactionProcess()
        {
        }
        
        public virtual async UniTask BoxReactionEnd()
        {
        }
    }
}