using UnityEngine;

public class BigBoxPart : MonoBehaviour
{
    public Vector3 ArrayPosition;
    private float _size = 1.03f;




    public void UpdateArrayPosition()
    {
        ArrayPosition = new Vector3(Mathf.Round((transform.position.x / _size)),
            Mathf.Round((transform.position.y / _size)),
            Mathf.Round((transform.position.z / _size)));
    }
}