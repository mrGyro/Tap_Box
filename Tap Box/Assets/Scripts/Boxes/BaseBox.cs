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
            SwipedBox = 30,
            BigBoxTapFlowBox = 40
        }

        public BoxData Data;
        
        public virtual async UniTask Init()
        {
        }

        public virtual async UniTask BoxReactionStart()
        {
        }
        
        public virtual async UniTask BoxReactionProcess()
        {
        }
        
        public virtual async UniTask BoxReactionEnd()
        {
        }
        
        public virtual bool IsBoxInPosition(Vector3 position)
        {
            return Data.ArrayPosition.ToVector3() == position;
        }
    }
}