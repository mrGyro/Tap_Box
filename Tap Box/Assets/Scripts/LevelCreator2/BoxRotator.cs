using System.Collections.Generic;
using Boxes;
using UnityEngine;

public class BoxRotator : MonoBehaviour
{
    private static readonly List<VectorDirection> VectorDirections = new()
    {
        new VectorDirection { direction = Vector3.left, angle = 0 },
        new VectorDirection { direction = Vector3.left, angle = 90 },
        new VectorDirection { direction = Vector3.left, angle = 180 },
        new VectorDirection { direction = Vector3.left, angle = 270 },
        new VectorDirection { direction = Vector3.up, angle = 90 },
        new VectorDirection { direction = Vector3.up, angle = -90 },
    };

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