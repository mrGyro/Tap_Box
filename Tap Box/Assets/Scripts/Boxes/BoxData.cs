using System;

namespace Boxes
{
    [Serializable]
    public class BoxData
    {
        public BaseBox.BlockType Type;
        public SerializedVector3 ArrayPosition;
        public SerializedVector3 Rotation;
        public SerializedVector3 Size;
    }
}