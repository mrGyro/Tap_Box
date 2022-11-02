using UnityEngine;

public class BaseBox : MonoBehaviour
{
    public enum BlockType
    {
        None = 0,
        TapFlowBox = 10
    }
    
    public BlockType Type;
    public Vector3 ArrayPosition;

    public virtual void BoxReaction()
    {
        
    }
}
