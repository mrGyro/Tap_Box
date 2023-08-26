using System.Collections.Generic;
using Boxes;
using UnityEngine;

public class BoxRotator : MonoBehaviour
{
    private static readonly List<VectorDirection> VectorDirections = new()
    {
        new VectorDirection { direction = new Vector3(0, 0, 0), angle = 0 },
        new VectorDirection { direction = new Vector3(0, 90, 0), angle = 0 },
        new VectorDirection { direction = new Vector3(0, 180, 0), angle = 0 },
        new VectorDirection { direction = new Vector3(0, 270, 0), angle = 0 },
        new VectorDirection { direction = new Vector3(0, 180, 90), angle = 0 },
        new VectorDirection { direction = new Vector3(0, 0, 90), angle = 0 },
        new VectorDirection { direction = new Vector3(90, 90, 0), angle = 0 },
        new VectorDirection { direction = new Vector3(-90, 90, 0), angle = 0 },
    };

    public static int GetSize()
    {
        return VectorDirections.Count;
    }

    public static void Rotate(int index, BaseBox box)
    {
        box.transform.rotation = Quaternion.Euler(Vector3.zero);
        box.Rotate(VectorDirections[index].direction);
    }

    private struct VectorDirection
    {
        public Vector3 direction;
        public float angle;
    }
}