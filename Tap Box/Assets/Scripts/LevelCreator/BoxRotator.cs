using System.Collections.Generic;
using System.Linq;
using Boxes;
using Boxes.BigBoxTapFlowBox;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace LevelCreator
{
    public class BoxRotator : MonoBehaviour
    {
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        private BaseBox _currentTargetBox;

        private static List<VectorDirection> VectorDirections = new()
        {
            new() { direction = Vector3.left, angle = 0 },
            new() { direction = Vector3.left, angle = 90 },
            new() { direction = Vector3.left, angle = 180 },
            new VectorDirection() { direction = Vector3.left, angle = 270 },
            new VectorDirection() { direction = Vector3.up, angle = 90 },
            new VectorDirection() { direction = Vector3.up, angle = -90 },
        };

        public void SetBox(BaseBox box)
        {
            _currentTargetBox = box;
        }

        public static void Rotate(int index, BaseBox box)
        {
            box.transform.rotation = Quaternion.Euler(Vector3.zero);
            box.Rotate(VectorDirections[index].direction, VectorDirections[index].angle);
        }

        private struct VectorDirection
        {
            public Vector3 direction;
            public float angle;
        }

        
    }
}