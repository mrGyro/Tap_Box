using UnityEngine;

public class BaseBox : MonoBehaviour
{
    public enum BlockType
    {
        None = 0,
        TapFlowBox = 10
    }

    public GameField.BoxData Data;
   

    public virtual void BoxReaction()
    {
        
    }
}
