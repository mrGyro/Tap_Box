using System;
using UnityEngine;

namespace Boxes
{
    [Serializable]
    public class BoxData
    {
        public BaseBox.BlockType Type;
        public Vector3 ArrayPosition;
        public Vector3 Rotation;
    }
}