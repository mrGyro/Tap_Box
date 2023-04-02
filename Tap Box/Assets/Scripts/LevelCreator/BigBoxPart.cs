using UnityEngine;

public class BigBoxPart : MonoBehaviour
{
    public Vector3 ArrayPosition;
    private float _size = 1.03f;

    public void UpdateArrayPosition()
    {
        ArrayPosition = new Vector3((transform.position.x / _size),
            (transform.position.y / _size),
            (transform.position.z / _size));
    }
}