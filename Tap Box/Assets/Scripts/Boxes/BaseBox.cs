using UnityEngine;

namespace Boxes
{
    public class BaseBox : MonoBehaviour
    {
        public enum BlockType
        {
            None = 0,
            TapFlowBox = 10,
            RotateRoadBox = 20
        }

        public BoxData Data;

        public virtual void BoxReaction()
        {
        }
    }
}