using UnityEngine;

public class BigBoxPart : MonoBehaviour
{
    public Vector3 ArrayPosition;
    public Vector3 ArrayDirection;

    public void UpdateArrayPosition()
    {
        ArrayPosition = new Vector3(Mathf.Round((transform.position.x / GameField.Size)),
            Mathf.Round((transform.position.y / GameField.Size)),
            Mathf.Round((transform.position.z / GameField.Size)));  
    }

    public void ResetPositionToLocalDefaultPlaces()
    {
        transform.localPosition = ArrayPosition * GameField.Size;
    }
}