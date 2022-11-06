using UnityEngine;

public class BaseBox : MonoBehaviour
{
    public enum BlockType
    {
        None = 0,
        TapFlowBox = 10
    }

    public BoxData Data;

    public virtual void BoxReaction()
    {
        
    }
}
