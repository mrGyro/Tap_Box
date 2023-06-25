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

    public void ResetDirections()
    {
        ArrayDirection = new Vector3(Mathf.Round((transform.position.x / GameField.Size)),
            Mathf.Round((transform.position.y / GameField.Size)),
            Mathf.Round((transform.position.z / GameField.Size)));  
    }

    public void ResetPositionToLocalDefaultPlaces()
    {
        transform.localPosition = ArrayPosition * GameField.Size;
    }
    
    public void UpdateArrayPosition(float size)
    {
        ArrayPosition = new Vector3(Mathf.Round((transform.position.x / size)),
            Mathf.Round((transform.position.y / size)),
            Mathf.Round((transform.position.z / size)));  
    }

    public void ResetDirections(float size)
    {
        ArrayDirection = new Vector3(Mathf.Round((transform.position.x / size)),
            Mathf.Round((transform.position.y / size)),
            Mathf.Round((transform.position.z / size)));  
    }

    public void ResetPositionToLocalDefaultPlaces(float size)
    {
        transform.localPosition = ArrayPosition * size;
    }
}