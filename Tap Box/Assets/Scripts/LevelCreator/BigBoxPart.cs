using UnityEngine;

public class BigBoxPart : MonoBehaviour
{
    public Vector3 ArrayPosition;
    private float _size = 1.03f;

    public void UpdateArrayPosition()
    {
        ArrayPosition = new Vector3((int)(transform.position.x / _size),
            (int)(transform.position.y / _size),
            (int)(transform.position.z / _size));

    }
}